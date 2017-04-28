using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Jeu_Comm.Entities;
using Jeu_Comm.Maps;
using Jeu_Comm.Maps.Cases;
using Jeu_Comm.CustomEventArgs;

namespace Jeu_Comm.Entities
{
    public class Joueur : AbsMoveableEntity, ITexturable
    {

        private int m_TextureVariant;
        private int m_BonusExtraBomb;
        private int m_BonusFirePower;
        private int m_BonusRollerSkates;
        private bool m_BonusGlove;
        private bool m_BonusKick;

        private int m_BombsLeft;

        private bool m_CarryingBomb;

        private Bomb m_Bomb;

        private byte m_IDJoueur;

        public event OnDropBombHandler DroppedBomb;
        public event OnGetBonusHandler PickedBonus;
        public event OnBombExplodeHandler BombExploded;
        public event OnKickBombHandler KickedBomb;
        public event OnPickBombHandler PickedBomb;
        public event OnShootBombHandler ShotBomb;
        public event OnGenericBlockEventHandler BombPlacedBonus;
        public event OnGenericMultiblockEventHandler BombBrokeBlocks;

        protected void FireBombBrokeBlocks(object sender, MultiCaseEventArgs e)
        {
            if (BombBrokeBlocks != null)
                BombBrokeBlocks(sender, e);
        }
        protected void FireBombPlacedBonus(object sender, CaseEventArgs e)
        {
            if (BombPlacedBonus != null)
                BombPlacedBonus(sender, e);
        }
        protected void FireShotBomb(object sender, ShootBombEventArgs e)
        {
            if (ShotBomb != null)
                ShotBomb(sender, e);
        }
        protected void FirePickedBomb(object sender, CaseEventArgs e)
        {
            if (PickedBomb != null)
                PickedBomb(sender, e);
        }
        protected void FireKickedBomb(object sender, KickedBombEventArgs e)
        {
            if (KickedBomb != null)
                KickedBomb(sender, e);
        }
        public void FireDroppedBomb(object sender, CaseEventArgs e)
        {
            if (DroppedBomb != null)
                DroppedBomb(sender, e);
        }
        protected void FirePickedBonus(object sender, CaseEventArgs e)
        {
            if (PickedBonus != null)
                PickedBonus(sender, e);
        }

        public Joueur(int x, int y, Map m, byte ID) : base(x, y, m, true)
        {
            m_Size = 10;
            m_TextureVariant = 0;

            m_BonusRollerSkates = 1;
            m_BonusKick = false;
            m_BonusGlove = true;
            m_BonusFirePower = 3;
            m_BonusExtraBomb = 1;

            m_BombsLeft = 1;
            m_CarryingBomb = false;

            m_IDJoueur = ID;

            this.ChangedCase += Joueur_ChangedCase;
            this.Collided += Joueur_Collided;
            this.Moved += Joueur_Moved;
            this.Died += Die;
        }

        public void Die(object sender, CancellableEventArgs e)
        {
            m_Dead = true;
            //MessageBox.Show("Ayyyyyyyy!! I'm dead!");
        }

        private void Joueur_Moved(object sender, CancellableEventArgs e)
        {
            if (m_CarryingBomb && m_Bomb != null)
            {
                m_Bomb.X = m_x;
                m_Bomb.Y = m_y;
            }
        }

        private void Joueur_Collided(object sender, BlockCollisionEventArgs e)
        {
            if (!m_BonusKick)
                return;
            CollisionInfo inf;
            bool[] tb = new bool[e.Info.Count];
            for (int i = 0; i < e.Info.Count; i++)
            {
                inf = e.Info[i];
                if (inf.Case is CaseVide && ((CaseVide)inf.Case).ContainsBomb)
                {
                    if (inf.Side != CollisionSide.None)
                    {
                        Bomb b = ((CaseVide)inf.Case).Bomb;
                        if (((CaseVide)inf.Case).Bomb.Kick(inf.Side))
                        {
                            FireKickedBomb(this, new KickedBombEventArgs(b, inf.Side, false));
                            e.Info.Remove(inf);
                        }
                    }
                }
            }
        }

        private void Joueur_ChangedCase(object sender, MultiCaseEventArgs e)
        {
            foreach (AbsCase c in e.Cases)
            {
                if (c.IsBreaking)
                {
                    FireDied(this, new CancellableEventArgs(false));
                }
                else if (c is CaseBonus)
                {
                    PickBonus((CaseBonus)c);
                }
            }
        }

