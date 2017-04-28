using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Jeu_Comm.Entities;


namespace Jeu_Comm.Network.Trames
{
    public class PlayerPositionUpdateTrame : AbsTrame
    {
        private PlayerPositions m_Pos;

        public PlayerPositionUpdateTrame(IPAddress SenderIP, PlayerPositions Positions, bool ForHost) : base(PacketType.PlayerPositionUpdatePacket, SenderIP, ForHost)
        {
            m_Pos = Positions;
        }

        public PlayerPositions Positions
        {
            get { return m_Pos; }
            set { m_Pos = value; }
        }

        public static bool Decode(ref byte[] Data, out AbsTrame Trame)
        {
            Trame = null;
            if (Data.Length < 72)
                return false;
            bool ForHost = (Data[1] & 128) > 0;
            IPAddress IP = DecodeIP(Data);
            PlayerPositions Positions = new PlayerPositions();
            int Pos = 6;
            if (!Positions.Decode(Data, ref Pos))
                return false;
            Trame = new PlayerPositionUpdateTrame(IP, Positions, ForHost);
            Data = Data.Skip(72).ToArray();
            return true;
        }

        // STX | PacketType | IP | PlayerPositions (64 bytes) | Checksum | ETX

        public override byte[] ToByteArray()
        {
            byte[] ret = new byte[72];
            ret[0] = 0x02;
            ret[1] = (byte)PacketType.PlayerPositionUpdatePacket;
            if (IsForHost)
                ret[1] |= 128;
            IP.GetAddressBytes().CopyTo(ret, 2);
            Positions.ToByteArray().CopyTo(ret, 6);
            ret[70] = NetworkUtils.Checksum(ret, 70);
            ret[71] = 0x03;
            return ret;
        }
    }
}
