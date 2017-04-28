/************************
 * Samuel Goulet
 * Novembre 2016
 * Formulaire de jeu!
 ***********************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;

using Jeu_Comm.Maps;
using Jeu_Comm.Maps.Cases;
using Jeu_Comm.Entities;
using Jeu_Comm.Lobby;
using Jeu_Comm.Network;
using Jeu_Comm.Network.Trames;
using Jeu_Comm.CustomEventArgs;

namespace Jeu_Comm
{
    // Formulaire de jeu!
    public partial class frmJeu : Form
    {
        // Contient l'état du clavier et de la souris
        private KeyWrapper m_KeyWrapper;
        // Carte sur laquelle on joue!
        private Map m_Map;
        // Les différents joueurs sur la carte
        private Joueur[] m_tJoueur;
        // Thread de calcul
        private Thread m_ThreadLoop;
        // Structure contenant différentes informations bien utiles
        private LobbyInfo m_LobbyInfo;
        // Numéro du joueur local
        private int m_NumeroJoueur;

        // Indique si la partie est finie
        private bool m_Finished;
        private Client m_Client;
        
        public frmJeu(int NumeroJoueur, Map Map, LobbyInfo lb, Client Client)
        {
            InitializeComponent();

            // Les graphiques sont en double buffer!
            this.DoubleBuffered = true;

            m_Client = Client;
            m_LobbyInfo = lb;
            m_NumeroJoueur = NumeroJoueur;
            m_Map = Map;

            // Position des joueurs lorsqu'ils apparaissent
            int[,] DefaultLocations = new int[4, 2]
            {
                { Map.EntityPixelPerCase + 10, Map.EntityPixelPerCase + 10 },
                { Map.EntityPixelPerCase * (m_Map.NoCase - 1) - 10, Map.EntityPixelPerCase + 10 },
                { Map.EntityPixelPerCase * (m_Map.NoCase - 1) - 10, Map.EntityPixelPerCase * (m_Map.NoCase - 1) - 10 },
                { Map.EntityPixelPerCase + 10, Map.EntityPixelPerCase * (m_Map.NoCase - 1) - 10 }
            };

            m_tJoueur = new Joueur[4];
            int i = 0;
            // Si le joueur existe, on le crée!
            while (i < 4)
            {
                if (m_LobbyInfo.SpotsTaken[i])
                {
                    m_tJoueur[i] = new Joueur(DefaultLocations[i, 0], DefaultLocations[i, 1], m_Map, (byte)i);
                }
                i++;
            }

            // Assignation des événements
            m_tJoueur[m_NumeroJoueur].Moved += OnPlayerMoved;
            m_tJoueur[m_NumeroJoueur].DroppedBomb += OnPlayerDroppedBomb;
            m_tJoueur[m_NumeroJoueur].BombExploded += OnPlayerBombExploded;
            m_tJoueur[m_NumeroJoueur].PickedBomb += OnPlayerPickedBomb;
            m_tJoueur[m_NumeroJoueur].KickedBomb += OnPlayerKickedBomb;
            m_tJoueur[m_NumeroJoueur].Died += OnPlayerDied;
            m_tJoueur[m_NumeroJoueur].ShotBomb += OnPlayerShotBomb;
            m_tJoueur[m_NumeroJoueur].PickedBonus += OnPlayerPickedBonus;
            m_tJoueur[m_NumeroJoueur].BombBrokeBlocks += OnPlayerBombBrokeBlocks;
            m_tJoueur[m_NumeroJoueur].BombPlacedBonus += OnPlayerBombPlacedBonus;
            m_Client.ReceivedPlayerPositionUpdatePacket += Client_ReceivedPositionUpdatePacket;
            m_Client.ReceivedPlayerDiePacket += Client_ReceivedPlayerDiePacket;
            m_Client.ReceivedBombPlacePacket += Client_ReceivedBombPlacePacket;
            m_Client.ReceivedBombExplodePacket += Client_ReceivedBombExplodePacket;
            m_Client.ReceivedBlockBreakPacket += Client_ReceivedBlockBreakPacket;
            m_Client.ReceivedBlockPlacePacket += Client_ReceivedBlockPlacePacket;
            m_Client.ReceivedPlayerPickupBonusPacket += Client_ReceivedPlayerPickupBonusPacket;
            m_Client.ReceivedPlayerPickupBombPacket += Client_ReceivedPlayerPickupBombPacket;
            m_Client.ReceivedPlayerThrowBombPacket += Client_ReceivedPlayerThrowBombPacket;
            m_Client.ReceivedPlayerKickBombPacket += Client_ReceivedPlayerKickBombPacket;
            
            // Initialisation des instances de Singleton
            EntityManager.InitInstance(m_tJoueur, m_Map, m_NumeroJoueur);
            TextureManager.InitInstance(m_LobbyInfo.Colors);
            // Création du thread de calcul
            ThreadStart LoopThread = new ThreadStart(MainLoop);
            m_ThreadLoop = new Thread(LoopThread);
            m_ThreadLoop.Start();
        }

        /// <summary>
        /// Lorsqu'un joueur place une bombe
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPlayerBombPlacedBonus(object sender, CaseEventArgs e)
        {
            if (!(sender is Bomb))
                return;
            Bomb b = (Bomb)sender;
            if (!(b.Owner == m_tJoueur[m_NumeroJoueur]))
                return;
            if (!(e.Case is CaseBonus))
                return;
            CaseBonus c = (CaseBonus)e.Case;
            // On envoie la bombe pas TCP au Server
            //Console.WriteLine("Bonus was placed at " + e.Case.X + ", " + e.Case.Y + ", of type " + ((CaseBonus)e.Case).BonusType.ToString());
            m_Client.SendTCP(new BlockPlaceTrame(m_Client.LocalIP, c.BonusType, (byte)c.X, (byte)c.Y, (byte)m_NumeroJoueur, true).ToByteArray());
        }

        /// <summary>
        /// Lorsqu'une bombe brise des cases
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPlayerBombBrokeBlocks(object sender, MultiCaseEventArgs e)
        {
            if (!(sender is Bomb))
                return;
            Bomb b = (Bomb)sender;
            if (!(b.Owner == m_tJoueur[m_NumeroJoueur]))
                return;
            //Console.WriteLine("Bomb broke ");
            //foreach (AbsCase c in e.Cases)
            //    Console.WriteLine(c.ToString());
            Point[] p = new Point[e.Cases.Length];
            for (int i = 0; i < e.Cases.Length; i++)
                p[i] = new Point(e.Cases[i].X, e.Cases[i].Y);
            // On envoie un tableau de Points contenant les positions des cases brisées
            m_Client.SendTCP(new BlockBreakTrame(m_Client.LocalIP, (byte)m_NumeroJoueur, p, true).ToByteArray());
        }

        /// <summary>
        /// Lorsqu'une bombe explose
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPlayerBombExploded(object sender, CaseEventArgs e)
        {
            if (!(sender is Bomb))
                return;
            Bomb b = (Bomb)sender;
            if (!(b.Owner == m_tJoueur[m_NumeroJoueur]))
                return;
            //Console.WriteLine("Bomb exploded at " + e.Case.X + ", " + e.Case.Y);
            // On envoie un paquet pour dire que la bombe a explosé!
            m_Client.SendTCP(new BombExplodeTrame(m_Client.LocalIP, b.ID, (byte)(b.X / Map.EntityPixelPerCase), (byte)(b.Y / Map.EntityPixelPerCase), true).ToByteArray());

        }

        /// <summary>
        /// Lorsqu'un joueur obtient un bonus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPlayerPickedBonus(object sender, CaseEventArgs e)
        {
            if (!(sender is Joueur))
                return;
            Joueur j = (Joueur)sender;
            if (!(e.Case is CaseBonus))
                return;
            CaseBonus c = (CaseBonus)e.Case;
            if (j.PlayerID == m_NumeroJoueur)
            {
                //Console.WriteLine("Picked bonus at " + e.Case.X + ", " + e.Case.Y);
                // On envoie un paquet pour dire que le joueur a pris le bonus
                m_Client.SendTCP(new PlayerPickupBonusTrame(m_Client.LocalIP, (byte)m_NumeroJoueur, c.BonusType, (byte)c.X, (byte)c.Y, true).ToByteArray());
            }
        }

        /// <summary>
        /// Lorsqu'un joueur lance une bombe
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPlayerShotBomb(object sender, ShootBombEventArgs e)
        {
            if (!(sender is Joueur))
                return;
            Joueur j = (Joueur)sender;
            if (j.PlayerID == m_NumeroJoueur)
            {
                //Console.WriteLine("Shot bomb " + e.Side.ToString());
                // On envoie un paquet pour dire que le joueur a lancé une bombe, en spécifiant la direction
                m_Client.SendTCP(new PlayerThrowBombTrame(m_Client.LocalIP, (byte)m_NumeroJoueur, e.Side, e.Bomb.ID, true).ToByteArray());
            }
        }

        /// <summary>
        /// Lorsqu'un joueur meurt
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPlayerDied(object sender, CancellableEventArgs e)
        {
            if (!(sender is Joueur))
                return;
            Joueur j = (Joueur)sender;
            if (j.PlayerID == m_NumeroJoueur)
            {
                //Console.WriteLine("Died");
                // On envoie un avis de décès au Server
                m_Client.SendTCP(new PlayerDieTrame(m_Client.LocalIP, (byte)m_NumeroJoueur, true).ToByteArray());
            }
        }

        /// <summary>
        /// Lorsqu'un joueur prent une bombe dans ses mains
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPlayerPickedBomb(object sender, CaseEventArgs e)
        {
            if (!(sender is Joueur))
                return;
            Joueur j = (Joueur)sender;
            if (!(e.Case is CaseVide))
                return;
            CaseVide cv = (CaseVide)e.Case;
            if (cv.ContainsBomb == false)
                return;
            if (j.PlayerID == m_NumeroJoueur)
            {
                //Console.WriteLine("Picked bomb");
                // On en informe le Server
                m_Client.SendTCP(new PlayerPickupBombTrame(m_Client.LocalIP, (byte)m_NumeroJoueur, (byte)cv.X, (byte)cv.Y, true).ToByteArray());
            }
        }

        /// <summary>
        /// Lorsqu'un joueur botte une bombe
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPlayerKickedBomb(object sender, KickedBombEventArgs e)
        {
            if (!(sender is Joueur))
                return;
            Joueur j = (Joueur)sender;
            if (j.PlayerID == m_NumeroJoueur)
            {
                //Console.WriteLine("Kicked bomb");
                // On en informe le Server en spécifiant la direction
                m_Client.SendTCP(new PlayerKickBombTrame(m_Client.LocalIP, (byte)m_NumeroJoueur, e.Side, e.Bomb.ID, true).ToByteArray());
            }
        }

        /// <summary>
        /// Lorsqu'un joueur place une bombe
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPlayerDroppedBomb(object sender, CaseEventArgs e)
        {
            if (!(sender is Joueur))
                return;
            Joueur j = (Joueur)sender;
            if (!(e.Case is CaseVide))
                return;
            CaseVide c = (CaseVide)e.Case;
            if (c.ContainsBomb == false)
                return;
            if (j.PlayerID == m_NumeroJoueur)
            {
                //Console.WriteLine("Dropped bomb");
                // On envoie la position et l'ID de la bombe au Server
                m_Client.SendTCP(new BombPlaceTrame(m_Client.LocalIP, c.Bomb.ID, (byte)c.X, (byte)c.Y, c.Bomb.Owner.PlayerID, true).ToByteArray());
            }
        }

        /// <summary>
        /// Lorsque le joueur bouge
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPlayerMoved(object sender, CancellableEventArgs e)
        {
            if (!(sender is Joueur))
                return;
            Joueur j = (Joueur)sender;
            if (j.PlayerID == m_NumeroJoueur)
            {
                // On envoie, par UDP, la nouvelle position et la vélocité du joueur!
                m_Client.SendUDP(new PlayerMoveTrame(m_Client.LocalIP, j.PlayerID, j.X, j.Y, j.VelX, j.VelY, true).ToByteArray());
            }
        }

        /// <summary>
        /// Thread de calcul
        /// </summary>
        private void MainLoop()
        {

            Stopwatch sw = new Stopwatch();
            sw.Start();
            m_Finished = false;
            long DeltaTime = 0;
            while (!m_Finished)
            {
                while (sw.ElapsedMilliseconds < 8)
                    Thread.Sleep(0); // On s'assure qu'on prend environ 8 ms par itération
                DeltaTime = sw.ElapsedMilliseconds; // On garde ce temps en mémoire
                sw.Restart();
                // On exécute les calculs pour le clavier
                EntityManager.Instance.TickPlayer(m_NumeroJoueur, DeltaTime, m_KeyWrapper);
                m_KeyWrapper.State &= ~KeyState.Space; // L'espace n'est traité qu'une fois!
                // On exécute les calculs sur les entités
                EntityManager.Instance.TickEntities(DeltaTime);
                // On dit au formulaire qu'il doit se rafraichir
                Invalidate();
            }
        }

        private void frmJeu_Paint(object sender, PaintEventArgs e)
        {
            // Appelé aussi souvent que possible...
            // Dessin de la carte
            m_Map.Draw(e.Graphics, ClientRectangle);
            // Dessin des entités
            EntityManager.Instance.Draw(e.Graphics, ClientRectangle);
        }

        private void frmJeu_KeyDown(object sender, KeyEventArgs e)
        {
            // Ajouter la touche pesée à la structure KeyWrapper
            switch (e.KeyCode)
            {
                case Keys.Up:
                    m_KeyWrapper.State |= KeyState.Up;
                    break;
                case Keys.Left:
                    m_KeyWrapper.State |= KeyState.Left;
                    break;
                case Keys.Down:
                    m_KeyWrapper.State |= KeyState.Down;
                    break;
                case Keys.Right:
                    m_KeyWrapper.State |= KeyState.Right;
                    break;
                case Keys.Space:
                    m_KeyWrapper.State |= KeyState.Space;
                    break;
            }
        }

        private void frmJeu_KeyUp(object sender, KeyEventArgs e)
        {
            // Enlever la touche du clavier à la structure KeyWrapper
            switch (e.KeyCode)
            {
                case Keys.Up:
                    m_KeyWrapper.State &= ~KeyState.Up;
                    break;
                case Keys.Left:
                    m_KeyWrapper.State &= ~KeyState.Left;
                    break;
                case Keys.Down:
                    m_KeyWrapper.State &= ~KeyState.Down;
                    break;
                case Keys.Right:
                    m_KeyWrapper.State &= ~KeyState.Right;
                    break;
                case Keys.Space:
                    m_KeyWrapper.State &= ~KeyState.Space;
                    break;
            }
        }

        private void frmJeu_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            // Informer Windows que l'on prend en charge ces touches
            if (e.KeyCode == Keys.Down || e.KeyCode == Keys.Up || e.KeyCode == Keys.Left || e.KeyCode == Keys.Right || e.KeyCode == Keys.Escape
                || e.KeyCode == Keys.W || e.KeyCode == Keys.S || e.KeyCode == Keys.A || e.KeyCode == Keys.D || e.KeyCode == Keys.F)
                e.IsInputKey = true;
        }

        /// <summary>
        /// Lorsque le formulaire se ferme
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmJeu_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_Finished = true;
            if (m_ThreadLoop != null)
                m_ThreadLoop.Abort();
            // Peut planter à l'occasion....
            Environment.Exit(0);
        }

        /// <summary>
        /// Lorsque le Server nous envoie une trame de position.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Client_ReceivedPositionUpdatePacket(object sender, TrameReceivedEventArgs e)
        {
            if (!(e.Trame is PlayerPositionUpdateTrame))
                return;
            PlayerPositionUpdateTrame t = (PlayerPositionUpdateTrame)e.Trame;
            for (int i = 0; i < 4; i++)
            {
                if (m_tJoueur[i] != null && i != m_NumeroJoueur)
                {
                    // On met à jour la position de tout le monde (sauf nous même)
                    m_tJoueur[i].X = t.Positions.X[i];
                    m_tJoueur[i].Y = t.Positions.Y[i];
                    m_tJoueur[i].VelX = t.Positions.VelX[i];
                    m_tJoueur[i].VelY = t.Positions.VelY[i];
                    // Si le joueur tient une bombe dans ses mains, on déplace aussi la bombe
                    if (m_tJoueur[i].Bomb != null)
                    {
                        m_tJoueur[i].Bomb.X = m_tJoueur[i].X;
                        m_tJoueur[i].Bomb.Y = m_tJoueur[i].Y;
                    }
                }
            }
        }

        /// <summary>
        /// Lorsque le Server nous informe qu'un joueur est mort
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Client_ReceivedPlayerDiePacket(object sender, TrameReceivedEventArgs e)
        {
            if (!(e.Trame is PlayerDieTrame))
                return;
            if (((PlayerDieTrame)e.Trame).PlayerID > 3)
                return;
            // On le tue
            m_tJoueur[((PlayerDieTrame)e.Trame).PlayerID].Die(null, null);
        }

        /// <summary>
        /// Lorsque le Server nous informe qu'une bombe a été placée
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Client_ReceivedBombPlacePacket(object sender, TrameReceivedEventArgs e)
        {
            // Vérification que la trame est bonne
            if (!(e.Trame is BombPlaceTrame))
                return;
            BombPlaceTrame t = (BombPlaceTrame)e.Trame;
            // ... et qu'on a pas placé la bombe nous-même
            if (t.OwnerID > 3)
                return;
            if (t.OwnerID == m_NumeroJoueur)
                return;
            // ... et que le propriétaire de la bombe existe
            if (!m_LobbyInfo.SpotsTaken[t.OwnerID] || m_tJoueur[t.OwnerID] == null)
                return;
            // Si la bombe n'existe pas
            AbsEntity ent = EntityManager.Instance.EntityFromID(t.EntityID);
            if (ent == null || !(ent is Bomb))
                // On la crée
                m_tJoueur[t.OwnerID].DropBomb(t.X, t.Y, false, t.EntityID);
            else // Sinon
                // On la place quelque part
                ((Bomb)ent).SettleAt(t.X, t.Y);
        }

        /// <summary>
        /// Lorsque le Server nous informe qu'une bombe a explosé
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Client_ReceivedBombExplodePacket(object sender, TrameReceivedEventArgs e)
        {
            if (!(e.Trame is BombExplodeTrame))
                return;
            BombExplodeTrame t = (BombExplodeTrame)e.Trame;
            // On récupère la bombe (qui devrait déjà exister)
            AbsEntity ent = EntityManager.Instance.EntityFromID(t.ID);
            if (ent == null || !(ent is Bomb))
                return;
            Bomb b = (Bomb)ent;
            if (b.Owner.PlayerID == m_NumeroJoueur)
                return;
            // ... et on l'explose!
            b.Update();
        }

        /// <summary>
        /// Lorsque le Server nous informe qu'une ou des cases ont été brisées
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Client_ReceivedBlockBreakPacket(object sender, TrameReceivedEventArgs e)
        {
            if (!(e.Trame is BlockBreakTrame))
                return;
            BlockBreakTrame t = (BlockBreakTrame)e.Trame;
            if (t.ID == m_NumeroJoueur)
                return;
            for (int i = 0; i < t.Points.Length; i++)
            {
                // On détruit ces cases!
                Bomb.Ignite(t.Points[i].X, t.Points[i].Y, m_Map, 600, null, true);
            }
        }

        /// <summary>
        /// Lorsque le Server nous informe que des cases ont été placées (bonus)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Client_ReceivedBlockPlacePacket(object sender, TrameReceivedEventArgs e)
        {
            if (!(e.Trame is BlockPlaceTrame))
                return;
            BlockPlaceTrame t = (BlockPlaceTrame)e.Trame;
            if (t.ID == m_NumeroJoueur)
                return;
            // On place ces bonus!
            m_Map[t.X, t.Y] = new CaseBonus(t.X, t.Y, m_Map, t.BonusType);
        }

        /// <summary>
        /// Lorsque le Server nous informe qu'un joueur a pris un bonus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Client_ReceivedPlayerPickupBonusPacket(object sender, TrameReceivedEventArgs e)
        {
            // On vérifie quelques affaires
            if (!(e.Trame is PlayerPickupBonusTrame))
                return;
            PlayerPickupBonusTrame t = (PlayerPickupBonusTrame)e.Trame;
            if (t.ID == m_NumeroJoueur)
                return;
            if (t.ID > 3)
                return;
            if (!m_LobbyInfo.SpotsTaken[t.ID] || m_tJoueur[t.ID] == null)
                return;
            if (t.X > m_Map.NoCase || t.Y > m_Map.NoCase)
                return;
            AbsCase c = m_Map[t.X, t.Y];
            if (!(c is CaseBonus))
                return;
            // ... et on donne ce boni au joueur!
            m_tJoueur[t.ID].PickBonus((CaseBonus)c);
        }

        /// <summary>
        /// Lorsque le Server nous informe qu'un joueur a pris une bombe
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Client_ReceivedPlayerPickupBombPacket(object sender, TrameReceivedEventArgs e)
        {
            if (!(e.Trame is PlayerPickupBombTrame))
                return;
            PlayerPickupBombTrame t = (PlayerPickupBombTrame)e.Trame;
            if (t.ID == m_NumeroJoueur)
                return;
            if (t.ID > 3)
                return;
            if (!m_LobbyInfo.SpotsTaken[t.ID] || m_tJoueur[t.ID] == null)
                return;
            if (t.X > m_Map.NoCase || t.Y > m_Map.NoCase)
                return;
            // On fait que le joueur prenne la bombe à une telle position
            m_tJoueur[t.ID].PickupBomb(t.X, t.Y);
        }

        /// <summary>
        /// Lorsque le Server nous informe que quelqu'un a lancé une bombe
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Client_ReceivedPlayerThrowBombPacket(object sender, TrameReceivedEventArgs e)
        {
            if (!(e.Trame is PlayerThrowBombTrame))
                return;
            PlayerThrowBombTrame t = (PlayerThrowBombTrame)e.Trame;
            if (t.ID == m_NumeroJoueur)
                return;
            if (t.ID > 3)
                return;
            if (!m_LobbyInfo.SpotsTaken[t.ID] || m_tJoueur[t.ID] == null)
                return;
            // On fait que le joueur lance la bombe du côté spécifié
            m_tJoueur[t.ID].ShootBomb(t.Side);
        }

        /// <summary>
        /// Lorsque le Server nous informe qu'un joueur a botté une bombe
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Client_ReceivedPlayerKickBombPacket(object sender, TrameReceivedEventArgs e)
        {
            if (!(e.Trame is PlayerKickBombTrame))
                return;
            PlayerKickBombTrame t = (PlayerKickBombTrame)e.Trame;
            if (t.ID == m_NumeroJoueur)
                return;
            if (t.ID > 3)
                return;
            if (!m_LobbyInfo.SpotsTaken[t.ID] || m_tJoueur[t.ID] == null)
                return;
            // On vérifie l'existence de la bombe
            AbsEntity ent = EntityManager.Instance.EntityFromID(t.BombID);
            if (ent == null || !(ent is Bomb))
                return;
            Bomb b = (Bomb)ent;
            if (b.Owner.PlayerID == m_NumeroJoueur)
                return;
            // et on la botte
            b.Kick(t.Side);
        }
    }
}
