using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Jeu_Comm.Network.Trames
{
    public class PlayerDieTrame : AbsTrame
    {

        public byte PlayerID;

        public PlayerDieTrame(IPAddress SenderIP, byte PlayerID, bool ForHost) 
            : base(PacketType.PlayerDiePacket, SenderIP, ForHost)
        {
            this.PlayerID = PlayerID;
        }

        // STX | PacketType | IP | ID | Checksum | ETX

        public override byte[] ToByteArray()
        {
            byte[] ret = new byte[9];
            ret[0] = 0x02;
            ret[1] = (byte)PacketType.PlayerDiePacket;
            if (IsForHost)
                ret[1] |= 128;
            m_IP.GetAddressBytes().CopyTo(ret, 2);
            ret[6] = PlayerID;
            ret[7] = NetworkUtils.Checksum(ret, 7);
            ret[8] = 0x03;
            return ret;
        }

        public static bool Decode(ref byte[] Data, out AbsTrame Trame)
        {
            Trame = null;
            if (Data.Length < 9)
                return false;
            if (NetworkUtils.Checksum(Data, 7) != Data[7])
                return false;
            byte ID = Data[6];
            IPAddress IP = DecodeIP(Data);
            bool ForHost = (Data[1] & 128) == 1;
            Trame = new PlayerDieTrame(IP, ID, ForHost);
            Data = Data.Skip(9).ToArray();
            return true;
        }
    }
}
