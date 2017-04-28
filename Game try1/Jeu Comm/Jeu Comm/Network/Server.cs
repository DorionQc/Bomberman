using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Drawing;

using Jeu_Comm.Network;
using Jeu_Comm.Network.Trames;
using Jeu_Comm.CustomEventArgs;
using Jeu_Comm.Lobby;
using Jeu_Comm.Maps;
using Jeu_Comm.Entities;

namespace Jeu_Comm.Network
{
    /// <summary>
    /// Classe principale du Server
    /// </summary>
    public partial class Server
    {
        private IPAddress m_HostIP;
        private LobbyInfo m_LobbyInfo;
        private ConnectionInfo[] m_ConnectionInfos;
        private Map m_Map;

        private PlayerPositions m_Positions;

        private Thread m_ThreadSendPositions;
        private bool[] m_tJoueursMort;
        private bool m_SendingPositions;
        private bool m_Connected;

        private event TrameReceivedEventHandler ReceivedTrame;

        private event TrameReceivedEventHandler ReceivedJoinGameRequestPacket;
        private event TrameReceivedEventHandler ReceivedGameInfoUpdatePacket;
        private event TrameReceivedEventHandler ReceivedPlayerMovePacket;
        private event TrameReceivedEventHandler ReceivedPlayerDiePacket;
        private event TrameReceivedEventHandler ReceivedBlockBreakPacket;
        private event TrameReceivedEventHandler ReceivedBlockPlacePacket;
        private event TrameReceivedEventHandler ReceivedBombPlacePacket;
        private event TrameReceivedEventHandler ReceivedBombExplodePacket;
        private event TrameReceivedEventHandler ReceivedPlayerPickupBombPacket;
        private event TrameReceivedEventHandler ReceivedPlayerThrowBombPacket;
        private event TrameReceivedEventHandler ReceivedPlayerKickBombPacket;
        private event TrameReceivedEventHandler ReceivedPlayerPickupBonusPacket;
        private event TrameReceivedEventHandler ReceivedACK;

        private readonly Color[] DefaultColors = new Color[]
            {
                Color.Red,
                Color.Blue,
                Color.Green,
                Color.Yellow
            };

        public Server(string Nom, frmStartup Form)
        {
            m_HostIP = NetworkUtils.GetLocalIPAddress();
            m_LobbyInfo = new LobbyInfo(m_HostIP);
            m_LobbyInfo.NomPartie = Nom;
            m_ConnectionInfos = new ConnectionInfo[4];
            m_tJoueursMort = new bool[4];
            m_Connected = true;

            InitialiseListener();
            InitialiseUDP();
            // Commence le broadcast pour publier l'existence d'une partie
            StartBroadcast(new BroadcastTrame(m_LobbyInfo.NomPartie, m_HostIP, false).ToByteArray());

            this.ReceivedJoinGameRequestPacket += Server_ReceivedJoinGameRequestPacket;
            this.ReceivedGameInfoUpdatePacket += Server_ReceivedGameInfoUpdatePacket;
            this.ReceivedPlayerMovePacket += Server_ReceivedPlayerMovePacket;
            this.ReceivedPlayerDiePacket += Server_ReceivedPlayerDiePacket;
            this.ReceivedBombPlacePacket += Server_ReceivedBombPlacePacket;
            this.ReceivedBombExplodePacket += Server_ReceivedBombExplodePacket;
            this.ReceivedBlockBreakPacket += Server_ReceivedBlockBreakPacket;
            this.ReceivedBlockPlacePacket += Server_ReceivedBlockPlacePacket;
            this.ReceivedPlayerPickupBonusPacket += Server_ReceivedPlayerPickupBonusPacket;
            this.ReceivedPlayerPickupBombPacket += Server_ReceivedPlayerPickupBombPacket;
            this.ReceivedPlayerThrowBombPacket += Server_ReceivedPlayerThrowBombPacket;
            this.ReceivedPlayerKickBombPacket += Server_ReceivedPlayerKickBombPacket;

            ReceivedTrame += Network_ReceivedTrame;

            Form.AddGameToListBox(new Partie(Nom, m_HostIP));
        }

        /**************
         Réception de données
         ****************/

        private void Server_ReceivedPlayerKickBombPacket(object sender, TrameReceivedEventArgs e)
        {
            if (!(e.Trame is PlayerKickBombTrame))
                return;
            PlayerKickBombTrame t = (PlayerKickBombTrame)e.Trame;
            SendToAll(t);
        }

        private void Server_ReceivedPlayerThrowBombPacket(object sender, TrameReceivedEventArgs e)
        {
            if (!(e.Trame is PlayerThrowBombTrame))
                return;
            PlayerThrowBombTrame t = (PlayerThrowBombTrame)e.Trame;
            SendToAll(t);
        }

