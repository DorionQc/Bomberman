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

namespace Jeu_Comm.Network
{
    partial class Server
    {
        private TcpListener m_TCPListener;
        private List<TcpClient> m_lClients;
        private List<Thread> m_lThreadSocket;
        private Thread m_ListeningThread;

        /// <summary>
        /// Commence le listener
        /// </summary>
        /// <returns></returns>
        public bool InitialiseListener()
        {
            m_lClients = new List<TcpClient>();
            m_lThreadSocket = new List<Thread>();
            m_TCPListener = new TcpListener(NetworkUtils.GetLocalIPAddress(), NetworkUtils.PORT);
            m_TCPListener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            m_TCPListener.Start();
            m_ListeningThread = new Thread(new ThreadStart(Listen));
            m_ListeningThread.Start();

            return true;
        }

        /// <summary>
        /// Attend la connection d'autres clients
        /// </summary>
        private void Listen()
        {
            TcpClient ReceivedClient;
            Thread TalkerThread;

            while (true)
            {
                try
                {
                    ReceivedClient = m_TCPListener.AcceptTcpClient();
                    ReceivedClient.SendBufferSize = 2048;
                    ReceivedClient.ReceiveBufferSize = 2048;
                }
                catch (Exception ex)
                {
                    Console.Write(ex.Message);
                    return;
                }
                if (m_lClients.Count < 4)
                {
                    // Ajoute le client reçu, lui donne un thread
                    Console.WriteLine("Received connection as | " + ReceivedClient.Client.LocalEndPoint.ToString() + 
                        ", from " + ReceivedClient.Client.RemoteEndPoint.ToString());
                    m_lClients.Add(ReceivedClient);
                    TalkerThread = new Thread(new ParameterizedThreadStart(ReceiveTCP));
                    m_lThreadSocket.Add(TalkerThread);
                    TalkerThread.Start(ReceivedClient);
                }
            }
        }

        /// <summary>
        /// Envoie une trame à un client spécifique
        /// </summary>
        /// <param name="Trame"></param>
        /// <param name="SendingSocket"></param>
        private void SendTCP(byte[] Trame, Socket SendingSocket)
        {
            Console.WriteLine("[SERVER] Sent " + ((PacketType)Trame[1]).ToString() + " to " + SendingSocket.RemoteEndPoint.ToString());
            SendingSocket.Send(Trame);
        }

        /// <summary>
        /// Threads qui recoivent les infos des clients
        /// </summary>
        /// <param name="TCPClient"></param>
        private void ReceiveTCP(object TCPClient)
        {
            TcpClient tcp = (TcpClient)TCPClient;
            byte[] buffer = new byte[2048];
            byte[] ReceivedData;
            AbsTrame Trame;
            int Length = 0;
            while (m_Connected)
            {
                try {
                    Length = tcp.Client.Receive(buffer);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    KickPlayer(tcp);
                    return;
                }

                ReceivedData = buffer.Take(Length).ToArray();
                while (ReceivedData != null && ReceivedData.Length != 0)
                {
                    if (PacketManager.Decode(ref ReceivedData, out Trame))
                    {
                        if (ReceivedTrame != null)
                        {
                            Console.WriteLine("[SERVER] Received " + Trame.ToString() + " on socket " + tcp.Client.RemoteEndPoint.ToString());
                            ReceivedTrame(this, new TrameReceivedEventArgs(Trame, tcp.Client, false));
                        }
                    }
                    else
                    {
                        ReceivedData = null;
                    }
                }
            }
        }

        /// <summary>
        /// Envoie une trame à tous
        /// </summary>
        /// <param name="Trame"></param>
        private void SendToAll(AbsTrame Trame)
        {
            byte[] BytesToSend = Trame.ToByteArray();
            for (int i = 0; i < 4; i++)
            {
                if (m_ConnectionInfos[i].Socket != null && m_ConnectionInfos[i].Socket.Connected)
                    try
                    {
                        Console.WriteLine("[SERVER] Sent " + Trame.ToString() + " to " + m_ConnectionInfos[i].Socket.RemoteEndPoint.ToString());
                        m_ConnectionInfos[i].Socket.Send(BytesToSend);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
            }
        }
    }



}