        public void PickBonus(CaseBonus c)
        {
            switch (((CaseBonus)c).BonusType)
            {
                case BonusType.ExtraBomb:
                    if (m_BonusExtraBomb < 9)
                    {
                        m_BonusExtraBomb++;
                        m_BombsLeft++;
                    }
                    break;
                case BonusType.FirePower:
                    if (m_BonusFirePower < 9)
                        m_BonusFirePower++;
                    break;
                case BonusType.Glove:
                    m_BonusGlove = true;
                    break;
                case BonusType.Kick:
                    m_BonusKick = true;
                    break;
                case BonusType.MaximumPower:
                    m_BonusFirePower = 9;
                    break;
                case BonusType.RollerSkates:
                    if (m_BonusRollerSkates < 9)
                        m_BonusRollerSkates++;
                    break;
            }
            FirePickedBonus(this, new CaseEventArgs(c, false));
            m_Map[c.X, c.Y] = new CaseVide(c.X, c.Y, m_Map);
        }

        public byte PlayerID
        {
            get { return m_IDJoueur; }
        }

        public Bomb Bomb
        {
            get { return m_Bomb; }
            set { m_Bomb = value; }
        }

        public int BombsLeft
        {
            get { return m_BombsLeft; }
            set { m_BombsLeft = value; }
        }

        public override EntityType Type
        {
            get
            {
                return EntityType.Joueur;
            }
        }

        public override void Tick(long DeltaTime)
        {
            base.Tick(DeltaTime);
            m_Velx *= (float)Math.Pow(0.9f, DeltaTime / 8);
            m_Vely *= (float)Math.Pow(0.9f, DeltaTime / 8);
            //UpdateTexture(DeltaTime);
        }

        public void UpdateTexture(long DeltaTime)
        {
            m_TextureVariant += (int)DeltaTime / 5;
            if (m_TextureVariant > 79)
                m_TextureVariant %= 80;
        }

        public void TickPlayer(long DeltaTime, KeyWrapper Wrapper)
        {
            if ((Wrapper.State & KeyState.Up) > 0)
                m_Vely -= (1.1f + m_Vely + m_BonusRollerSkates / 20.0f) / 15;
            if ((Wrapper.State & KeyState.Down) > 0)
                m_Vely += (1.1f - m_Vely + m_BonusRollerSkates / 20.0f) / 15;
            if ((Wrapper.State & KeyState.Left) > 0)
                m_Velx -= (1.1f + m_Velx + m_BonusRollerSkates / 20.0f) / 15;
            if ((Wrapper.State & KeyState.Right) > 0)
                m_Velx += (1.1f - m_Velx + m_BonusRollerSkates / 20.0f) / 15;
            if ((Wrapper.State & KeyState.Space) > 0)
            {
                if (m_CarryingBomb)
                     ShootBomb(m_x / 30, m_y / 30, m_Velx, m_Vely);
                else if (m_Map[m_x / 30, m_y / 30] is CaseVide && ((CaseVide)m_Map[m_x / 30, m_y / 30]).ContainsBomb)
                    PickupBomb(m_x / 30, m_y / 30);
                else
                    DropBomb(m_x / 30, m_y / 30, true);
            }
        }

        public bool ShootBomb(int x, int y, float Velx, float Vely)
        {
            CollisionSide Side = 0;
            if (m_Velx > 0)
            {
                Side = CollisionSide.Right;
            }
            else if (m_Velx < 0)
            {
                Side = CollisionSide.Left;
            }
            else if (m_Vely > 0)
            {
                Side = CollisionSide.Down;
            }
            else if (m_Vely < 0)
            {
                Side = CollisionSide.Up;
            }
            else
                return false;
            return ShootBomb(Side);

        }

        public bool ShootBomb(CollisionSide Side)
        {
            if (m_Bomb == null) return false;
            switch (Side)
            {
                case CollisionSide.Up: m_Bomb.VelY = -1; break;
                case CollisionSide.Down: m_Bomb.VelY = 1; break;
                case CollisionSide.Left: m_Bomb.VelX = -1; break;
                case CollisionSide.Right: m_Bomb.VelX = 1; break;
                default: return false;
            }
            FireShotBomb(this, new ShootBombEventArgs(this, m_Bomb, Side, false));
            m_CarryingBomb = false;
            m_Bomb.Carried = false;
            m_Bomb.Flying = true;
            m_Bomb.Z = 0;
            m_Bomb = null;
            return true;
        }

