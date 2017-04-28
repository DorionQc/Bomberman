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
    partial class Client
    {

        private UdpClient m_UDPBroadcastReceiver;
        private Thread m_BroadcastReceivingThread;

        /// <summary>
        /// Crée un client UDP qui recoit sur un port static, en continue
        /// </summary>
        /// <returns></returns>
        public bool InitialiseBroadcastReceiver()
        {
            m_UDPBroadcastReceiver = new UdpClient();
            m_UDPBroadcastReceiver.EnableBroadcast = true;
            m_UDPBroadcastReceiver.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            m_UDPBroadcastReceiver.Client.Bind(new IPEndPoint(NetworkUtils.GetLocalIPAddress(), NetworkUtils.PORT));
            m_BroadcastReceivingThread = new Thread(new ThreadStart(ReceiveBroadcast));
            m_BroadcastReceivingThread.Start();
            return true;
        }

        /// <summary>
        /// Méthode de lecture, dans un autre thread
        /// </summary>
        private void ReceiveBroadcast()
        {
            byte[] ReceivedData;
            IPEndPoint ReceivedFrom = new IPEndPoint(IPAddress.Any, NetworkUtils.PORT);
            AbsTrame Trame;
            while (true)
            {
                try
                {
                    ReceivedData = m_UDPBroadcastReceiver.Receive(ref ReceivedFrom);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return;
                }
                while (ReceivedData != null && ReceivedData.Length != 0)
                {
                    if (PacketManager.Decode(ref ReceivedData, out Trame))
                    {
                        if (ReceivedTrame != null && Trame is BroadcastTrame)
                        {
                            ReceivedTrame(this, new TrameReceivedEventArgs(Trame, m_UDPBroadcastReceiver.Client, false));
                        }
                    }
                    else
                        ReceivedData = null;
                }
            }
        }

    }
}
