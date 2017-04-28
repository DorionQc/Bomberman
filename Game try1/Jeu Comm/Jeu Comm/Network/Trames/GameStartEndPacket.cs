using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Drawing;

using Jeu_Comm.Lobby;
using Jeu_Comm.Maps;

namespace Jeu_Comm.Network.Trames
{
    public class GameStartEndPacket : AbsTrame
    {
        private bool m_StartEnd;
        private Map m_Map;
        private LobbyInfo m_LobbyInfo;
        public GameStartEndPacket(IPAddress SenderIP, LobbyInfo lb, bool StartEnd, bool ForHost, Map map)
            : base(PacketType.GameStartEndPacket, SenderIP, ForHost)
        {
            m_LobbyInfo = lb;
            m_StartEnd = StartEnd;
            m_Map = map;
        }

        public bool StartEnd
        {
            get { return m_StartEnd; }
            set { m_StartEnd = value; }
        }

        public Map Map
        {
            get { return m_Map; }
            set { m_Map = value; }
        }

        public LobbyInfo LobbyInfo
        {
            get { return m_LobbyInfo; }
            set { m_LobbyInfo = value; }
        }

        // STX | PacketType | LocalIP | Start/End | LobbyInfo | Map | Ck | ETX
        public static bool Decode(ref byte[] Data, out AbsTrame Trame)
        {
            Trame = null;
            if (Data.Length < 9)
                return false;
            LobbyInfo lb = new LobbyInfo();
            IPAddress IP;
            Map map = new Map(21);
            bool StartEnd;
            IP = DecodeIP(Data);
            StartEnd = Data[6] > 0;
            bool ForHost = (Data[1] & 128) == 1;
            int Position = 7;
            if (!lb.FromByteArray(Data, ref Position))
                return false;
            if (!map.FromByteArray(Data, ref Position))
                return false;
            if (NetworkUtils.Checksum(Data, Position) != Data[Position]
                || (Data[1] & 127) != (byte)PacketType.GameStartEndPacket)
                return false;
            Trame = new GameStartEndPacket(IP, lb, StartEnd, ForHost, map);
            Data = Data.Skip(Position + 2).ToArray();
            return true;
            
        }

        // STX | PacketType | LocalIP | Start/End | LobbyInfo | Map | Ck | ETX
        public override byte[] ToByteArray()
        {
            byte[] map = m_Map.ToByteArray();
            byte[] lb = m_LobbyInfo.ToByteArray();
            byte[] ret = new byte[9 + lb.Length + map.Length];
            ret[0] = 0x02;
            ret[1] = (byte)PacketType.GameStartEndPacket;
            if (IsForHost)
                ret[1] |= 128;
            m_IP.GetAddressBytes().CopyTo(ret, 2);
            ret[6] = m_StartEnd ? (byte)1 : (byte)0;
            lb.CopyTo(ret, 7);
            map.CopyTo(ret, 7 + lb.Length);
            ret[ret.Length - 2] = NetworkUtils.Checksum(ret, ret.Length - 2);
            ret[ret.Length - 1] = 0x03;
            return ret;

        }

        public override string ToString()
        {
            return base.ToString() + " Game Started";
        }
    }
}
