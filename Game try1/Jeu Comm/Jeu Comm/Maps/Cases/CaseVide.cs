using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Jeu_Comm.Entities;

namespace Jeu_Comm.Maps.Cases
{
    public class CaseVide : AbsCase
    {
        private Bomb m_Bomb;
        public CaseVide(int x, int y, Map Parent) : base(x, y, Parent, false, true, true)
        {
            m_Bomb = null;
        }

        public bool ContainsBomb
        {
            get { return m_Bomb != null; }
        }

        public Bomb Bomb
        {
            get
            {
                return m_Bomb; // null if there's no bomb
            }
            set
            {
                m_Bomb = value;
                m_FireGoThrough = value == null;
            }
        }

        public override bool IsSolid
        {
            get
            {
                return m_Bomb != null;
            }
        }

        public override CaseType Type
        {
            get
            {
                return CaseType.Vide;
            }
        }

        public override Image Texture
        {
            get
            {
                return TextureManager.Instance.TextureCaseVide;
            }
        }
    }
}
