using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Jeu_Comm.Maps.Cases;
using Jeu_Comm.CustomEventArgs;
using Jeu_Comm.Maps;

namespace Jeu_Comm.Entities
{

    [Flags]
    public enum CollisionSide
    {
        None = 0,
        Up = 1,
        Left = 2,
        Down = 4,
        Right = 8
    };

    public struct CollisionInfo
    {
        public CollisionSide Side;
        public AbsCase Case;

        public CollisionInfo(CollisionSide Side, AbsCase Case)
        {
            this.Side = Side;
            this.Case = Case;
        }
    }

    public abstract class AbsMoveableEntity : AbsEntity
    {

        protected float m_Velx;
        protected float m_Vely;

        public event OnChangeCaseHandler ChangedCase;
        public event OnMoveHandler Moved;
        public event OnCollideWithBlockHandler Collided;

        public AbsMoveableEntity(int x, int y, Map m, bool Registered)
            : base(x, y, m, Registered)
        {
            m_Velx = 0;
            m_Vely = 0;
        }

        protected void FireMoved(object sender, CancellableEventArgs e)
        {
            if (Moved != null)
                Moved(sender, e);
        }

        protected void FireChangedCase(object sender, MultiCaseEventArgs e)
        {
            if (ChangedCase != null)
                ChangedCase(sender, e);
        }
        protected void FireCollided(object sender, BlockCollisionEventArgs e)
        {
            if (Collided != null)
                Collided(sender, e);
        }


        public float VelX
        {
            get { return m_Velx; }
            set { m_Velx = value; }
        }

        public float VelY
        {
            get { return m_Vely; }
            set { m_Vely = value; }
        }
        
        /// <summary>
        /// Vérifie les collisions avec les cases solides
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="velx"></param>
        /// <param name="vely"></param>
        /// <returns></returns>
        protected virtual List<CollisionInfo> CheckCollision(int x, int y, float velx, float vely)
        {

            List<CollisionInfo> ret = new List<CollisionInfo>(2);
            int i = 0, n = 0;
            int xm = x + (int)velx - m_Size; // top left
            int ym = y + (int)vely - m_Size;
            int rad2 = m_Size + m_Size;
            int oldx = x - m_Size;
            int oldy = y - m_Size;
            AbsCase[] Cases = GetCasesIn(x + (int)velx, y + (int)vely);


            if (xm < 0)
                ret.Add(new CollisionInfo(CollisionSide.Left, null));
            else if (xm + rad2 >= m_Map.NoCase * Map.EntityPixelPerCase)
                ret.Add(new CollisionInfo(CollisionSide.Right, null));
            if (ym < 0)
                ret.Add(new CollisionInfo(CollisionSide.Up, null));
            else if (ym + rad2 >= m_Map.NoCase * Map.EntityPixelPerCase)
                ret.Add(new CollisionInfo(CollisionSide.Down, null));

            for (; i < Cases.Length; i++)
            {
                if (Cases[i].IsSolid)
                    n++;
                if (Cases[i].IsBreaking)
                    FireDied(this, new CancellableEventArgs(false));
            }
            if (n == 0)
            {
                if (ret.Count > 0)
                    i = 0;
                return ret;
            }
            Cases = Cases.Where((ca) => ca.IsSolid == true).OrderBy((a) => Math.Abs(xm - a.X * Map.EntityPixelPerCase) + Math.Abs(ym - a.Y * Map.EntityPixelPerCase)).ToArray();

            float dxm, dym, dx, dy;
            for (i = 0; i < n; i++)
            {
                dxm = xm - Cases[i].X * Map.EntityPixelPerCase;
                dym = ym - Cases[i].Y * Map.EntityPixelPerCase;
                dx = x - Cases[i].X * Map.EntityPixelPerCase - m_Size;
                dy = y - Cases[i].Y * Map.EntityPixelPerCase - m_Size;
                if (dx <= -rad2)
                {
                    if (-dxm <= rad2 - 1 && dym > -rad2 && dym < Map.EntityPixelPerCase)
                    {
                        ret.Add(new CollisionInfo(CollisionSide.Left, Cases[i]));
                        xm = x - m_Size;
                    }
                }
                else if (dx >= Map.EntityPixelPerCase)
                {
                    if (dxm <= Map.EntityPixelPerCase && dym > -rad2 && dym < Map.EntityPixelPerCase)
                    {
                        ret.Add(new CollisionInfo(CollisionSide.Right, Cases[i]));
                        xm = x - m_Size;
                    }
                }
                if (dy <= -rad2)
                {
                    if (-dym <= rad2 - 1 && dxm > -rad2 && dxm < Map.EntityPixelPerCase)
                    {
                        ret.Add(new CollisionInfo(CollisionSide.Up, Cases[i]));
                        ym = y - m_Size;
                    }
                }
                else if (dy >= Map.EntityPixelPerCase)
                {
                    if (dym <= Map.EntityPixelPerCase && dxm > -rad2 && dxm < Map.EntityPixelPerCase)
                    {
                        ret.Add(new CollisionInfo(CollisionSide.Down, Cases[i]));
                        ym = y - m_Size;
                    }
                }
            }

            return ret;
        }