        private void Server_ReceivedPlayerPickupBombPacket(object sender, TrameReceivedEventArgs e)
        {
            if (!(e.Trame is PlayerPickupBombTrame))
                return;
            PlayerPickupBombTrame t = (PlayerPickupBombTrame)e.Trame;
            SendToAll(t);
        }

        private void Server_ReceivedPlayerPickupBonusPacket(object sender, TrameReceivedEventArgs e)
        {
            if (!(e.Trame is PlayerPickupBonusTrame))
                return;
            PlayerPickupBonusTrame t = (PlayerPickupBonusTrame)e.Trame;
            SendToAll(t);
        }

        private void Server_ReceivedBlockPlacePacket(object sender, TrameReceivedEventArgs e)
        {
            if (!(e.Trame is BlockPlaceTrame))
                return;
            BlockPlaceTrame t = (BlockPlaceTrame)e.Trame;
            SendToAll(t);
        }

        private void Server_ReceivedBlockBreakPacket(object sender, TrameReceivedEventArgs e)
        {
            if (!(e.Trame is BlockBreakTrame))
                return;
            BlockBreakTrame t = (BlockBreakTrame)e.Trame;
            SendToAll(t);
        }

        private void Server_ReceivedBombExplodePacket(object sender, TrameReceivedEventArgs e)
        {
            if (!(e.Trame is BombExplodeTrame))
                return;
            BombExplodeTrame t = (BombExplodeTrame)e.Trame;
            SendToAll(t);
        }

        private void Server_ReceivedBombPlacePacket(object sender, TrameReceivedEventArgs e)
        {
            if (!(e.Trame is BombPlaceTrame))
                return;
            BombPlaceTrame t = (BombPlaceTrame)e.Trame;
            SendToAll(t);
        }

        private void Server_ReceivedPlayerDiePacket(object sender, TrameReceivedEventArgs e)
        {
            if (!(e.Trame is PlayerDieTrame))
                return;
            PlayerDieTrame t = (PlayerDieTrame)e.Trame;
            m_tJoueursMort[t.PlayerID] = true;
            int JoueursVivant = 0;
            for (int i = 0; i < 4; i++)
            {
                if (m_LobbyInfo.SpotsTaken[i])
                    if (m_tJoueursMort[i] == false)
                        JoueursVivant++;
            }
            if (JoueursVivant <= 1)
            {
                // Lorsqu'il ne reste plus qu'un (ou zéro) joueur
                int i = 0;
                while (i < 4 && (m_tJoueursMort[i] == true || m_LobbyInfo.SpotsTaken[i] == false))
                    i++;
                Console.WriteLine("Fin de partie!");
                if (i != 4)
                {
                    SendToAll(new GameEndTrame(m_HostIP, (byte)i, false));
                }
                else
                {
                    SendToAll(new GameEndTrame(m_HostIP, 0, false));
                }
                m_Broadcasting = false;
                m_SendingPositions = false;
                m_Connected = false;
                foreach (TcpClient c in m_lClients)
                    c.Close();
                m_TCPListener.Stop();
                m_UDPServer.Close();
            }
            else
                SendToAll(t);
        }


        private void Server_ReceivedPlayerMovePacket(object sender, TrameReceivedEventArgs e)
        {
            if (!(e.Trame is PlayerMoveTrame))
                return;
            PlayerMoveTrame t = (PlayerMoveTrame)e.Trame;
            m_Positions.Update(t.ID, t.X, t.Y, t.VelX, t.VelY);
        }

        /// <summary>
        /// Envoie, en boucle, la position des joueurs
        /// </summary>
        private void SendPosition()
        {
            while (m_SendingPositions)
            {
                Thread.Sleep(20);
                SendToAllUDP(new PlayerPositionUpdateTrame(m_HostIP, m_Positions, false).ToByteArray());
            }
        }

