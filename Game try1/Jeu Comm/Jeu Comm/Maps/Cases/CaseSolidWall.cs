using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jeu_Comm.Maps.Cases
{
    public class CaseSolidWall : AbsCase
    {
        public CaseSolidWall(int x, int y, Map Parent) : base(x, y, Parent, true, false, false)
        { }

        public override CaseType Type
        {
            get
            {
                return CaseType.SolidWall;
            }
        }

        public override Image Texture
        {
            get
            {
                return TextureManager.Instance.TextureCaseSolidWall;
            }
        }
    }
}
