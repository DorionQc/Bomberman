using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

using Jeu_Comm.Maps.Cases;

namespace Jeu_Comm.Maps
{
    public class Map
    {
        private AbsCase[,] m_tCases;
        private int m_NoCase;

        private Random m_Random;

        // Nombre d'unités contenus dans une case, utilisées par les entités
        public const int EntityPixelPerCase = 30;

        public int NoCase
        {
            get { return m_NoCase; }
        }

        public Map(int Size)
        {
            m_NoCase = Size + (Size % 2 - 1);
            m_tCases = new AbsCase[Size, Size];
            m_Random = new Random();
            // Remplissage aléatoire de la carte
            for (int x = 1; x < Size - 1; x++)
                for (int y = 1; y < Size - 1; y++)
                {
                    if (x % 2 == 0 && y % 2 == 0)
                        m_tCases[x, y] = new CaseSolidWall(x, y, this);
                    else if (m_Random.Next() % 10 != 0)
                        m_tCases[x, y] = new CaseWall(x, y, this);
                    else
                        m_tCases[x, y] = new CaseVide(x, y, this);
                }

            for (int x = 0; x < Size - 1; x++)
            {
                m_tCases[x, 0] = new CaseSolidWall(x, 0, this);
                m_tCases[Size - 1, x] = new CaseSolidWall(Size - 1, x, this);
                m_tCases[Size - 1 - x, Size - 1] = new CaseSolidWall(Size - 1 - x, Size - 1, this);
                m_tCases[0, Size - 1 - x] = new CaseSolidWall(0, Size - 1 - x, this);
            }

            // Génération des coins

            int i = 1;
            m_tCases[1, 1] = new CaseVide(1, 1, this);
            m_tCases[1, m_NoCase - 2] = new CaseVide(1, m_NoCase - 2, this);
            m_tCases[m_NoCase - 2, 1] = new CaseVide(m_NoCase - 2, 1, this);
            m_tCases[m_NoCase - 2, m_NoCase - 2] = new CaseVide(m_NoCase - 2, m_NoCase - 2, this);
            bool tld = true, tlr = true, trd = true, trl = true, blu = true, blr = true, bru = true, brl = true;
            while (i < 4 && (tld || tlr || trd || trl || blu || blr || bru || brl))
            {
                if (tld)
                {
                    m_tCases[1, 1 + i] = new CaseVide(1, 1 + i, this);
                    if (m_Random.Next() % 7 == 0) tld = false;
                }
                if (tlr)
                {
                    m_tCases[1 + i, 1] = new CaseVide(1 + i, 1, this);
                    if (m_Random.Next() % 7 == 0) tlr = false;
                }
                if (trd)
                {
                    m_tCases[m_NoCase - 2, 1 +  i] = new CaseVide(m_NoCase - 2, 1 +  i, this);
                    if (m_Random.Next() % 7 == 0) trd = false;
                }
                if (trl)
                {
                    m_tCases[m_NoCase - 2 - i, 1] = new CaseVide(m_NoCase - 2 - i, 1, this);
                    if (m_Random.Next() % 7 == 0) trl = false;
                }
                if (blu)
                {
                    m_tCases[1, m_NoCase - 2 - i] = new CaseVide(1, m_NoCase - 2 - i, this);
                    if (m_Random.Next() % 7 == 0) blu = false;
                }
                if (blr)
                {
                    m_tCases[1 + i, m_NoCase - 2] = new CaseVide(1 + i, m_NoCase - 2, this);
                    if (m_Random.Next() % 7 == 0) blr = false;
                }
                if (bru)
                {
                    m_tCases[m_NoCase - 2, m_NoCase - 2 - i] = new CaseVide(m_NoCase - 2, m_NoCase - 2 - i, this);
                    if (m_Random.Next() % 7 == 0) bru = false;
                }
                if (brl)
                {
                    m_tCases[m_NoCase - 2 - i, m_NoCase - 2] = new CaseVide(m_NoCase - 2 - i, m_NoCase - 2, this);
                    if (m_Random.Next() % 7 == 0) brl = false;
                }
                i++;
            }
        }

        // Indexeur pour aller chercher facilement des cases
        public AbsCase this[int x, int y]
        {
            get
            {
                if (x >= 0 && y >= 0 && x < m_NoCase && y < m_NoCase)
                    return m_tCases[x, y];
                throw new IndexOutOfRangeException(string.Format("{0}, {1} is outside the range 0-{2}", x, y, m_NoCase - 1));
            }
            set
            {
                if (x >= 0 && y >= 0 && x < m_NoCase && y < m_NoCase)
                {
                    if (m_tCases[x, y].Fire != null)
                        m_tCases[x, y].Fire = null;
                    if (m_tCases[x, y] is CaseVide)
                        if (((CaseVide)m_tCases[x, y]).ContainsBomb)
                            ((CaseVide)m_tCases[x, y]).Bomb = null;
                    m_tCases[x, y] = value;
                }
            }
        }
        public void Draw(Graphics g, Rectangle r)
        {
            float w = (float)r.Width / m_NoCase;
            using (SolidBrush b = new SolidBrush(Color.Black)) // La brosse est passée simplement pour éviter d'avoir à toujours en créer. Sa couleur a peu d'importance
            {                                                  // et changera à chaque utilisation
                foreach (AbsCase c in m_tCases)
                    c.Draw(g, r, b, w);
            }
        }

        /// <summary>
        /// Tries to place a bonus at the specified spot in the map
        /// </summary>
        /// <param name="x">X coords of the case to place a bonus in</param>
        /// <param name="y">Y coords of the case to place a bonus in</param>
        /// <returns>True if a bonus was place, false otherwise</returns>
        public bool MakeRandomBonus(int x, int y)
        {
            if (x < 0 || y < 0 || x >= m_NoCase || y >= m_NoCase)
                return false;
            if (!(m_tCases[x, y] is CaseWall))
                return false;

            if (m_Random.Next() % 3 != 0)
                return false;

            m_tCases[x, y] = new CaseBonus(x, y, this, m_Random);
            return true;

        }

        // Size, byte[Size, Size]
        public bool FromByteArray(byte[] Data, ref int Position)
        {
            if (Data.Length < Position + 1)
                return false;
            m_NoCase = Data[Position++];
            if (Data.Length < Position + 1 + m_NoCase * m_NoCase)
                return false;
            for (int x = 0; x < m_NoCase; x++)
            {
                for (int y = 0; y < m_NoCase; y++)
                {
                    if (Data[Position] == 2)
                        m_tCases[x, y] = new CaseSolidWall(x, y, this);
                    else if (Data[Position] == 1)
                        m_tCases[x, y] = new CaseWall(x, y, this);
                    else
                       m_tCases[x, y] = new CaseVide(x, y, this);
                    Position++;
                }
            }
            return true;
        }

        public byte[] ToByteArray()
        {
            byte[] ret = new byte[m_NoCase * m_NoCase + 1];
            ret[0] = (byte)m_NoCase;
            for (int x = 0; x < m_NoCase; x++)
            {
                for (int y = 0; y < m_NoCase; y++)
                {
                    ret[1 + x * m_NoCase + y] = (byte)m_tCases[x, y].Type;
                }
            }
            return ret;
        }

    }
}
