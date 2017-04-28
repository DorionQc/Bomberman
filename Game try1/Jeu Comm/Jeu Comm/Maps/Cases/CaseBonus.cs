using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;

namespace Jeu_Comm.Maps.Cases
{
    public enum BonusType
    {
        ExtraBomb,
        FirePower,
        RollerSkates,
        Glove,
        Kick,
        MaximumPower
    };

    public class CaseBonus : AbsCase
    {
        private byte m_TextureVariant;
        private BonusType m_Type;
        public CaseBonus(int x, int y, Map Parent, Random r) : base(x, y, Parent, false, true, false)
        {
            m_TextureVariant = 0;
            int ran = r.Next() % 100;
            if (ran < 30)
                m_Type = BonusType.ExtraBomb;
            else if (ran < 50)
                m_Type = BonusType.FirePower;
            else if (ran < 70)
                m_Type = BonusType.RollerSkates;
            else if (ran < 80)
                m_Type = BonusType.Glove;
            else if (ran < 95)
                m_Type = BonusType.Kick;
            else
                m_Type = BonusType.MaximumPower;
        }

        public CaseBonus(int x, int y, Map Parent, BonusType t) : base(x, y, Parent, false, true, false)
        {
            m_TextureVariant = 0;
            m_Type = t;
        }



        public override CaseType Type
        {
            get
            {
                return CaseType.Bonus;
            }
        }

        public BonusType BonusType
        {
            get
            {
                return m_Type;
            }
        }

        public override Image Texture
        {
            get
            {
                if (m_TextureVariant >= 19)
                    m_TextureVariant = 0;
                else
                    m_TextureVariant++;
                return TextureManager.Instance.tTextureCaseBonus[(int)m_Type, m_TextureVariant / 10];
            }
        }
    }
}
