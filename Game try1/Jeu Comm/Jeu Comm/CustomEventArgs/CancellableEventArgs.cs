using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jeu_Comm.CustomEventArgs
{
    public class CancellableEventArgs
    {
        public bool Cancelled;

        public CancellableEventArgs(bool Cancelled)
        {
            this.Cancelled = Cancelled;
        }
    }
}
