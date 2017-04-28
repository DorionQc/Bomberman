using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;

using Jeu_Comm.Maps.Cases;
using Jeu_Comm.Maps;
using Jeu_Comm.CustomEventArgs;

namespace Jeu_Comm.Entities
{
    public class Bomb : AbsMoveableEntity, ITexturable
    {
        private int m_TextureVariant;
        private int m_LifeTime;
        private Joueur m_Owner;
        private int m_Power;
        private int m_z;

        private bool m_Carried;
        private bool m_Flying;

        public event OnBombExplodeHandler Explode;
        public event OnGenericMultiblockEventHandler BreakBlocks;
        public event OnGenericBlockEventHandler PlaceBonus;

        public Bomb(int x, int y, Map m, Joueur Owner, int LifeTime, int Power, bool Register, int ID) : base(x, y, m, Register)
        {
            m_Registered = Register;
            m_Owner = Owner;
            m_LifeTime = LifeTime;
            m_Power = Power;
            m_Size = 10;
            m_TextureVariant = 0;
            m_z = 0;
            m_Carried = false;
            m_Flying = false;

            if (ID != 0)
                m_ID = ID;

            Collided += Bomb_Collided;
        }

        public Bomb(int x, int y, Map m, Joueur Owner, int LifeTime, int Power, bool Register)
            : this(x, y, m, Owner, LifeTime, Power, Register, 0)
        { }

        public void FireBreakBlocks(object sender, MultiCaseEventArgs e)
        {
            if (BreakBlocks != null)
                BreakBlocks(sender, e);
        }

        public void FirePlacedBonus(object sender, CaseEventArgs e)
        {
            if (PlaceBonus != null)
                PlaceBonus(sender, e);
        }

        public void FireExplode(object sender, CaseEventArgs e)
        {
            if (Explode != null)
                Explode(sender, e);
        }

        /// <summary>
        /// La bombe se fait botter
        /// </summary>
        /// <param name="side"></param>
        /// <returns></returns>
        public bool Kick(CollisionSide side)
        {
            bool ret = false;
            int mx = m_x / 30, my = m_y / 30;
            switch (side)
            {
                case CollisionSide.Left:
                    if (!m_Map[mx + 1, my].IsSolid)
                    {
                        ret = true;
                        m_Velx += 1;
                    }
                    break;
                case CollisionSide.Right:
                    if (!m_Map[mx - 1, my].IsSolid)
                    {
                        ret = true;
                        m_Velx -= 1;
                    }
                    break;
                case CollisionSide.Up:
                    if (!m_Map[mx, my + 1].IsSolid)
                    {
                        ret = true;
                        m_Vely += 1;
                    }
                    break;
                case CollisionSide.Down:
                    if (!m_Map[mx, my - 1].IsSolid)
                    {
                        ret = true;
                        m_Vely -= 1;
                    }
                    break;
            }
            if (ret && m_Map[mx, my] is CaseVide)
            {
                ((CaseVide)m_Map[mx, my]).Bomb = null;
            }
            return ret;
        }

        /// <summary>
        /// La bombe entre en collision avec quelque chose
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Bomb_Collided(object sender, CustomEventArgs.BlockCollisionEventArgs e)
        {
            int mx, my;
            if (m_Flying)
            {
                mx = e.Info[0].Case.X;
                my = e.Info[0].Case.Y;
            }
            else
            {
                mx = m_x / Map.EntityPixelPerCase;
                my = m_y / Map.EntityPixelPerCase;
            }
            if (!(e.Info[0].Case is CaseVide))
                m_Map[mx, my] = new CaseVide(mx, my, m_Map);
            SettleAt(mx, my);
        }

        /// <summary>
        /// La bombe se place quelque part
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void SettleAt(int x, int y)
        {
            if (!(m_Map[x, y] is CaseVide))
                m_Map[x, y] = new CaseVide(x, y, m_Map);
            CaseVide cv = (CaseVide)m_Map[x, y];
            if (cv.IsBreaking)
                m_LifeTime = 150;
            ((CaseVide)m_Map[x, y]).Bomb = this;
            m_x = x * Map.EntityPixelPerCase + m_Size;
            m_y = y * Map.EntityPixelPerCase + m_Size;
            m_Velx = 0;
            m_Vely = 0;
            m_z = 0;
            m_Owner.FireDroppedBomb(m_Owner, new CaseEventArgs(m_Map[x, y], false));
            m_Flying = false;
            m_Carried = false;
        }

