using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Drawing;

using Jeu_Comm.Network;
using Jeu_Comm.Network.Trames;
using Jeu_Comm.CustomEventArgs;
using Jeu_Comm.Lobby;

namespace Jeu_Comm.Network
{
    /// <summary>
    /// Structure servant à lier les informations d'un socket au numéro associé
    /// </summary>
    public struct ConnectionInfo
    {
        public Socket Socket;
        public byte NumeroJoueur;

        public ConnectionInfo(Socket Socket, byte NumeroJoueur)
        {
            this.Socket = Socket;
            this.NumeroJoueur = NumeroJoueur;
        }
    }
}
