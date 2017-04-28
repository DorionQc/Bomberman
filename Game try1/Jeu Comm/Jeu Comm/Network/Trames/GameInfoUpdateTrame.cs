using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Drawing;
using System.IO;

using Jeu_Comm.Network;
using Jeu_Comm.Lobby;

namespace Jeu_Comm.Network.Trames
{
    public class GameInfoUpdateTrame : AbsTrame
    {
        private LobbyInfo m_LobbyInfo;
        private int m_NumeroJoueur;
        public GameInfoUpdateTrame(IPAddress LocalIP, LobbyInfo lobbyinfo, int NumeroJoueur, bool ForHost) : base(PacketType.GameInfoUpdatePacket, LocalIP, ForHost)
        {
            m_LobbyInfo = lobbyinfo;
            m_NumeroJoueur = NumeroJoueur;
        }

        public LobbyInfo LobbyInfo
        {
            get { return m_LobbyInfo; }
            set { m_LobbyInfo = value; }
        }

        public int NumeroJoueur
        {
            get { return m_NumeroJoueur; }
            set { m_NumeroJoueur = value; }
        }

        public override byte[] ToByteArray()
        {
            byte[] lbBytes = m_LobbyInfo.ToByteArray();
            int Length = 9 + lbBytes.Length;
            byte[] ret = new byte[Length];

            ret[0] = (byte)0x02;
            ret[1] = (byte)PacketType.GameInfoUpdatePacket;
            if (IsForHost)
                ret[1] |= 128;
            m_IP.GetAddressBytes().CopyTo(ret, 2);
            ret[6] = (byte)m_NumeroJoueur;
            lbBytes.CopyTo(ret, 7);
            
            ret[7 + lbBytes.Length] = NetworkUtils.Checksum(ret, 7 + lbBytes.Length);
            ret[8 + lbBytes.Length] = (byte)0x03;
            return ret;

        }

        // STX | PacketType | LocalIP | Numero de joueur | HostIP | Nom de partie |
        // Amount of players | 4 x Player IPs | 4 x Nom of players | 4 x Colors | Checksum | ETX

        public static bool Decode(ref byte[] ByteData, out AbsTrame Trame)
        {
            Trame = null;
            LobbyInfo lb = new LobbyInfo();
            IPAddress IP;
            int NumeroJoueur;
            if (ByteData.Length < 9)
                return false;
            NumeroJoueur = ByteData[6];
            IP = new IPAddress(NetworkUtils.SubArray(ByteData, 2, 4));
            int Position = 7;
            if (!lb.FromByteArray(ByteData, ref Position))
                return false;
            bool ForHost = (ByteData[1] & 128) == 1;
            if (NetworkUtils.Checksum(ByteData, Position) != ByteData[Position] || (ByteData[1] & 127) != (byte)PacketType.GameInfoUpdatePacket)
                return false;

            Trame = new GameInfoUpdateTrame(IP, lb, NumeroJoueur, ForHost);
            ByteData = ByteData.Skip(Position + 2).ToArray();
            return true;

        }

        public override string ToString()
        {
            return base.ToString() + m_LobbyInfo.NomPartie;
        }
    }
}
