using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.IO;

using Jeu_Comm.Network;
using Jeu_Comm.Network.Trames;
using Jeu_Comm.CustomEventArgs;
using Jeu_Comm.Lobby;

namespace Jeu_Comm.Network
{
    // TCP
    partial class Client
    {

        private TcpClient m_TCPClient;
        private Thread m_TCPReceivingThread;

        /// <summary>
        /// Initialise le client TCP
        /// </summary>
        /// <param name="Host"></param>
        /// <returns></returns>
        public bool InitialiseTCP(IPEndPoint Host)
        {
            m_TCPClient = new TcpClient();
            m_TCPClient.ExclusiveAddressUse = false;
            m_TCPClient.ReceiveBufferSize = 2048;
            m_TCPClient.SendBufferSize = 2048;
            m_TCPClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            try
            {
                // Essaie de se connecter
                m_TCPClient.Connect(Host);
                m_Connected = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                m_Connected = false;
                return false;
            }

            m_TCPReceivingThread = new Thread(new ThreadStart(ReceiveTCP));
            m_TCPReceivingThread.Start();
            return true;
            
        }

        /// <summary>
        /// Ferme le client TCP
        /// </summary>
        public void CloseTCP()
        {
            if (m_TCPClient != null)
            {
                try
                {
                    m_TCPClient.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            m_Connected = false;
        }

        /// <summary>
        /// Envoie une trame par TCP au serveur
        /// </summary>
        /// <param name="Trame"></param>
        public void SendTCP(byte[] Trame)
        {
            if (m_TCPClient == null || !m_TCPClient.Connected || m_Connected == false)
                return;
            try
            {
                m_TCPClient.Client.Send(Trame);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                
            }


        }

        /// <summary>
        /// Recoit sur le réseau, en boucle
        /// </summary>
        private void ReceiveTCP()
        {
            byte[] buffer = new byte[2048];
            byte[] ReceivedData;
            AbsTrame Trame;
            int Length = 0;
            while (m_Connected)
            {
                try
                {
                    Length = m_TCPClient.Client.Receive(buffer);
                }
                catch (Exception ex)
                {
                    ResetLobby();
                    Console.WriteLine(ex.Message);
                    return;
                    
                }
                ReceivedData = buffer.Take(Length).ToArray();
                while (ReceivedData != null && ReceivedData.Length != 0)
                {
                    // Décoder la trame
                    if (PacketManager.Decode(ref ReceivedData, out Trame))
                    {
                        if (ReceivedTrame != null)
                        {
                            //Console.WriteLine("[CLIENT] Received " + Trame.ToString() + " on socket " + m_TCPClient.Client.LocalEndPoint.ToString());
                            ReceivedTrame(this, new TrameReceivedEventArgs(Trame, m_TCPClient.Client, false));
                        }

                    }
                    else
                        ReceivedData = null;
                }
            }
        }
    }
}
