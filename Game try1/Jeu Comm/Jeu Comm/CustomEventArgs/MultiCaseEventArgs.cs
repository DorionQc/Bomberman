using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jeu_Comm.Maps.Cases;

namespace Jeu_Comm.CustomEventArgs
{
    public class MultiCaseEventArgs : CancellableEventArgs
    {
        public AbsCase[] Cases;

        public MultiCaseEventArgs(AbsCase[] Cases, bool Cancelled) : base(Cancelled)
        {
            this.Cases = Cases;
        }
    }
}
