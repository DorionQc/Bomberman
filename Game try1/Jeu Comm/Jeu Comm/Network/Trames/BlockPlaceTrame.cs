using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

using Jeu_Comm.Maps.Cases;

namespace Jeu_Comm.Network.Trames
{
    public class BlockPlaceTrame :  AbsTrame
    {
        public BonusType BonusType;
        public byte X, Y;
        public byte ID;

        public BlockPlaceTrame(IPAddress SenderIP, BonusType Type, byte x, byte y, byte IDJoueur, bool ForHost)
            : base(PacketType.BlockPlacePacket, SenderIP, ForHost)
        {
            X = x;
            Y = y;
            ID = IDJoueur;
            BonusType = Type;
        }

        public override byte[] ToByteArray()
        {
            byte[] ret = new byte[12];
            ret[0] = 0x02;
            ret[1] = (byte)PacketType.BlockPlacePacket;
            if (IsForHost)
                ret[1] |= 128;
            IP.GetAddressBytes().CopyTo(ret, 2);
            ret[6] = ID;
            ret[7] = (byte)BonusType;
            ret[8] = X;
            ret[9] = Y;
            ret[10] = NetworkUtils.Checksum(ret, 10);
            ret[11] = 0x03;
            return ret;
        }

        // STX | PacketType | IP | ID | Type | X | Y | Checksum | ETX

        public static bool Decode(ref byte[] Data, out AbsTrame Trame)
        {
            Trame = null;
            if (Data.Length < 12)
                return false;
            if (NetworkUtils.Checksum(Data, 10) != Data[10])
                return false;
            IPAddress IP = DecodeIP(Data);
            bool ForHost = (Data[1] & 128) == 1;
            BonusType Type = (BonusType)Data[7];
            byte X = Data[8];
            byte Y = Data[9];
            byte PlayedID = Data[6];
            Trame = new BlockPlaceTrame(IP, Type, X, Y, PlayedID, ForHost);
            Data = Data.Skip(12).ToArray();
            return true;
        }
    }
}
