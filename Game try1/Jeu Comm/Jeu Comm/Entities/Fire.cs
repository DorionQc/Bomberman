using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Jeu_Comm.Maps;
using Jeu_Comm.Maps.Cases;

namespace Jeu_Comm.Entities
{
    public class Fire : AbsEntity, ITexturable
    {
        int m_TextureVariant;
        int m_LifeTime;
        public Fire(int x, int y, Map m, int LifeTime, bool Registered) : base(x, y, m, Registered)
        {
            m_TextureVariant = 0;
            m_LifeTime = LifeTime;
            m_Size = 15;
        }

        public Fire(int x, int y, Map m, int LifeTime, bool Registered, int ID)
            : this(x, y, m, LifeTime, Registered)
        {
            if (ID != 0)
                m_ID = ID;
        }

        public override EntityType Type
        {
            get
            {
                return EntityType.Fire;
            }
        }

        public override void Draw(Graphics g, Rectangle r, SolidBrush b, float Width)
        {
            int rad = m_Size;
            Image bit = TextureManager.Instance.tTextureFire[m_TextureVariant / 21];
            //g.DrawImage(bit, m_x * Width, m_y * Width, Width, Width);
            g.DrawImage(bit, new Rectangle((int)((m_x - rad) * Width), (int)((m_y - rad) * Width), (int)(rad * Width * 2), (int)(rad * Width * 2)), 0, 0, bit.Width, bit.Height, GraphicsUnit.Pixel);

        }

        public override void Tick(long DeltaTime)
        {
            if (IsRegistered)
            {
                m_LifeTime -= (int)DeltaTime;
                if (m_LifeTime < 0)
                    Update();
            }
        }

        public void UpdateTexture(long DeltaTime)
        {
            m_TextureVariant += (int)DeltaTime / 5;
            if (m_TextureVariant > 39)
                m_TextureVariant %= 40;
        }

        public void Update()
        {
            AbsCase c = m_Map[m_x / 30, m_y / 30];
            if (c is CaseVide)
            {
                ((CaseVide)c).Fire = null;
            }
            else
                m_Map[m_x / 30, m_y / 30] = new CaseVide(m_x / 30, m_y / 30, m_Map);
            EntityManager.Instance.Remove(this);
        }

    }
}
