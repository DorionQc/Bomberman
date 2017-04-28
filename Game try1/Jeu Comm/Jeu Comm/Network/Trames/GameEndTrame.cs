using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Jeu_Comm.Network.Trames
{
    public class GameEndTrame : AbsTrame
    {
        public byte ID;

        public GameEndTrame(IPAddress SenderIP, byte ID, bool ForHost)
            : base(PacketType.GameEndPacket, SenderIP, ForHost)
        {
            this.ID = ID;
        }

        // STX | PacketType | IP | ID | CK | ETX

        public override byte[] ToByteArray()
        {
            byte[] ret = new byte[9];
            ret[0] = 0x02;
            ret[1] = (byte)PacketType.GameEndPacket;
            if (IsForHost)
                ret[1] |= 128;
            m_IP.GetAddressBytes().CopyTo(ret, 2);
            ret[6] = ID;
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
            IPAddress IP = DecodeIP(Data);
            bool ForHost = (Data[1] & 128) == 1;
            byte ID = Data[6];
            Trame = new GameEndTrame(IP, ID, ForHost);
            Data = Data.Skip(9).ToArray();
            return true;
        }
    }
}
