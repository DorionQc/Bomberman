using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Windows.Forms;
using System.Threading.Tasks;

using Jeu_Comm.Network;
using Jeu_Comm.Network.Trames;
using Jeu_Comm.CustomEventArgs;
using Jeu_Comm.Lobby;

namespace Jeu_Comm.Network
{
    public delegate void TrameReceivedEventHandler(object sender, TrameReceivedEventArgs e);
    /// <summary>
    /// Classe principale du Client
    /// </summary>
    public partial class Client
    {
        // IP local
        private IPAddress m_LocalIP;
        // IP de l'hôte
        private IPAddress m_HostIP;
        private LobbyInfo m_LobbyInfo;

        // Indique si on est connecté
        private bool m_Connected;
        private byte m_ID; // Identifiant du client, de 0 à 3

        private DateTime m_TimeSent;

        // Les deux formulaires
        private frmStartup m_StartupForm;
        private frmJeu m_frmJeu;

        private event TrameReceivedEventHandler ReceivedTrame;

        public event TrameReceivedEventHandler ReceivedBroadcastPacket;
        public event TrameReceivedEventHandler ReceivedGameInfoUpdatePacket;
        public event TrameReceivedEventHandler ReceivedRegisterPlayerPacket;
        public event TrameReceivedEventHandler ReceivedGameStartEndPacket;
        public event TrameReceivedEventHandler ReceivedPlayerMovePacket;
        public event TrameReceivedEventHandler ReceivedPlayerPositionUpdatePacket;
        public event TrameReceivedEventHandler ReceivedPlayerDiePacket;
        public event TrameReceivedEventHandler ReceivedBlockBreakPacket;
        public event TrameReceivedEventHandler ReceivedBlockPlacePacket;
        public event TrameReceivedEventHandler ReceivedBombPlacePacket;
        public event TrameReceivedEventHandler ReceivedBombExplodePacket;
        public event TrameReceivedEventHandler ReceivedPlayerPickupBombPacket;
        public event TrameReceivedEventHandler ReceivedPlayerThrowBombPacket;
        public event TrameReceivedEventHandler ReceivedPlayerKickBombPacket;
        public event TrameReceivedEventHandler ReceivedPlayerPickupBonusPacket;
        public event TrameReceivedEventHandler ReceivedGameEndPacket;

        public Client(frmStartup Form)
        {
            m_StartupForm = Form;
            m_LocalIP = NetworkUtils.GetLocalIPAddress();
            m_HostIP = null;
            m_Connected = false;
            m_ID = 255;
            m_LobbyInfo = new LobbyInfo(m_LocalIP);
            InitialiseBroadcastReceiver();

            ReceivedGameInfoUpdatePacket += Client_ReceivedGameInfoUpdatePacket;
            ReceivedBroadcastPacket += Client_ReceivedBroadcastPacket;
            ReceivedRegisterPlayerPacket += Client_ReceivedRegisterPlayerPacket;
            ReceivedGameStartEndPacket += Client_ReceivedGameStartEndPacket;
            ReceivedGameEndPacket += Client_ReceivedGameEndPacket;

            ReceivedTrame += Network_ReceivedTrame;
        }

        public IPAddress LocalIP
        {
            get { return m_LocalIP; }
        }

        private void Network_ReceivedTrame(object sender, TrameReceivedEventArgs e)
        {
            // Recoit une trame et l'envoie à la bonne méthode (par le biais d'évenements)
            TrameReceivedEventHandler EventHandler;
            switch (e.Trame.Type)
            {
                default:
                    EventHandler = null;
                    break;
                case PacketType.GameInfoUpdatePacket:
                    EventHandler = ReceivedGameInfoUpdatePacket;
                    break;
                case PacketType.RegisterPlayerPacket:
                    EventHandler = ReceivedRegisterPlayerPacket;
                    break;
                case PacketType.GameStartEndPacket:
                    EventHandler = ReceivedGameStartEndPacket;
                    break;
                case PacketType.PlayerMovePacket:
                    EventHandler = ReceivedPlayerMovePacket;
                    break;
                case PacketType.PlayerPositionUpdatePacket:
                    EventHandler = ReceivedPlayerPositionUpdatePacket;
                    break;
                case PacketType.PlayerDiePacket:
                    EventHandler = ReceivedPlayerDiePacket;
                    break;
                case PacketType.BlockBreakPacket:
                    EventHandler = ReceivedBlockBreakPacket;
                    break;
                case PacketType.BlockPlacePacket:
                    EventHandler = ReceivedBlockPlacePacket;
                    break;
                case PacketType.BombPlacePacket:
                    EventHandler = ReceivedBombPlacePacket;
                    break;
                case PacketType.BombExplodePacket:
                    EventHandler = ReceivedBombExplodePacket;
                    break;
                case PacketType.PlayerPickupBombPacket:
                    EventHandler = ReceivedPlayerPickupBombPacket;
                    break;
                case PacketType.PlayerThrowBombPacket:
                    EventHandler = ReceivedPlayerThrowBombPacket;
                    break;
                case PacketType.PlayerKickBombPacket:
                    EventHandler = ReceivedPlayerKickBombPacket;
                    break;
                case PacketType.PlayerPickupBonusPacket:
                    EventHandler = ReceivedPlayerPickupBonusPacket;
                    break;
                case PacketType.BroadcastGamePacket:
                    EventHandler = ReceivedBroadcastPacket;
                    break;
                case PacketType.GameEndPacket:
                    EventHandler = ReceivedGameEndPacket;
                    break;
            }
            if (EventHandler != null)
                EventHandler(this, new TrameReceivedEventArgs(e.Trame, e.Socket, false));
        }

