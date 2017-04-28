/*************************
 * Samuel Goulet
 * Novembre 2016
 * Projet final Communication par Ordinateur
 ****************************/

 /********************
  * Le code n'est pas très commenté
  * Fiez vous au guide technique en cas
  * de questions.
  *********************/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Net;
using System.Runtime.InteropServices;

using Jeu_Comm.Network.Trames;
using Jeu_Comm.Network;
using Jeu_Comm.Lobby;


namespace Jeu_Comm
{
    // C'est le premier formulaire à apparaitre!
    public partial class frmStartup : Form
    {
        private Client m_Client;
        private Server m_Server;

        // Liste des parties qui sont dans le ListBox
        private List<Partie> m_Partie;

        // Structure contenant les informations sur la partie (Joueurs, Nom, Hôte, etc)
        private LobbyInfo m_LobbyInfo;
        // Numéro local du joueur
        private int m_NumeroJoueur;
        // Indique si le joueur est prêt à jouer
        private bool m_Validated;

        private TextBox[] txtNomJoueur;
        private TextBox[] txtIPJoueur;
        private Panel[] pnCouleurJoueur;
        private Panel[] pnImageCouleur;
        private Color[] m_DefaultColors;
        private GroupBox[] m_GroupBox;


        // Pour pouvoir créer la console
        [DllImport("kernel32.dll")]
        public static extern bool AllocConsole();

        public frmStartup()
        {
            InitializeComponent();
            // Création de la console
            AllocConsole();

            // Recherche de l'IP local
            IPAddress LocalIP = NetworkUtils.GetLocalIPAddress();

            m_NumeroJoueur = -1;
            m_Partie = new List<Partie>();
            m_Client = new Client(this);
            m_Server = null;
            m_LobbyInfo = new LobbyInfo(LocalIP);
            m_Validated = false;



            txtIP.Text = LocalIP.ToString();
            txtNomJoueur = new TextBox[] { txtNomJoueur1, txtNomJoueur2, txtNomJoueur3, txtNomJoueur4 };
            txtIPJoueur = new TextBox[] { txtIPJoueur1, txtIPJoueur2, txtIPJoueur3, txtIPJoueur4 };
            pnCouleurJoueur = new Panel[] { panelColorJoueur1, panelColorJoueur2, panelColorJoueur3, panelColorJoueur4 };
            pnImageCouleur = new Panel[] { panelImageJoueur1, panelImageJoueur2, panelImageJoueur3, panelImageJoueur4 };
            m_DefaultColors = new Color[] { Color.Red, Color.Blue, Color.Yellow, Color.Green };
            m_GroupBox = new GroupBox[] { gbPlayer1, gbPlayer2, gbPlayer3, gbPlayer4 };
            UpdateGUI();

        }

        public LobbyInfo LobbyInfo
        {
            get { return m_LobbyInfo; }
            set { m_LobbyInfo = value; }
        }

        public int NumeroJoueur
        {
            get { return m_NumeroJoueur; }
            set { m_NumeroJoueur = value; }
        }

        /// <summary>
        /// Ajoute une partie au ListBox
        /// </summary>
        /// <param name="p"></param>
        public void AddGameToListBox(Partie p)
        {
            if (!this.IsHandleCreated)
                return;
            // Vérifie que la partie n'existe pas déjà
            int i = 0;
            while (i < m_Partie.Count && (!m_Partie[i].Host.Equals(p.Host) || m_Partie[i].Nom != p.Nom))
                i++; 
            if (i != m_Partie.Count)
                return;
            m_Partie.Add(new Partie(p.Nom, p.Host));
            // Au cas où l'appel de la fonction se fait sur un autre thread
            Invoke(new Action(() =>
            {
                btnHost.Enabled = false;
                lbJoin.Items.Add(p.Nom + " @" + p.Host.ToString());
            }));
        }

        /// <summary>
        /// Enlever une partie du ListBox
        /// </summary>
        /// <param name="p"></param>
        public void RemoveGameFromListBox(Partie p)
        {
            // Vérifie que la partie existe déjà
            int i = 0;
            while (i < m_Partie.Count && (!m_Partie[i].Host.Equals(p.Host) || m_Partie[i].Nom != p.Nom))
                i++;
            if (i == m_Partie.Count)
                return;

            // L'enlève
            m_Partie.RemoveAt(i);

            // L'enlève du ListBox (dans un autre thread possiblement
            if (!IsDisposed)
            Invoke(new Action(() =>
            {
                if (!lbJoin.IsDisposed)
                {
                    lbJoin.Items.RemoveAt(i);
                    if (lbJoin.Items.Count == 0)
                        btnHost.Enabled = true;
                }
            }));
        }

