using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Jeu_Comm.Network.Trames
{
    public class BombPlaceTrame : AbsTrame
    {
        public int EntityID;
        public byte X, Y;
        public byte OwnerID;

        public BombPlaceTrame(IPAddress SenderIP, int BombID, byte X, byte Y, byte OwnerID, bool ForHost)
            : base(PacketType.BombPlacePacket, SenderIP, ForHost)
        {
            EntityID = BombID;
            this.X = X;
            this.Y = Y;
            this.OwnerID = OwnerID;
        }

        public override byte[] ToByteArray()
        {
            byte[] ret = new byte[15];
            ret[0] = 0x02;
            ret[1] = (byte)PacketType.BombPlacePacket;
            if (IsForHost)
                ret[1] |= 128;
            m_IP.GetAddressBytes().CopyTo(ret, 2);
            BitConverter.GetBytes(EntityID).CopyTo(ret, 6);
            ret[10] = X;
            ret[11] = Y;
            ret[12] = OwnerID;
            ret[13] = NetworkUtils.Checksum(ret, 13);
            ret[14] = 0x03;
            return ret;
        }

        // STX | PacketType | IP | bombID | X | Y | PlayerID | Checksum | ETX

        public static bool Decode(ref byte[] Data, out AbsTrame Trame)
        {
            Trame = null;
            if (Data.Length < 15)
                return false;
            if (NetworkUtils.Checksum(Data, 13) != Data[13])
                return false;

            IPAddress IP = DecodeIP(Data);
            bool ForHost = (Data[1] & 128) == 1;
            int BombID = BitConverter.ToInt32(Data, 6);
            byte X = Data[10];
            byte Y = Data[11];
            byte PlayedID = Data[12];
            Trame = new BombPlaceTrame(IP, BombID, X, Y, PlayedID, ForHost);
            Data = Data.Skip(15).ToArray();
            return true;

        }


    }
}