        public bool Carried
        {
            get { return m_Carried; }
            set { m_Carried = value; }
        }

        public bool Flying
        {
            get { return m_Flying; }
            set { m_Flying = value; }
        }

        public int Z
        {
            get { return m_z; }
            set { m_z = value; }
        }

        public Joueur Owner
        {
            get { return m_Owner; }
            set { m_Owner = value; }
        }

        public int LifeTime
        {
            get { return m_LifeTime; }
            set { m_LifeTime = value; }
        }

        public override EntityType Type
        {
            get
            {
                return EntityType.Bomb;
            }
        }

        /// <summary>
        /// Méthode de dessin
        /// </summary>
        /// <param name="g"></param>
        /// <param name="r"></param>
        /// <param name="b"></param>
        /// <param name="Width"></param>
        public override void Draw(Graphics g, Rectangle r, SolidBrush b, float Width)
        {
            int rad = m_Size + 5;
            Image bit = TextureManager.Instance.tTextureBomb[m_Owner.PlayerID, (m_TextureVariant / 20) % 4];
            //g.DrawImage(bit, m_x * Width, m_y * Width, Width, Width);
            if (m_Velx == 0 && m_Vely == 0 && !m_Carried)
                g.DrawImage(bit, (m_x - m_x % 30) * Width, (m_y - m_y % 30) * Width, rad * 2 * Width, rad * 2 * Width);
            else
                g.DrawImage(bit, (m_x - 15) * Width, (m_y - 15 - m_z) * Width, rad * 2 * Width, rad * 2 * Width);
            /*if (m_Velx == 0 && m_Vely == 0 && !m_Carried)
                g.DrawImage(bit, new Rectangle((int)((m_x - m_x % 30) * Width), (int)((m_y - m_y % 30) * Width), (int)(rad * 2 * Width), (int)(rad * 2 * Width)),
                    0, 0, bit.Width, bit.Height, GraphicsUnit.Pixel, TextureManager.PlayerColorAttribute[0]);
            else
                g.DrawImage(bit, new Rectangle((int)((m_x - 15) * Width), (int)((m_y - 15 - m_z) * Width), (int)(rad * 2 * Width), (int)(rad * 2 * Width)),
                    0, 0, bit.Width, bit.Height, GraphicsUnit.Pixel, TextureManager.PlayerColorAttribute[0]);*/

        }

        /// <summary>
        /// Appelée par l'EntityManager. Fait ce qui est nécessaire
        /// </summary>
        /// <param name="DeltaTime"></param>
        public override void Tick(long DeltaTime)
        {
            if (!m_Carried && !m_Flying)
            {
                base.Tick(DeltaTime);
            }
            else if (m_Flying)
            {
                TickFlying(DeltaTime);
            }
            if (m_z == 0 && m_Flying == false && IsRegistered)
            {
                // Décrémente sa durée de vie
                m_LifeTime -= (int)DeltaTime;
                if (m_LifeTime < 0)
                    Update();
            }
        }

        /// <summary>
        /// Change la texture de la bombe
        /// </summary>
        /// <param name="DeltaTime"></param>
        public void UpdateTexture(long DeltaTime)
        {
            m_TextureVariant += (int)DeltaTime / 5;
            if (m_TextureVariant > 79)
                m_TextureVariant %= 80;
        }

