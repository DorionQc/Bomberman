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
        private UdpClient m_UDPServer;
        private Thread m_UDPReceivingThread;
        private int[] m_tClientPort;
        private Thread m_BroadcastThread;
        private byte[] m_BroadcastTrame;
        private volatile bool m_Broadcasting;

        /// <summary>
        /// Commence le serveur UDP
        /// </summary>
        /// <returns></returns>
        public bool InitialiseUDP()
        {
            m_UDPServer = new UdpClient();
            m_tClientPort = new int[4];
            m_UDPServer.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            m_UDPServer.EnableBroadcast = true;
            m_UDPServer.ExclusiveAddressUse = false;
            m_UDPServer.Client.Bind(new IPEndPoint(m_HostIP, NetworkUtils.PORT));
            m_UDPReceivingThread = new Thread(new ThreadStart(ReceiveUDP));
            m_UDPReceivingThread.Start();
            return true;
        }

        /// <summary>
        /// Recoit, en boucle, sur UDP (broadcast)
        /// </summary>
        private void ReceiveUDP()
        {
            byte[] ReceivedData;
            IPEndPoint ReceivedFrom = new IPEndPoint(IPAddress.Any, NetworkUtils.PORT);
            AbsTrame Trame;
            bool Connected = true;
            while (Connected)
            {
                try
                {
                    ReceivedData = m_UDPServer.Receive(ref ReceivedFrom);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return;
                }
                while (ReceivedData != null && ReceivedData.Length != 0)
                {
                    if (PacketManager.Decode(ref ReceivedData, out Trame) && Trame.IsForHost)
                    {
                        if (ReceivedTrame != null)
                        {
                            //Console.WriteLine("[SERVER UDP] Received " + Trame.ToString());
                            ReceivedTrame(this, new TrameReceivedEventArgs(Trame, m_UDPServer.Client, false));
                        }
                    }
                    else
                        ReceivedData = null;
                }
            }
        }

        /// <summary>
        /// Envoie, en broadcast, sur UDP
        /// </summary>
        /// <param name="Trame"></param>
        private void SendToAllUDP(byte[] Trame)
        {
            if (Trame.Length > 1 && (Trame[1] & 128) == 0)
            {
                try
                {
                    m_UDPServer.Send(Trame, Trame.Length, new IPEndPoint(IPAddress.Broadcast, NetworkUtils.PORT));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return;
                }
            }
        }

        /// <summary>
        /// Commence la publicité
        /// </summary>
        /// <param name="Trame"></param>
        public void StartBroadcast(byte[] Trame)
        {
            if (Trame == null || m_UDPServer == null)
                return;
            m_BroadcastTrame = Trame;
            
            m_Broadcasting = true;
            m_BroadcastThread = new Thread(new ThreadStart(Broadcast));
            m_BroadcastThread.Start();
        }

        /// <summary>
        /// Arrete la publicité
        /// </summary>
        public void StopBroadcast()
        {
            m_Broadcasting = false;
        }

        /// <summary>
        /// Publie l'existence de la partie
        /// </summary>
        private void Broadcast()
        {
            while (m_Broadcasting)
            {
                //Console.WriteLine("Broadcasted");
                try
                {
                    m_UDPServer.Send(m_BroadcastTrame, m_BroadcastTrame.Length, new IPEndPoint(IPAddress.Broadcast, NetworkUtils.PORT));
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return;
                }
                Thread.Sleep(100);
            }
        }
    }
}
