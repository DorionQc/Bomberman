using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

using Jeu_Comm.Entities;

namespace Jeu_Comm.Maps.Cases
{

    public enum CaseType
    {
        Vide = 0,
        Wall,
        SolidWall,
        Bonus
    };

    public abstract class AbsCase
    {
        protected int m_x;
        protected int m_y;

        protected Map m_Map;

        protected bool m_Breakable;

        protected bool m_Breaking;
        protected bool m_FireGoThrough;

        protected bool m_Solid;

        protected Fire m_FireEntity;

        public AbsCase(int x, int y, Map m, bool Solid, bool Breakable, bool LetsFireThrough)
        {
            m_x = x;
            m_y = y;
            m_Map = m;
            m_Solid = Solid;
            m_Breakable = Breakable;
            m_Breaking = false;
            m_FireGoThrough = LetsFireThrough;
        }

        public int X
        {
            get { return m_x; }
        }

        public int Y
        {
            get { return m_y; }
        }

        public Map Parent
        {
            get { return m_Map; }
        }

        public bool IsBreakable
        {
            get { return m_Breakable; }
        }

        public bool IsBreaking
        {
            get { return m_FireEntity != null; }
        }

        public Fire Fire
        {
            get { return m_FireEntity; }
            set
            {
                m_FireEntity = value;
            }
        }

        public bool LetsFireThrough
        {
            get { return m_FireGoThrough; }
        }

        public virtual bool IsSolid
        {
            get { return m_Solid; }
        }

        public abstract Image Texture { get; }

        public abstract CaseType Type { get; }

        public virtual void Draw(Graphics g, Rectangle r, SolidBrush b, float Width)
        {
            g.DrawImage(Texture, m_x * Width, m_y * Width, Width, Width);
        }

        public override string ToString()
        {
            return string.Format("{0} at {1}, {2}", Type, m_x, m_y);
        }
    }
}
