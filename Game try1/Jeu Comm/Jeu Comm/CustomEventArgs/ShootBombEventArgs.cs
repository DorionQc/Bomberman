using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jeu_Comm.Entities;
using Jeu_Comm.Maps.Cases;

namespace Jeu_Comm.CustomEventArgs
{
    public class ShootBombEventArgs : CancellableEventArgs
    {
        public Joueur Joueur;
        public Bomb Bomb;
        public CollisionSide Side;

        public ShootBombEventArgs(Joueur j, Bomb b, CollisionSide s, bool Cancelled) : base(Cancelled)
        {
            Joueur = j;
            Bomb = b;
            Side = s;
        }
    }
}
