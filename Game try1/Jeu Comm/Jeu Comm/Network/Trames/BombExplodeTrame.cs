using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Jeu_Comm.Network.Trames
{
    public class BombExplodeTrame : AbsTrame
    {
        public int ID;
        public byte X, Y;

        public BombExplodeTrame(IPAddress SenderIP, int ID, byte X, byte Y, bool ForHost)
            : base(PacketType.BombExplodePacket, SenderIP, ForHost)
        {
            this.ID = ID;
            this.X = X;
            this.Y = Y;
        }

        public override byte[] ToByteArray()
        {
            byte[] ret = new byte[14];
            ret[0] = 0x02;
            ret[1] = (byte)PacketType.BombExplodePacket;
            if (IsForHost)
                ret[1] |= 128;
            m_IP.GetAddressBytes().CopyTo(ret, 2);
            BitConverter.GetBytes(ID).CopyTo(ret, 6);
            ret[10] = X;
            ret[11] = Y;
            ret[12] = NetworkUtils.Checksum(ret, 12);
            ret[13] = 0x03;
            return ret;
        }

        // STX | PacketType | IP | ID | X | Y | CK | ETX

        public static bool Decode(ref byte[] Data, out AbsTrame Trame)
        {
            Trame = null;
            if (Data.Length < 14)
                return false;
            if (NetworkUtils.Checksum(Data, 12) != Data[12])
                return false;

            IPAddress IP = DecodeIP(Data);
            bool ForHost = (Data[1] & 128) == 1;
            int BombID = BitConverter.ToInt32(Data, 6);
            byte X = Data[10];
            byte Y = Data[11];
            Trame = new BombExplodeTrame(IP, BombID, X, Y, ForHost);
            Data = Data.Skip(14).ToArray();
            return true;

        }
    }
}
