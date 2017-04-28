using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Drawing;

using Jeu_Comm.Maps.Cases;

namespace Jeu_Comm.Network.Trames
{
    public class BlockBreakTrame : AbsTrame
    {
        public Point[] Points;
        public byte ID;

        public BlockBreakTrame(IPAddress SenderIP, byte ID, Point[] Pos, bool ForHost)
            : base(PacketType.BlockBreakPacket, SenderIP, ForHost)
        {
            this.ID = ID;
            Points = Pos;
        }

        public override byte[] ToByteArray()
        {
            byte[] ret = new byte[10 + 2 * Points.Length];
            ret[0] = 0x02;
            ret[1] = (byte)PacketType.BlockBreakPacket;
            if (IsForHost)
                ret[1] |= 128;
            m_IP.GetAddressBytes().CopyTo(ret, 2);
            ret[6] = ID;
            ret[7] = (byte)Points.Length;
            int Pos = 8;
            for (int i = 0; i < Points.Length; i++)
            {
                ret[Pos++] = (byte)Points[i].X;
                ret[Pos++] = (byte)Points[i].Y;
            }
            ret[Pos] = NetworkUtils.Checksum(ret, Pos);
            ret[Pos + 1] = 0x03;

            return ret;
        }

        // STX | PacketType | IP | ID | Count | Points | Checksum | ETX

        public static bool Decode(ref byte[] Data, out AbsTrame Trame)
        {
            Trame = null;
            if (Data.Length < 10)
                return false;
            byte Length = Data[7];
            if (Data.Length < 10 + Length * 2)
                return false;
            if (NetworkUtils.Checksum(Data, 8 + Length * 2) != Data[8 + Length * 2])
                return false;
            IPAddress IP = DecodeIP(Data);
            byte ID = Data[6];
            bool ForHost = (Data[1] & 128) == 1;
            List<Point> pts = new List<Point>(Length);
            for (int i = 0; i < Length; i++)
            {
                pts.Add(new Point(Data[8 + 2 * i], Data[9 + 2 * i]));
            }
            Trame = new BlockBreakTrame(IP, ID, pts.ToArray(), ForHost);
            Data = Data.Skip(10 + Length * 2).ToArray();
            return true;
        }
    }
}
