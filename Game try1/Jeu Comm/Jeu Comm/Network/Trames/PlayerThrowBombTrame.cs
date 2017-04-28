using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

using Jeu_Comm.Entities;

namespace Jeu_Comm.Network.Trames
{
    public class PlayerThrowBombTrame : AbsTrame
    {
        public byte ID;
        public CollisionSide Side;
        public int BombID;

        public PlayerThrowBombTrame(IPAddress SenderIP, byte PlayerID, CollisionSide Side, int BombID, bool ForHost)
            : base(PacketType.PlayerThrowBombPacket, SenderIP, ForHost)
        {
            this.ID = PlayerID;
            this.Side = Side;
            this.BombID = BombID;
        }

        public override byte[] ToByteArray()
        {
            byte[] ret = new byte[14];
            ret[0] = 0x02;
            ret[1] = (byte)PacketType.PlayerThrowBombPacket;
            if (IsForHost)
                ret[1] |= 128;
            m_IP.GetAddressBytes().CopyTo(ret, 2);
            BitConverter.GetBytes(BombID).CopyTo(ret, 6);
            ret[10] = (byte)Side;
            ret[11] = ID;
            ret[12] = NetworkUtils.Checksum(ret, 12);
            ret[13] = 0x03;
            return ret;
        }

        // STX | PacketType | IP | bombID | Side | PlayerID | Checksum | ETX

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
            CollisionSide s = (CollisionSide)Data[10];
            byte PlayerID = Data[11];
            Trame = new PlayerThrowBombTrame(IP, PlayerID, s, BombID, ForHost);
            Data = Data.Skip(14).ToArray();
            return true;

        }
    }
}
