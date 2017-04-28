using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

using System.Runtime.InteropServices;

namespace Jeu_Comm.Network
{
    public static class NetworkUtils
    {
        /// <summary>
        /// Port utilisé
        /// </summary>
        public const int PORT = 59635;
        /// <summary>
        /// Trouver la bonne adresse IP (favorise celle commençant par 192.168.0)
        /// </summary>
        /// <returns></returns>
        public static IPAddress GetLocalIPAddress()
        {
            int i = 0;
            IPAddress[] tAddresses = Dns.GetHostAddresses(Dns.GetHostName());
            List<IPAddress> lAddresses = new List<IPAddress>(tAddresses.Length);
            for (i = 0; i < tAddresses.Length; i++)
                if (tAddresses[i].AddressFamily == AddressFamily.InterNetwork)
                {
                    lAddresses.Add(tAddresses[i]);
                    Console.WriteLine("Found IP : " + tAddresses[i].ToString());
                }
            if (lAddresses.Count == 0)
                return IPAddress.None;
            i = 0;
            while (i < lAddresses.Count && !lAddresses[i].ToString().StartsWith("192.168.0"))
                i++;
            if (i == lAddresses.Count)
                return lAddresses[0];
            return lAddresses[i];
        }

        /// <summary>
        /// Calcule le checksum d'une trame
        /// </summary>
        /// <param name="Trame">La trame à calculer.</param>
        /// <returns>Un byte représentant le checksum</returns>
        public static byte Checksum(byte[] Trame, int ByteCount)
        {
            byte Total = 0;
            for (int i = 0; i < ByteCount; i++)
            {
                Total += Trame[i];
            }
            return Total;
        }

        /// <summary>
        /// Retourne une partie d'un tableau
        /// </summary>
        /// <param name="Source"></param>
        /// <param name="Index"></param>
        /// <param name="Length"></param>
        /// <returns></returns>
        public static byte[] SubArray(byte[] Source, int Index, int Length)
        {
            byte[] ret = new byte[Length];
            if (Index + Length > Source.Length || Index < 0 || Length < 0)
                return null;
            for (int i = 0; i < Length; i++)
            {
                ret[i] = Source[Index + i];
            }
            return ret;
        }
    }
}