        /// <summary>
        /// Calcule le mouvement de l'entité
        /// </summary>
        /// <param name="DeltaTime"></param>
        public override void Tick(long DeltaTime)
        {
            float dt = DeltaTime / 2;
            int vx = (int)(m_Velx * dt);
            int vy = (int)(m_Vely * dt);
            List<CollisionInfo> res = CheckCollision(m_x, m_y, vx, vy);
            CollisionSide rs = 0;
            foreach (CollisionInfo c in res)
            {
                rs |= c.Side;
                if (c.Case != null && c.Case.IsBreaking)
                    FireDied(this, new CancellableEventArgs(false));
            }
            bool Or = false;
            if (Math.Abs(m_Velx) < 0.01f)
            {
                m_Velx = 0;
                Or = true;
            }
            if (Math.Abs(m_Vely) < 0.01f)
            {
                m_Vely = 0;
                Or = true;
            }
            if (Or)
                FireMoved(this, new CancellableEventArgs(false));

            if (vx == 0 && vy == 0)
                return; 

            
            if (rs != CollisionSide.None)
            {
                BlockCollisionEventArgs e = new BlockCollisionEventArgs(m_x / Map.EntityPixelPerCase, m_y / Map.EntityPixelPerCase, res, false);
                FireCollided(this, e);
                if ((rs & (CollisionSide.Left | CollisionSide.Right)) > 0)
                {
                    //m_Velx = 0;
                    vx = 0;
                }
                if ((rs & (CollisionSide.Up | CollisionSide.Down)) > 0)
                {
                    //m_Vely = 0;
                    vy = 0;
                }
            }

            
            if (m_x / Map.EntityPixelPerCase != (m_x + vx) / Map.EntityPixelPerCase) // Side only
            {
                FireChangedCase(this, new MultiCaseEventArgs(new AbsCase[] {
                    m_Map[(m_x + vx) / Map.EntityPixelPerCase, (m_y + vy) / Map.EntityPixelPerCase]
                }, false));
                if (m_y / Map.EntityPixelPerCase != (m_y + vy) / Map.EntityPixelPerCase) // Both side and up/down
                {
                    FireChangedCase(this, new MultiCaseEventArgs(new AbsCase[] {
                        m_Map[(m_x + vx) / Map.EntityPixelPerCase, (m_y + vy) / Map.EntityPixelPerCase],
                        m_Map[(m_x + vx) / Map.EntityPixelPerCase, m_y / Map.EntityPixelPerCase], m_Map[m_x / Map.EntityPixelPerCase, (m_y + vy) / Map.EntityPixelPerCase]
                    }, false));
                }
            }
            else if (m_y / Map.EntityPixelPerCase != (m_y + vy) / Map.EntityPixelPerCase) // Up/Down only
            {
                FireChangedCase(this, new MultiCaseEventArgs(new AbsCase[] {
                    m_Map[(m_x + vx) / Map.EntityPixelPerCase, (m_y + vy) / Map.EntityPixelPerCase]
                }, false));
            }
            m_x += vx;
            m_y += vy;
            FireMoved(this, new CancellableEventArgs(false));
        }
    }
}