        /// <summary>
        /// Mise à jour des contrôles du formulaire
        /// </summary>
        public void UpdateGUI()
        {
            if (!IsHandleCreated)
                return;
            // Possiblement lancé à partir d'un autre thread
            Invoke(new Action(() =>
            {
                for (int i = 0; i < 4; i++)
                {
                    foreach (Control c in m_GroupBox[i].Controls)
                        c.Enabled = (m_NumeroJoueur == i && m_Validated == false);
                    m_GroupBox[i].Enabled = m_NumeroJoueur == i;
                    txtNomJoueur[i].ReadOnly = m_NumeroJoueur != i;
                }
                txtName.Text = m_LobbyInfo.NomPartie;
                for (int i = 0; i < 4; i++)
                {
                    // Enlever l'évenement, question qu'il ne soit pas lancé lors de l'assignation du texte
                    txtNomJoueur[i].TextChanged -= new EventHandler(NameTextChanged);
                    txtNomJoueur[i].Text = m_LobbyInfo.NomJoueurs[i];
                    txtIPJoueur[i].Text = m_LobbyInfo.IPJoueurs[i].ToString();
                    pnCouleurJoueur[i].BackColor = m_LobbyInfo.Colors[i];
                    pnImageCouleur[i].BackgroundImage = TextureManager.ChangeColor(m_LobbyInfo.Colors[i], Properties.Resources.TexturePlayerFront1);
                    txtNomJoueur[i].TextChanged += new EventHandler(NameTextChanged);
                }
            }));
        }

        /// <summary>
        /// Double click sur un item du ListBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lbJoin_DoubleClick(object sender, EventArgs e)
        {
            // Vérification qu'on est pas "out of bound"
            if (lbJoin.SelectedIndex > m_Partie.Count || lbJoin.SelectedIndex == -1)
                return;
            m_Client.Join(m_Partie[lbJoin.SelectedIndex]);
        }

        /// <summary>
        /// Click sur le bouton
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnHost_Click(object sender, EventArgs e)
        {
            if (m_Server == null)
            {
                if (txtName.Text != "")
                {
                    // Création du Server
                    m_Server = new Server(txtName.Text, this);
                    // Le Client se joint au Server
                    m_Client.Join(m_Partie.Last());
                }
            }
        }

        /// <summary>
        /// Bouton de validation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnValidateClick(object sender, EventArgs e)
        {
            // Mise à jour du LobbyInfo
            m_Validated = true;
            m_LobbyInfo.NomJoueurs[m_NumeroJoueur] = txtNomJoueur[m_NumeroJoueur].Text;
            m_LobbyInfo.Colors[m_NumeroJoueur] = pnCouleurJoueur[m_NumeroJoueur].BackColor;
            m_LobbyInfo.PlayerReady[m_NumeroJoueur] = true;
            // Envoi des données au Server
            m_Client.SendTCP(new GameInfoUpdateTrame(m_Client.LocalIP, m_LobbyInfo, m_NumeroJoueur, true).ToByteArray());
        }
        
        /// <summary>
        /// Click sur l'un des 4 boutons de choix de couleur
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPickColorJoueur2_Click(object sender, EventArgs e)
        {
            if (dlgPlayerColor.ShowDialog() == DialogResult.OK)
            {
                m_LobbyInfo.Colors[m_NumeroJoueur] = dlgPlayerColor.Color;
                pnCouleurJoueur[m_NumeroJoueur].BackColor = dlgPlayerColor.Color;
                UpdateGUI();
                
            }
        }

        /// <summary>
        /// Lancé lorsque le formulaire se ferme
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmStartup_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (m_Server != null)
            {
                m_Server.Stop();
            }
            // Détruit TOUT (en espérant ne pas planter...)
            Environment.Exit(0);

        }

        /// <summary>
        /// Modification dans l'un des textboxes (Oui, ce n'est pas efficace)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NameTextChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < 4; i++)
                m_LobbyInfo.NomJoueurs[i] = txtNomJoueur[i].Text;
        }

    }
}
