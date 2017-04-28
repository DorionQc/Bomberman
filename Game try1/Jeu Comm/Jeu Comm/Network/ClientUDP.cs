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
    partial class Client
    {

        private UdpClient m_UDPClient;
        private Thread m_UDPReceivingThread;

        /// <summary>
        /// Initialise le client UDP
        /// </summary>
        /// <param name="Port"></param>
        /// <returns></returns>
        public bool InitialiseUDP(int Port)
        {
            if (Port < NetworkUtils.PORT || Port > NetworkUtils.PORT + 4)
                return false;
            m_UDPClient = new UdpClient();
            m_UDPClient.EnableBroadcast = true;
            m_UDPClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            m_UDPClient.ExclusiveAddressUse = false;
            m_UDPClient.Client.Bind(new IPEndPoint(m_LocalIP, NetworkUtils.PORT));
            m_UDPReceivingThread = new Thread(new ThreadStart(ReceiveUDP));
            m_UDPReceivingThread.Start();
            return true;
        }

        /// <summary>
        /// Envoie par UDP
        /// </summary>
        /// <param name="Trame"></param>
        public void SendUDP(byte[] Trame)
        {
            if (m_UDPClient == null || m_Connected == false)
                return;
            try
            {
                m_UDPClient.Send(Trame, Trame.Length, new IPEndPoint(IPAddress.Broadcast, NetworkUtils.PORT));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
                //Console.WriteLine("[CLIENT] Sent " + ((PacketType)(Trame[1] & 127)).ToString() + " to " + m_HostIP.ToString() + " on port " + NetworkUtils.PORT.ToString());
        }

        /// <summary>
        /// Ferme le client UDP
        /// </summary>
        public void CloseUDP()
        {
            if (m_UDPClient != null)
            {
                try
                {
                    m_UDPClient.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        /// <summary>
        /// Recoit, en boucle, par UDP
        /// </summary>
        public void ReceiveUDP()
        {
            byte[] ReceivedData;
            IPEndPoint ReceivedFrom = new IPEndPoint(IPAddress.Any, NetworkUtils.PORT);
            AbsTrame Trame;
            while (m_Connected)
            {
                try
                {
                    ReceivedData = m_UDPClient.Receive(ref ReceivedFrom);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return;
                }
                while (ReceivedData != null && ReceivedData.Length != 0)
                {
                    if (PacketManager.Decode(ref ReceivedData, out Trame) && Trame.IsForHost == false)
                    {
                        if (ReceivedTrame != null)
                        {
                            //Console.WriteLine("[CLIENT UDP] Received " + Trame.ToString());
                            ReceivedTrame(this, new TrameReceivedEventArgs(Trame, m_UDPClient.Client, false));
                        }
                    }
                    else
                        ReceivedData = null;
                }
                ReceivedFrom.Address = IPAddress.Any;
                ReceivedFrom.Port = NetworkUtils.PORT;
            }
        }

    }
}
