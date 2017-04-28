using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Jeu_Comm.Network.Trames
{
    public enum PacketType : byte
    {
        BroadcastGamePacket,
        JoinGameRequestPacket,
        RegisterPlayerPacket,
        GameInfoUpdatePacket,
        GameStartEndPacket,

        PlayerMovePacket,
        PlayerPositionUpdatePacket,
        PlayerDiePacket,
        BlockBreakPacket,
        BlockPlacePacket,
        BombPlacePacket,
        BombExplodePacket,
        PlayerPickupBombPacket,
        PlayerThrowBombPacket,
        PlayerKickBombPacket,
        PlayerPickupBonusPacket,
        GameEndPacket,
        ACKPacket
    };

    public abstract class AbsTrame
    {
        protected PacketType m_Type;
        protected IPAddress m_IP;
        protected bool m_ForHost;

        public AbsTrame(PacketType Type, IPAddress SenderIP, bool ForHost)
        {
            m_Type = Type;
            m_IP = SenderIP;
            m_ForHost = ForHost;
        }

        public bool IsForHost
        {
            get { return m_ForHost; }
            set { m_ForHost = value; }
        }

        public PacketType Type
        {
            get { return m_Type; }
        }
        public IPAddress IP
        {
            get { return m_IP; }
            set { m_IP = value; }
        }

        // STX | PacketType | IP (4 bytes) | Checksum (1 byte) | ETX
        /// <summary>
        /// Takes the bytes from 2 to 6 and converts them to an IP
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        protected static IPAddress DecodeIP(byte[] Data)
        {
            if (Data.Length < 8)
                return IPAddress.None;
            return new IPAddress(new byte[] { Data[2], Data[3], Data[4], Data[5] });
        }

        public abstract byte[] ToByteArray();

        public override string ToString()
        {
            return m_Type.ToString() + "@" + IP.ToString() + " | ";
        }
    }
}
