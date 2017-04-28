using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Jeu_Comm.Network.Trames
{
    public class BroadcastTrame : AbsTrame
    {
        
        private string m_Nom;

        public BroadcastTrame(string NomDePartie, IPAddress IP, bool ForHost) : base(PacketType.BroadcastGamePacket, IP, ForHost)
        {
            m_Nom = NomDePartie;
        }

        public string Nom
        {
            get { return m_Nom; }
            set { m_Nom = value; }
        }

        // Format = STX | PacketType | LocalIP | (length)Nom | Checksum | ETX

        public static bool Decode(ref byte[] ByteData, out AbsTrame Trame)
        {
            Trame = null;
            if (ByteData.Length < 8)
                return false;
            if (NetworkUtils.Checksum(ByteData, 6) != ByteData[ByteData[6] + 7])
                return false;
            IPAddress IP = DecodeIP(ByteData);
            bool ForHost = (ByteData[1] & 128) == 1;
            string Nom = Encoding.Default.GetString(ByteData, 7, ByteData[6]);
            Trame = new BroadcastTrame(Nom, IP, ForHost);
            ByteData = ByteData.Skip(9 + ByteData[6]).ToArray();
            return true;
        }

        public override byte[] ToByteArray()
        {
            byte[] bytesNom = Encoding.Default.GetBytes(m_Nom);
            byte[] ret = new byte[9 + bytesNom.Length];
            ret[0] = 0x02;
            ret[1] = (byte)m_Type;
            if (IsForHost)
                ret[1] |= 128;
            m_IP.GetAddressBytes().CopyTo(ret, 2);
            ret[6] = (byte)bytesNom.Length;
            bytesNom.CopyTo(ret, 7);
            ret[ret.Length - 2] = NetworkUtils.Checksum(ret, 6);
            ret[ret.Length - 1] = 0x03;
            return ret;
        }

        public override string ToString()
        {
            return base.ToString() + m_Nom;
        }
    }
}
