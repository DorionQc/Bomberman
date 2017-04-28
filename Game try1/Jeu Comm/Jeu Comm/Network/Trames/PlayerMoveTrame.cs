using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Jeu_Comm.Network.Trames
{
    public class PlayerMoveTrame : AbsTrame
    {
        public byte ID;
        public int X, Y;
        public float VelX, VelY;
        public PlayerMoveTrame(IPAddress SenderIP, byte PlayerID, int x, int y, float velx, float vely, bool ForHost) : base(PacketType.PlayerMovePacket, SenderIP, ForHost)
        {
            ID = PlayerID;
            X = x;
            Y = y;
            VelX = velx;
            VelY = vely;
        }

        public static bool Decode(ref byte[] Data, out AbsTrame Trame)
        {
            Trame = null;
            if (Data.Length < 25)
                return false;
            if (!(NetworkUtils.Checksum(Data, 23) == Data[23]))
                return false;
            IPAddress IP = DecodeIP(Data);
            bool ForHost = (Data[1] & 128) > 0;
            byte ID = Data[6];
            int X = BitConverter.ToInt32(Data, 7);
            int Y = BitConverter.ToInt32(Data, 11);
            float VelX = BitConverter.ToSingle(Data, 15);
            float VelY = BitConverter.ToSingle(Data, 19);
            Trame = new PlayerMoveTrame(IP, ID, X, Y, VelX, VelY, ForHost);
            Data = Data.Skip(25).ToArray();
            return true;
        }

        // STX | PacketType | IP | ID | X | Y | VelX | VelY | Checksum | ETX

        public override byte[] ToByteArray()
        {
            byte[] ret = new byte[25];
            ret[0] = 0x02;
            ret[1] = (byte)PacketType.PlayerMovePacket;
            if (IsForHost)
                ret[1] |= 128;
            IP.GetAddressBytes().CopyTo(ret, 2);
            ret[6] = ID;
            BitConverter.GetBytes(X).CopyTo(ret, 7);
            BitConverter.GetBytes(Y).CopyTo(ret, 11);
            BitConverter.GetBytes(VelX).CopyTo(ret, 15);
            BitConverter.GetBytes(VelY).CopyTo(ret, 19);
            ret[23] = NetworkUtils.Checksum(ret, 23);
            ret[24] = 0x03;
            return ret;
        }

        public override string ToString()
        {
            return base.ToString() + string.Format("{0} - at {1}, {2} with V:{3}, {4}", ID, X, Y, VelX, VelY);
        }
    }
}
