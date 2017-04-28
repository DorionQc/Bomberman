using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

using Jeu_Comm.Lobby;

namespace Jeu_Comm.Network.Trames
{
    public class RegisterPlayerTrame : AbsTrame
    {
        protected DateTime m_TimeSent;
        protected byte m_IDJoueur;

        public RegisterPlayerTrame(DateTime TimeSent, IPAddress SenderIP, byte PlayerID, bool ForHost)
            : base(PacketType.RegisterPlayerPacket, SenderIP, ForHost)
        {
            m_TimeSent = TimeSent;
            m_IDJoueur = PlayerID;
        }

        public DateTime TimeSent
        {
            get { return m_TimeSent; }
            set { m_TimeSent = value; }
        }

        public byte IDJoueur
        {
            get { return m_IDJoueur; }
            set { m_IDJoueur = value; }
        }

        public static bool Decode(ref byte[] ByteData, out AbsTrame Trame)
        {
            Trame = null;
            if (ByteData.Length < 17)
                return false;
            if (NetworkUtils.Checksum(ByteData, 15) != ByteData[15])
                return false;
            IPAddress IP = DecodeIP(ByteData);
            bool ForHost = (ByteData[1] & 128) == 1;
            DateTime TimeSent = new DateTime(BitConverter.ToInt64(ByteData, 6));
            byte NumeroJoueur = ByteData[14];
            Trame = new RegisterPlayerTrame(TimeSent, IP, NumeroJoueur, ForHost);
            ByteData = ByteData.Skip(17).ToArray();
            return true;
        }

        // STX | PacketType | IP | Time Sent | NumeroJoueur | Checksum | ETX

        public override byte[] ToByteArray()
        {
            byte[] ret = new byte[17];
            ret[0] = 0x02;
            ret[1] = (byte)m_Type;
            if (IsForHost)
                ret[1] |= 128;
            m_IP.GetAddressBytes().CopyTo(ret, 2);
            BitConverter.GetBytes(m_TimeSent.Ticks).CopyTo(ret, 6);
            ret[14] = m_IDJoueur;
            ret[15] = NetworkUtils.Checksum(ret, 15);
            ret[16] = 0x03;
            return ret;
        }

        public override string ToString()
        {
            return base.ToString() + "at " + m_TimeSent.ToString("hh:mm:ss");
        }

    }
}
