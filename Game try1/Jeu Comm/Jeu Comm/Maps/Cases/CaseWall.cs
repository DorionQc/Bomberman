using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jeu_Comm.Maps.Cases
{
    public class CaseWall : AbsCase
    {
        public CaseWall(int x, int y, Map Parent) : base(x, y, Parent, true, true, false)
        { }

        public override CaseType Type
        {
            get
            {
                return CaseType.Wall;
            }
        }

        public override Image Texture
        {
            get
            {
                return TextureManager.Instance.TextureCaseWall;
            }
        }
    }
}
