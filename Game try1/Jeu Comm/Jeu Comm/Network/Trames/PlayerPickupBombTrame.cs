using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Jeu_Comm.Network.Trames
{
    class PlayerPickupBombTrame : AbsTrame
    {
        public byte ID;
        public byte X, Y;

        public PlayerPickupBombTrame(IPAddress SenderIP, byte ID, byte X, byte Y, bool ForHost)
            : base(PacketType.PlayerPickupBombPacket, SenderIP, ForHost)
        {
            this.ID = ID;
            this.X = X;
            this.Y = Y;
        }

        public override byte[] ToByteArray()
        {
            byte[] ret = new byte[11];
            ret[0] = 0x02;
            ret[1] = (byte)PacketType.PlayerPickupBombPacket;
            if (IsForHost)
                ret[1] |= 128;
            IP.GetAddressBytes().CopyTo(ret, 2);
            ret[6] = ID;
            ret[7] = X;
            ret[8] = Y;
            ret[9] = NetworkUtils.Checksum(ret, 9);
            ret[10] = 0x03;
            return ret;
        }

        // STX | PacketType | IP | ID | X | Y | Checksum | ETX

        public static bool Decode(ref byte[] Data, out AbsTrame Trame)
        {
            Trame = null;
            if (Data.Length < 11)
                return false;
            if (NetworkUtils.Checksum(Data, 9) != Data[9])
                return false;
            IPAddress IP = DecodeIP(Data);
            bool ForHost = (Data[1] & 128) == 1;
            byte X = Data[7];
            byte Y = Data[8];
            byte PlayedID = Data[6];
            Trame = new PlayerPickupBombTrame(IP, PlayedID, X, Y, ForHost);
            Data = Data.Skip(11).ToArray();
            return true;
        }
    }
}
