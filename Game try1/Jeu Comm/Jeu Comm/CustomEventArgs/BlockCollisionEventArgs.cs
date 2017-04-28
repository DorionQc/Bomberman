using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jeu_Comm.Maps.Cases;
using Jeu_Comm.Entities;

namespace Jeu_Comm.CustomEventArgs
{
    public class BlockCollisionEventArgs : CancellableEventArgs
    {
        public int X;
        public int Y;
        public List<CollisionInfo> Info;

        public BlockCollisionEventArgs(int x, int y, List<CollisionInfo> ColInfo, bool Cancelled) : base(Cancelled)
        {
            X = x;
            Y = x;
            Info = ColInfo;
        }
    }
}
