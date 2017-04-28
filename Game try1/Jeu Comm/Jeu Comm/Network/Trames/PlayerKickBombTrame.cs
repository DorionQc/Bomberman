using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

using Jeu_Comm.Entities;

namespace Jeu_Comm.Network.Trames
{
    public class PlayerKickBombTrame : PlayerThrowBombTrame
    {
        public PlayerKickBombTrame(IPAddress SenderIP, byte PlayerID, CollisionSide Side, int BombID, bool ForHost)
            : base(SenderIP, PlayerID, Side, BombID, ForHost)
        {
            m_Type = PacketType.PlayerKickBombPacket;
        }

        public override byte[] ToByteArray()
        {
            byte[] ret = base.ToByteArray();
            ret[1] = (byte)PacketType.PlayerKickBombPacket;
            ret[12] = NetworkUtils.Checksum(ret, 12);
            return ret;
        }

        public static new bool Decode(ref byte[] Data, out AbsTrame Trame)
        {
            Trame = null;
            if (Data.Length < 14)
                return false;
            if (NetworkUtils.Checksum(Data, 12) != Data[12])
                return false;

            IPAddress IP = DecodeIP(Data);
            bool ForHost = (Data[1] & 128) == 1;
            int BombID = BitConverter.ToInt32(Data, 6);
            CollisionSide s = (CollisionSide)Data[10];
            byte PlayerID = Data[11];
            Trame = new PlayerKickBombTrame(IP, PlayerID, s, BombID, ForHost);
            Data = Data.Skip(14).ToArray();
            return true;

        }
    }
}
