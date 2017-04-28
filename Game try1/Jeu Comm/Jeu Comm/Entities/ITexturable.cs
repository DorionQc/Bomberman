using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jeu_Comm.Entities
{
    interface ITexturable
    {
        void UpdateTexture(long DeltaTime);
    }
}