        private void Server_ReceivedGameInfoUpdatePacket(object sender, TrameReceivedEventArgs e)
        {
            if (!(e.Trame is GameInfoUpdateTrame))
                return;
            m_LobbyInfo = ((GameInfoUpdateTrame)e.Trame).LobbyInfo;
            SendToAll(new GameInfoUpdateTrame(m_HostIP, m_LobbyInfo, 255, false));
            int i = 0, Count = 0;
            while (i < 4)
            {
                if (m_LobbyInfo.SpotsTaken[i])
                    if (m_LobbyInfo.PlayerReady[i])
                        Count++;
                i++;
            }
            // Mettre les informations stockée en mémoire
            if (Count == m_lClients.Count)
            {
                m_Map = new Map(21);
                m_Positions = new PlayerPositions();
                int[,] DefaultLocations = new int[4, 2]
                {
                    { Map.EntityPixelPerCase + 10, Map.EntityPixelPerCase + 10 },
                    { Map.EntityPixelPerCase * (m_Map.NoCase - 1) - 10, Map.EntityPixelPerCase + 10 },
                    { Map.EntityPixelPerCase * (m_Map.NoCase - 1) - 10, Map.EntityPixelPerCase * (m_Map.NoCase - 1) - 10 },
                    { Map.EntityPixelPerCase + 10, Map.EntityPixelPerCase * (m_Map.NoCase - 1) - 10 }
                };
                m_Positions.X = new int[4] { DefaultLocations[0, 0], DefaultLocations[1, 0], DefaultLocations[2, 0], DefaultLocations[3, 0] };
                m_Positions.Y = new int[4] { DefaultLocations[0, 1], DefaultLocations[1, 1], DefaultLocations[2, 1], DefaultLocations[3, 1] };
                m_Positions.VelX = new float[4];
                m_Positions.VelY = new float[4];
                m_SendingPositions = true;
                m_ThreadSendPositions = new Thread(new ThreadStart(SendPosition));
                m_ThreadSendPositions.Start();
                SendToAll(new GameStartEndPacket(m_HostIP, m_LobbyInfo, true, false, m_Map)); // Start game
            }
        }

        /// <summary>
        /// Arrête le serveur
        /// </summary>
        public void Stop()
        {
            m_Broadcasting = false;
            m_TCPListener.Stop();
            m_UDPServer.Close();

           
        }

        /// <summary>
        /// Ajoute un joueur à la partie
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Server_ReceivedJoinGameRequestPacket(object sender, TrameReceivedEventArgs e)
        {
            SendTCP(new ACKTrame(e.Trame.Type, m_HostIP, false).ToByteArray(), e.Socket);
            // Lui assigne un ID de joueur
            byte ID = m_LobbyInfo.GetFirstAvailableSpot();
            if (!(e.Trame is JoinGameRequestTrame))
                return;
            if (ID == 255)
                return;
            
            if (!m_LobbyInfo.AddPlayer("Joueur " + (ID + 1).ToString(), ID, e.Trame.IP, DefaultColors[ID]))
                return;
            m_ConnectionInfos[ID] = new ConnectionInfo(e.Socket, ID);
            AbsTrame t = new RegisterPlayerTrame(((JoinGameRequestTrame)e.Trame).TimeSent, m_HostIP, ID, false);
            SendTCP(t.ToByteArray(), e.Socket);
            t = new GameInfoUpdateTrame(m_HostIP, m_LobbyInfo, 255, false);
            SendToAll(t);
        }

        /// <summary>
        /// Envoie une trame à la bonne méthode pour être traitée
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Network_ReceivedTrame(object sender, TrameReceivedEventArgs e)
        {
            TrameReceivedEventHandler EventHandler;
            switch (e.Trame.Type)
            {
                default:
                    EventHandler = null;
                    break;
                case PacketType.JoinGameRequestPacket:
                    EventHandler = ReceivedJoinGameRequestPacket;
                    break;
                case PacketType.GameInfoUpdatePacket:
                    EventHandler = ReceivedGameInfoUpdatePacket;
                    break;
                case PacketType.PlayerMovePacket:
                    EventHandler = ReceivedPlayerMovePacket;
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
                case PacketType.ACKPacket:
                    EventHandler = ReceivedACK;
                    break;
            }
            if (EventHandler != null)
                EventHandler(this, new TrameReceivedEventArgs(e.Trame, e.Socket, false));
        }

        /// <summary>
        /// Sort un joueur de la partie (en cas de déconnection accidentelle)
        /// </summary>
        /// <param name="Client"></param>
        public void KickPlayer(TcpClient Client)
        {
            int NumeroJoueur = 0;
            while (NumeroJoueur < 4 && m_ConnectionInfos[NumeroJoueur].Socket != Client.Client)
                NumeroJoueur++;
            if (NumeroJoueur == 4)
                return;
            m_LobbyInfo.IPJoueurs[NumeroJoueur] = IPAddress.None;
            m_LobbyInfo.Colors[NumeroJoueur] = Color.White;
            m_LobbyInfo.NomJoueurs[NumeroJoueur] = "";
            m_LobbyInfo.PlayerReady[NumeroJoueur] = false;
            m_LobbyInfo.SpotsTaken[NumeroJoueur] = false;
            m_lClients.Remove(Client);
            GameInfoUpdateTrame t = new GameInfoUpdateTrame(m_HostIP, m_LobbyInfo, 255, false);
            SendToAll(t);
        }
    }
}
