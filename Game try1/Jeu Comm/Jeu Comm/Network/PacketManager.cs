using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Jeu_Comm.Network.Trames;

namespace Jeu_Comm.Network
{
    public static class PacketManager
    {
        /// <summary>
        /// Décode la trame, en se fiant au deuxième byte (PacketType)
        /// </summary>
        /// <param name="Packet"></param>
        /// <param name="Trame"></param>
        /// <returns></returns>
        public static bool Decode(ref byte[] Packet, out AbsTrame Trame)
        {
            Trame = null;
            if (Packet.Length == 0)
                return false;
            switch (Packet[1] & 127)
            {
                default:                                           return false;
                case (byte)PacketType.BroadcastGamePacket:         return (BroadcastTrame.Decode(ref Packet, out Trame));
                case (byte)PacketType.JoinGameRequestPacket:       return (JoinGameRequestTrame.Decode(ref Packet, out Trame));
                case (byte)PacketType.RegisterPlayerPacket:        return (RegisterPlayerTrame.Decode(ref Packet, out Trame));
                case (byte)PacketType.GameStartEndPacket:          return (GameStartEndPacket.Decode(ref Packet, out Trame));
                case (byte)PacketType.GameInfoUpdatePacket:        return (GameInfoUpdateTrame.Decode(ref Packet, out Trame));
                case (byte)PacketType.PlayerMovePacket:            return (PlayerMoveTrame.Decode(ref Packet, out Trame));
                case (byte)PacketType.PlayerPositionUpdatePacket:  return (PlayerPositionUpdateTrame.Decode(ref Packet, out Trame));
                case (byte)PacketType.PlayerDiePacket:             return (PlayerDieTrame.Decode(ref Packet, out Trame));
                case (byte)PacketType.BlockBreakPacket:            return (BlockBreakTrame.Decode(ref Packet, out Trame));
                case (byte)PacketType.BlockPlacePacket:            return (BlockPlaceTrame.Decode(ref Packet, out Trame));
                case (byte)PacketType.BombPlacePacket:             return (BombPlaceTrame.Decode(ref Packet, out Trame));
                case (byte)PacketType.BombExplodePacket:           return (BombExplodeTrame.Decode(ref Packet, out Trame));
                case (byte)PacketType.PlayerPickupBombPacket:      return (PlayerPickupBombTrame.Decode(ref Packet, out Trame));
                case (byte)PacketType.PlayerKickBombPacket:        return (PlayerKickBombTrame.Decode(ref Packet, out Trame));
                case (byte)PacketType.PlayerThrowBombPacket:       return (PlayerThrowBombTrame.Decode(ref Packet, out Trame));
                case (byte)PacketType.PlayerPickupBonusPacket:     return (PlayerPickupBonusTrame.Decode(ref Packet, out Trame));
                case (byte)PacketType.GameEndPacket:               return (GameEndTrame.Decode(ref Packet, out Trame));
                case (byte)PacketType.ACKPacket:                   return (ACKTrame.Decode(ref Packet, out Trame));
            }
        }
    }
}