        /// <summary>
        /// Explosion! BOOM
        /// </summary>
        public void Update()
        {
            
            int i = 1, x, y, mx = m_x / Map.EntityPixelPerCase, my = m_y / Map.EntityPixelPerCase;
            CaseEventArgs e = new CaseEventArgs(m_Map[mx, my], false);
            FireExplode(this, e);
            if (e.Cancelled)
                return;
            m_Owner.BombsLeft++;
            if (m_Registered)
            {
                bool bLeft = true, bRight = true, bUp = true, bDown = true;

                List<AbsCase> BrokenBlocks = new List<AbsCase>();
                while (i <= m_Power && (bRight || bUp || bLeft || bDown))
                {
                    if (bRight)
                    {
                        x = mx + i;
                        if (x < m_Map.NoCase && m_Map[x, my].IsBreakable)
                        {
                            if (!SetFire(x, my, BrokenBlocks, m_Registered))
                                bRight = false;
                        }
                        else
                            bRight = false;
                    }
                    if (bLeft)
                    {
                        x = mx - i;
                        if (x >= 0 && m_Map[x, my].IsBreakable)
                        {
                            if (!SetFire(x, my, BrokenBlocks, m_Registered))
                                bLeft = false;
                        }
                        else
                            bLeft = false;
                    }
                    if (bUp)
                    {
                        y = my + i;
                        if (y < m_Map.NoCase && m_Map[mx, y].IsBreakable)
                        {
                            if (!SetFire(mx, y, BrokenBlocks, m_Registered))
                                bUp = false;
                        }
                        else
                            bUp = false;
                    }
                    if (bDown)
                    {
                        y = my - i;
                        if (y >= 0 && m_Map[mx, y].IsBreakable)
                        {
                            if (!SetFire(mx, y, BrokenBlocks, m_Registered))
                                bDown = false;
                        }
                        else
                            bDown = false;
                    }
                    i++;
                }

                SetFire(mx, my, BrokenBlocks, m_Registered);
                FireBreakBlocks(this, new MultiCaseEventArgs(BrokenBlocks.ToArray(), false));
            }
            ((CaseVide)m_Map[mx, my]).Bomb = null;
            EntityManager.Instance.Remove(this);


        }

        /// <summary>
        /// Changes a block to fire
        /// </summary>
        /// <param name="x">X coord (in map coords) of the block</param>
        /// <param name="y">Y coord (in map coords) of the block</param>
        /// <returns>True if the block was changed and you can keep updating other blocks.
        /// False if you must stop updating blocks.</returns>
        private bool SetFire(int x, int y, List<AbsCase> BrokenBlocks, bool Registered)
        {
            if (x < 0 || y < 0 || x >= m_Map.NoCase || y >= m_Map.NoCase)
                return false;
            bool ret = false;
            AbsCase c = m_Map[x, y];
            if (!c.IsBreakable)
                return false;
            if (c is CaseVide)
            {
                if (((CaseVide)c).ContainsBomb)
                    ((CaseVide)c).Bomb.LifeTime = 150;
            }

            if (c is CaseWall)
            {
                if (!m_Map.MakeRandomBonus(x, y))
                    Ignite(x, y, m_Map, 600, BrokenBlocks, Registered);
                else
                    FirePlacedBonus(this, new CaseEventArgs(m_Map[x, y], false));
            }
            else
            {
                Ignite(x, y, m_Map, 600, BrokenBlocks, Registered);
                if (c.LetsFireThrough)
                {
                    ret = true;
                }
            }

            return ret;
        }

        /// <summary>
        /// Brise une case
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="m"></param>
        /// <param name="LifeTime"></param>
        /// <param name="BrokenBlocks"></param>
        /// <param name="Register"></param>
        /// <param name="EntityID"></param>
        public static void Ignite(int x, int y, Map m, int LifeTime, List<AbsCase> BrokenBlocks, bool Register, int EntityID = 0)
        {
            Fire f = new Fire(x * Map.EntityPixelPerCase + Map.EntityPixelPerCase / 2, y * Map.EntityPixelPerCase + Map.EntityPixelPerCase / 2, m, 600, Register, EntityID);
            m[x, y].Fire = f;
            if (BrokenBlocks != null)
                BrokenBlocks.Add(m[x, y]);
            EntityManager.Instance.Add(f);
        }








        // Collisions dans les airs
        #region Flying Collisions