        /// <summary>
        /// La partie est terminée
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Client_ReceivedGameEndPacket(object sender, TrameReceivedEventArgs e)
        {
            if (!(e.Trame is GameEndTrame))
                return;
            GameEndTrame t = (GameEndTrame)e.Trame;
            m_TCPClient.Close();
            m_UDPClient.Close();
            m_UDPBroadcastReceiver.Close();
            MessageBox.Show("Partie terminée! " + m_LobbyInfo.NomJoueurs[t.ID] + " a gagné!!!");
            if (m_frmJeu != null)
            {
                // Fermeture du formulaire
                m_frmJeu.Invoke(new Action(() => { m_frmJeu.Close(); }));
            }
            // Fermeture de TOUT
            Environment.Exit(0);
        }

        /// <summary>
        /// La partie commence
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Client_ReceivedGameStartEndPacket(object sender, TrameReceivedEventArgs e)
        {
            if (!(e.Trame is GameStartEndPacket))
                return;
            GameStartEndPacket t = (GameStartEndPacket)e.Trame;
            m_LobbyInfo = t.LobbyInfo;
            m_StartupForm.LobbyInfo = m_LobbyInfo;
            m_StartupForm.UpdateGUI();
            if (t.StartEnd) // Inutile, finalement
            {
                // On crée le formulaire de jeu dans un nouveau thread
                Thread ThreadJeu = new Thread(new ParameterizedThreadStart(StartJeu));
                ThreadJeu.Start(t.Map);
            }
        }

        /// <summary>
        /// On crée le formulaire de jeu dans un autre thread
        /// </summary>
        /// <param name="map"></param>
        private void StartJeu(object map)
        {
            Maps.Map Map = (Maps.Map)map;
            m_frmJeu = new frmJeu(m_ID, Map, m_LobbyInfo, this);
            Application.Run(m_frmJeu);
        }

        /// <summary>
        /// On recoit une publicité d'une partie
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Client_ReceivedBroadcastPacket(object sender, TrameReceivedEventArgs e)
        {
            if (!(e.Trame is BroadcastTrame))
                return;
            BroadcastTrame t = (BroadcastTrame)e.Trame;
            if (t.Nom.Length == 0 || t.IP == IPAddress.Any || t.IP == IPAddress.None || t.IP == IPAddress.Broadcast)
                return;
            Partie p;
            if (t.Nom[0] == '-')
            {
                // Si la partie doit être enlevée
                p = new Partie(t.Nom.Substring(1, t.Nom.Length - 1), t.IP);
                m_StartupForm.RemoveGameFromListBox(p);
            }
            else
            {
                // Si la partie doit être ajoutée
                p = new Partie(t.Nom, t.IP);
                m_StartupForm.AddGameToListBox(p);
            }

        }

        /// <summary>
        /// On recoit de nouvelles informations sur la partie
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Client_ReceivedGameInfoUpdatePacket(object sender, TrameReceivedEventArgs e)
        {
            SendTCP(new ACKTrame(e.Trame.Type, m_LocalIP, true).ToByteArray());
            if (!(e.Trame is GameInfoUpdateTrame))
                return;
            GameInfoUpdateTrame t = (GameInfoUpdateTrame)e.Trame;
            // Mise à jour de tout
            m_LobbyInfo = t.LobbyInfo;
            m_StartupForm.LobbyInfo = t.LobbyInfo;
            m_StartupForm.UpdateGUI();
        }

        /// <summary>
        /// Rejoindre une partie
        /// </summary>
        /// <param name="p"></param>
        public void Join(Partie p)
        {
            if (m_TCPClient != null)
                return;
            JoinGameRequestTrame t = new JoinGameRequestTrame(m_LocalIP);
            InitialiseTCP(new IPEndPoint(p.Host, NetworkUtils.PORT));
            m_TimeSent = t.TimeSent;
            SendTCP(t.ToByteArray());
        }

        /// <summary>
        /// Le Server nous a bien identifié
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Client_ReceivedRegisterPlayerPacket(object sender, TrameReceivedEventArgs e)
        {
            if (!(e.Trame is RegisterPlayerTrame))
                return;
            RegisterPlayerTrame t = (RegisterPlayerTrame)e.Trame;
            if (t.IDJoueur < 0 || t.IDJoueur > 3)
                return;
            if (t.TimeSent.Ticks != m_TimeSent.Ticks)
                return;
            m_HostIP = e.Trame.IP;
            SendTCP(new ACKTrame(t.Type, m_LocalIP, true).ToByteArray());
            m_Connected = true;
            m_ID = t.IDJoueur;
            // Notre numéro de joueur
            Console.WriteLine("[CLIENT] I AM NOW REGISTERED AS " + m_ID.ToString());
            m_StartupForm.NumeroJoueur = m_ID;
            // Début du client UDP (connecté au serveur)
            InitialiseUDP(NetworkUtils.PORT + m_ID);
        }

        /// <summary>
        /// Remettre tout à sa valeur par défaut
        /// </summary>
        public void ResetLobby()
        {
            m_StartupForm.RemoveGameFromListBox(new Partie(m_LobbyInfo.NomPartie, m_LobbyInfo.Host));
            m_LobbyInfo = new LobbyInfo(m_LocalIP);
            m_StartupForm.LobbyInfo = m_LobbyInfo;
            m_StartupForm.NumeroJoueur = -1;
            m_StartupForm.UpdateGUI();
            CloseTCP();
            CloseUDP();
            m_TCPClient = null;
            m_UDPClient = null;
        }
        

    }
}
