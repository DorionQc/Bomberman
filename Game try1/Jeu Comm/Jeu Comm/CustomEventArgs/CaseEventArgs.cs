using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jeu_Comm.Maps.Cases;

namespace Jeu_Comm.CustomEventArgs
{
    public class CaseEventArgs : CancellableEventArgs
    {
        public AbsCase Case;

        public CaseEventArgs(AbsCase Case, bool Cancelled) : base(Cancelled)
        {
            this.Case = Case;
        }
    }
}