        private void TickFlying(long DeltaTime)
        {
            float dt = DeltaTime / 2;
            int vx = (int)(m_Velx * dt);
            int vy = (int)(m_Vely * dt);
            List<CollisionInfo> CollisionInfoReturned = CheckFlyingCollision(m_x, m_y, vx, vy);
            CollisionSide Side = 0;
            CollisionInfo Target = new CollisionInfo(CollisionSide.None, null);
            foreach (CollisionInfo c in CollisionInfoReturned)
            {
                Side |= c.Side;
                if (c.Case != null)
                {
                    if (c.Case.IsBreaking)
                    {
                        FireDied(this, new CancellableEventArgs(false));
                    }
                    Target = c;
                }
                else if (c.Case == null && c.Side != 0)
                {
                    switch (Side)
                    {
                        case CollisionSide.Up:
                            m_y = m_Map.NoCase * Map.EntityPixelPerCase - m_Size;
                            break;
                        case CollisionSide.Down:
                            m_y = m_Size;
                            break;
                        case CollisionSide.Left:
                            m_x = m_Map.NoCase * Map.EntityPixelPerCase - m_Size;
                            break;
                        case CollisionSide.Right:
                            m_x = m_Size;
                            break;
                    }
                }
            }

            if (Side == CollisionSide.None)
            {
                FireMoved(this, new CancellableEventArgs(false));
            }
            else
            {
                if (Target.Case != null && !Target.Case.IsSolid)
                {
                    BlockCollisionEventArgs e = new BlockCollisionEventArgs(Target.Case.X, Target.Case.Y, new List<CollisionInfo> { new CollisionInfo(Target.Side, Target.Case)}, false);
                    FireCollided(this, e);
                }
                if ((Side & (CollisionSide.Left | CollisionSide.Right)) > 0)
                {
                    //m_Velx = 0;
                    vx = 0;
                }
                if ((Side & (CollisionSide.Up | CollisionSide.Down)) > 0)
                {
                    //m_Vely = 0;
                    vy = 0;
                }
                if (vx != 0 || vy != 0)
                    FireMoved(this, new CancellableEventArgs(false));

            }

            // Détection de mouvement
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

            if (Math.Abs(m_Velx) < 0.01f)
                m_Velx = 0;
            if (Math.Abs(m_Vely) < 0.01f)
                m_Vely = 0;
        }

        private List<CollisionInfo> CheckFlyingCollision(int x, int y, float velx, float vely)
        {

            List<CollisionInfo> ret = new List<CollisionInfo>(2);
            int i = 0, n = 0;
            int xm = x + (int)velx - m_Size; // top left
            int ym = y + (int)vely - m_Size;
            int rad2 = m_Size + m_Size;
            int oldx = x - m_Size;
            int oldy = y - m_Size;
            AbsCase[] Cases = GetCasesIn(x + (int)velx, y + (int)vely);


            if (xm < 0) // TODO - Faire que la bombe loop autour de la map
                ret.Add(new CollisionInfo(CollisionSide.Left, null));
            else if (xm + rad2 >= m_Map.NoCase * Map.EntityPixelPerCase)
                ret.Add(new CollisionInfo(CollisionSide.Right, null));
            if (ym < 0)
                ret.Add(new CollisionInfo(CollisionSide.Up, null));
            else if (ym + rad2 >= m_Map.NoCase * Map.EntityPixelPerCase)
                ret.Add(new CollisionInfo(CollisionSide.Down, null));

            for (; i < Cases.Length; i++)
            {
                if (!Cases[i].IsSolid) // Vérifier les cases pas solides
                    n++;
                //if (Cases[i] is CaseVide && Cases[i].IsBreaking) // Exploser la bombe
                //    Update();
            }
            if (n == 0)
            {
                if (ret.Count > 0)
                    i = 0;
                return ret;
            }
            Cases = Cases.Where((ca) => ca.IsSolid == false).OrderBy((a) => Math.Abs(xm - a.X * Map.EntityPixelPerCase) + Math.Abs(ym - a.Y * Map.EntityPixelPerCase)).ToArray();
            // Ignorer tout cela et trouver la case dans laquelle on tombe

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
            // Retourner la case dans laquelle on tombe.
            return ret;
        }

        #endregion
    }
}
