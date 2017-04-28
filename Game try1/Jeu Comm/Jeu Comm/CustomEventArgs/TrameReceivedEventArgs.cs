using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

using Jeu_Comm.Network.Trames;
namespace Jeu_Comm.CustomEventArgs
{
    public class TrameReceivedEventArgs : CancellableEventArgs
    {
        public AbsTrame Trame;
        public Socket Socket;

        public TrameReceivedEventArgs(AbsTrame Trame, Socket Socket, bool Cancelled) : base(Cancelled)
        {
            this.Trame = Trame;
            this.Socket = Socket;
        }

    }
}