        public bool PickupBomb(int x, int y)
        {
            if (x >= m_Map.NoCase || y >= m_Map.NoCase)
                return false;
            AbsCase c = m_Map[x, y];
            CaseVide cv;
            if (!m_BonusGlove)
                return false;
            if (!(c is CaseVide))
                return false;
            cv = (CaseVide)c;
            if (!cv.ContainsBomb)
                return false;
            if (cv.Bomb.Owner != this)
                return false;
            FirePickedBomb(this, new CaseEventArgs(m_Map[x, y], false));
            m_Bomb = cv.Bomb;
            m_Bomb.Z = 40;
            m_Map[x, y] = new CaseVide(x, y, m_Map);
            m_CarryingBomb = true;
            m_Bomb.Carried = true;
            
            return true;
        }

        public bool DropBomb(int x, int y, bool Register, int ID = 0)
        {
            if (x < 0 || y < 0 || x >= m_Map.NoCase || y >= m_Map.NoCase)
                return false;
            CaseVide cv;
            if (Register)
            {
                if (!(m_Map[x, y] is CaseVide))
                    return false;
                if (m_BombsLeft <= 0)
                    return false;
                cv = (CaseVide)m_Map[x, y];
                if (cv.ContainsBomb)
                    return false;
            }
            else
            {
                if (!(m_Map[x, y] is CaseVide))
                    m_Map[x, y] = new CaseVide(x, y, m_Map);
                cv = (CaseVide)m_Map[x, y];
            }
            cv.Bomb = new Bomb(x * 30 + 15, y * 30 + 15, m_Map, this, 3000, m_BonusFirePower, Register, ID);
            if (cv.IsBreaking)
                cv.Bomb.LifeTime = 150;
            m_BombsLeft--;
            EntityManager.Instance.Add(cv.Bomb);
            FireDroppedBomb(this, new CaseEventArgs(m_Map[x, y], false));
            cv.Bomb.Explode += Bomb_Exploded;
            cv.Bomb.PlaceBonus += FireBombPlacedBonus;
            cv.Bomb.BreakBlocks += FireBombBrokeBlocks;
            return true;
        }

        private void Bomb_Exploded(object sender, CaseEventArgs e)
        {
            if (BombExploded != null)
                BombExploded(sender, e);
        }

        public override void Draw(Graphics g, Rectangle r, SolidBrush b, float w)
        {
            //b.Color = Color.Gold;
            int rad = m_Size + 5;
            Image bit;
            if (Math.Abs(m_Velx) > Math.Abs(m_Vely)) // Left or right
            {
                if (m_Velx > 0)
                    bit = TextureManager.Instance.tTexturePlayerRight[m_IDJoueur, m_TextureVariant / 20];
                else
                    bit = TextureManager.Instance.tTexturePlayerLeft[m_IDJoueur, m_TextureVariant / 20];
            }
            else // Up or down
            {
                if (m_Vely > 0)
                    bit = TextureManager.Instance.tTexturePlayerDown[m_IDJoueur, m_TextureVariant / 20];
                else
                    bit = TextureManager.Instance.tTexturePlayerUp[m_IDJoueur, m_TextureVariant / 20];
            }
            if (m_Velx == 0 && m_Vely == 0)
                bit = TextureManager.Instance.tTexturePlayerDown[m_IDJoueur, 0];
            //g.FillRectangle(b, (m_x - m_Radius) * w, (m_y - m_Radius) * w, m_Radius * 2 * w, m_Radius * 2 * w);
            //g.DrawImage(bit, (m_x - rad) * w, (m_y - rad - 20) * w, rad * w * 2, rad * w * 3);
            g.DrawImage(bit, (m_x - rad) * w, (m_y - rad - 20) * w, rad * w * 2, rad * w * 3);
            //g.DrawString(string.Format("{0}, {1}\r\n{2}, {3}", m_x, m_y, m_Velx, m_Vely), new Font("Arial", 12), b, 10, 45 * (m_IDJoueur));
        }
    }
}
