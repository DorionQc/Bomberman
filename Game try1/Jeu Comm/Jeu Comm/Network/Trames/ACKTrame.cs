using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

using Jeu_Comm.Network;

namespace Jeu_Comm.Network.Trames
{
    public class ACKTrame : AbsTrame
    {

        private PacketType m_TrameReceivedType;

        public ACKTrame(PacketType TrameReceivedType, IPAddress SenderIP, bool ForHost) : base(PacketType.ACKPacket, SenderIP, ForHost)
        {
            m_TrameReceivedType = TrameReceivedType;
        }

        public PacketType ReceivedPacketType
        {
            get { return m_TrameReceivedType; }
            set { m_TrameReceivedType = value; }
        }

        // Format = STX | PacketType | LocalIP | ReceivedPacketType | Checksum | ETX

        public static bool Decode(ref byte[] ByteData, out AbsTrame Trame)
        {
            Trame = null;
            if (ByteData.Length < 9)
                return false;
            if (NetworkUtils.Checksum(ByteData, 7) != ByteData[7])
                return false;
            IPAddress IP = DecodeIP(ByteData);
            PacketType Type = (PacketType)(ByteData[6] & 127);
            bool ForHost = (ByteData[1] & 128) == 1;
            Trame = new ACKTrame(Type, IP, ForHost);
            ByteData = ByteData.Skip(9).ToArray();
            return true;
        }

        public override byte[] ToByteArray()
        {
            byte[] ret = new byte[9];
            ret[0] = 0x02;
            ret[1] = (byte)m_Type;
            if (IsForHost)
                ret[1] |= 128;
            m_IP.GetAddressBytes().CopyTo(ret, 2);
            ret[6] = (byte)m_TrameReceivedType;
            ret[ret.Length - 2] = NetworkUtils.Checksum(ret, 7);
            ret[ret.Length - 1] = 0x03;
            return ret;
        }

        public override string ToString()
        {
            return base.ToString() + "Received " + m_TrameReceivedType.ToString();
        }
    }
}
