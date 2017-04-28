using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Jeu_Comm.Entities;

namespace Jeu_Comm.CustomEventArgs
{
    public class MultiFireEventArgs : CancellableEventArgs
    {
        public Fire[] Fire;
        public MultiFireEventArgs(Fire[] Fire, bool Cancelled)
            : base(Cancelled)
        {
            this.Fire = Fire;
        }
    }
}
