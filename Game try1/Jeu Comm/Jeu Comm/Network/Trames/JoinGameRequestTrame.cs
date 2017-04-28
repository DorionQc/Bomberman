using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Jeu_Comm.Network.Trames
{
    public class JoinGameRequestTrame : AbsTrame
    {
        private DateTime m_TimeSent;
        public JoinGameRequestTrame(IPAddress SenderIP)
            : base(PacketType.JoinGameRequestPacket, SenderIP, true)
        {
            m_TimeSent = DateTime.Now;
        }

        public DateTime TimeSent
        {
            get { return m_TimeSent; }
            set { m_TimeSent = value; }
        }

        public static bool Decode(ref byte[] ByteData, out AbsTrame Trame)
        {
            Trame = null;
            if (ByteData.Length < 16)
                return false;
            if (NetworkUtils.Checksum(ByteData, 14) != ByteData[14])
                return false;
            IPAddress IP = DecodeIP(ByteData);
            bool ForHost = (ByteData[1] & 128) == 1;
            DateTime TimeSent = new DateTime(BitConverter.ToInt64(ByteData, 6));
            Trame = new JoinGameRequestTrame(IP);
            ((JoinGameRequestTrame)Trame).TimeSent = TimeSent;
            ByteData = ByteData.Skip(16).ToArray();
            return true;
        }
        // STX | PacketType | IP | TimeSent | Checksum | ETX

        public override byte[] ToByteArray()
        {
            byte[] ret = new byte[16];
            ret[0] = 0x02;
            ret[1] = (byte)m_Type;
            if (IsForHost)
                ret[1] |= 128;
            m_IP.GetAddressBytes().CopyTo(ret, 2);
            BitConverter.GetBytes(m_TimeSent.Ticks).CopyTo(ret, 6);
            ret[14] = NetworkUtils.Checksum(ret, 14);
            ret[15] = 0x03;
            return ret;
        }

        public override string ToString()
        {
            return base.ToString() + "at " + m_TimeSent.ToString("hh:mm:ss");
        }
    }
}
