using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Jeu_Comm.Maps;
using Jeu_Comm.Maps.Cases;
using Jeu_Comm.Entities;
using Jeu_Comm.CustomEventArgs;

namespace Jeu_Comm.Entities
{
    public delegate void OnMoveHandler(object sender, CancellableEventArgs e);
    public delegate void OnDieHandler(object sender, CancellableEventArgs e);
    public delegate void OnDropBombHandler(object sender, CaseEventArgs e);
    public delegate void OnPickBombHandler(object sender, CaseEventArgs e);
    public delegate void OnKickBombHandler(object sender, KickedBombEventArgs e);
    public delegate void OnGetBonusHandler(object sender, CaseEventArgs e);
    public delegate void OnChangeCaseHandler(object sender, MultiCaseEventArgs e);
    public delegate void OnCollideWithBlockHandler(object sender, BlockCollisionEventArgs e);
    public delegate void OnBombExplodeHandler(object sender, CaseEventArgs e);
    public delegate void OnShootBombHandler(object sender, ShootBombEventArgs e);
    public delegate void OnFireStopHandler(object sender, MultiFireEventArgs e);

    public delegate void OnGenericMultiblockEventHandler(object sender, MultiCaseEventArgs e);
    public delegate void OnGenericBlockEventHandler(object sender, CaseEventArgs e);

    public enum EntityType
    {
        Joueur = 0,
        Bomb,
        Fire
    };

    public abstract class AbsEntity
    {
        protected int m_x;
        protected int m_y;

        protected int m_Size;

        protected bool m_Dead;
        protected bool m_Registered;

        protected Map m_Map;

        protected int m_ID;

        public event OnDieHandler Died;
        protected void FireDied(object sender, CancellableEventArgs e)
        {
            if (Died != null)
                Died(sender, e);
        }

        public AbsEntity(int x, int y, Map m, bool Registered)
        {
            m_Registered = Registered;
            m_x = x;
            m_y = y;
            m_Map = m;
            m_Size = 15;
            m_Dead = false;
            m_ID = (int)DateTime.Now.Ticks ^ (x << 16) ^ y;
        }

        public bool IsDead
        {
            get { return m_Dead; }
            set { m_Dead = value; }
        }

        public int ID
        {
            get { return m_ID; }
        }

        public int X
        {
            get { return m_x; }
            set { if (value >= 0 && value < m_Map.NoCase * Map.EntityPixelPerCase) m_x = value; }
        }

        public int Y
        {
            get { return m_y; }
            set { if (value >= 0 && value < m_Map.NoCase * Map.EntityPixelPerCase) m_y = value; }
        }

        public int Radius
        {
            get { return m_Size; }
            set { m_Size = value; }
        }

        public Map Map
        {
            get { return m_Map; }
            set { m_Map = value; }
        }

        public bool IsRegistered
        {
            get { return m_Registered; }
        }

        public abstract EntityType Type { get; }

        /// <summary>
        /// Calcule et donne un tableau contenant les cases dans lesquelles se trouve l'entité
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public AbsCase[] GetCasesIn(int x, int y)
        {
            int n = 1, i = 0;
            int x1, x2, y1, y2;
            bool bx, by;
            float r = m_Size + 0.01f;
            x1 = (x - m_Size) / Map.EntityPixelPerCase;
            x2 = (x + m_Size - 1) / Map.EntityPixelPerCase;
            y1 = (y - m_Size) / Map.EntityPixelPerCase;
            y2 = (y + m_Size - 1) / Map.EntityPixelPerCase;
            if (y2 >= m_Map.NoCase)
            {
                y2 = m_Map.NoCase - 1;
                if (y1 >= m_Map.NoCase)
                    y1 = m_Map.NoCase - 1;
            }
            if (x2 >= m_Map.NoCase)
            {
                x2 = m_Map.NoCase - 1;
                if (x1 >= m_Map.NoCase)
                    x1 = m_Map.NoCase - 1;
            }
            if (x1 < 0)
            {
                x1 = 0;
                if (x2 < 0)
                    x2 = 0;
            }
            if (y1 < 0)
            {
                y1 = 0;
                if (y2 < 0)
                    y2 = 0;
            }
            bx = (x1 != x2);
            by = (y1 != y2);
            if (bx) n <<= 1;
            if (by) n <<= 1;
            AbsCase[] ret = new AbsCase[n];
            ret[i++] = m_Map[x1, y1];
            if (bx) ret[i++] = m_Map[x2, y1];
            if (by) ret[i++] = m_Map[x1, y2];
            if (bx && by) ret[i++] = m_Map[x2, y2];
            return ret;
        }

        public abstract void Tick(long DeltaTime);

        public abstract void Draw(Graphics g, Rectangle r, SolidBrush b, float Width);

    }
}
