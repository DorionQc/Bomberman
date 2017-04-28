using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jeu_Comm.Entities;

namespace Jeu_Comm.CustomEventArgs
{
    public class KickedBombEventArgs : CancellableEventArgs
    {
        public Bomb Bomb;
        public CollisionSide Side;
        public KickedBombEventArgs(Bomb b, CollisionSide Side, bool Cancelled) : base(Cancelled)
        {
            Bomb = b;
            this.Side = Side;
        }
    }
}
