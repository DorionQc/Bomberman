using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

using Jeu_Comm.Maps;
using Jeu_Comm.CustomEventArgs;
using Jeu_Comm.Maps.Cases;


namespace Jeu_Comm.Entities
{
    public class EntityManager
    {
        // Singleton! :D
        private List<AbsEntity> m_Entities;
        private Joueur[] m_tJoueur;
        private Map m_Map;
        private object SomeLock;
        private int m_ID;
        private List<Fire> m_DeadFire;

        private static EntityManager INSTANCE;

        public event OnFireStopHandler FireStopped;

        private EntityManager() : this(null, null, 0) { }
        private EntityManager(Joueur[] j, Map m, int NumeroJoueur)
        {
            m_Entities = new List<AbsEntity>();
            m_tJoueur = j;
            m_Map = m;
            m_ID = NumeroJoueur;
            SomeLock = new object();
            m_DeadFire = new List<Fire>();
        }

        protected void FireFireStopped(object sender, MultiFireEventArgs e)
        {
            if (FireStopped != null)
                FireStopped(sender, e);
        }

        public static EntityManager Instance
        {
            get
            {
                if (INSTANCE == null)
                    INSTANCE = new EntityManager();
                return INSTANCE;
            }
        }

        public static void InitInstance(Joueur[] j, Map m, int ID)
        {
            if (INSTANCE != null)
            {
                INSTANCE.m_Entities = new List<AbsEntity>();
                INSTANCE.m_tJoueur = j;
                INSTANCE.m_Map = m;
                INSTANCE.m_ID = ID;
            }
            INSTANCE = new EntityManager(j, m, ID);
        }

        public List<AbsEntity> Entities
        {
            get { return m_Entities; }
        }

        public Joueur[] Joueurs
        {
            get { return m_tJoueur; }
        }
        
        public Map Map
        {
            get { return m_Map; }
            set { m_Map = value; }
        }

        public AbsEntity EntityFromID(int ID)
        {
            int i = 0;
            lock (SomeLock)
            {
                while (i < m_Entities.Count && m_Entities[i].ID != ID)
                    i++;
            }
            if (i == m_Entities.Count)
                return null;
            return m_Entities[i];
        }

        public void Add(AbsEntity e)
        {
            lock (SomeLock)
            {
                m_Entities.Add(e);
            }
            for (int i = 0; i < m_Entities.Count; i++)
                if (m_Entities.Count((ent) => { return ent.ID == m_Entities[i].ID; }) > 1)
                    Console.WriteLine("L'identifiant d'entités " + m_Entities[i].ID + "est déja en cours d'utilisation");
        }

        public bool Remove(AbsEntity e)
        {
            if (e is Fire)
            {
                m_DeadFire.Add((Fire)e);
            }
            lock (SomeLock)
            {
                return m_Entities.Remove(e);
            }
        }

        public void TickPlayer(int IDPlayer, long DeltaTime, KeyWrapper Wrapper)
        {
            if (!m_tJoueur[IDPlayer].IsDead)
                m_tJoueur[IDPlayer].TickPlayer(DeltaTime, Wrapper);
        }

        public void TickEntities(long DeltaTime)
        {
            List<AbsEntity> ToUpdate = m_Entities.ToList();

            foreach (AbsEntity e in ToUpdate)
            {
                e.Tick(DeltaTime);
                if (e is ITexturable)
                    ((ITexturable)e).UpdateTexture(DeltaTime);
            }
            for (int i = 0; i < 4; i++)
                if (m_tJoueur[i] != null && m_tJoueur[i].IsDead == false)
                    m_tJoueur[i].UpdateTexture(DeltaTime);//Tick(DeltaTime);
            if (m_tJoueur[m_ID].IsDead == false)
                m_tJoueur[m_ID].Tick(DeltaTime);
            if (m_DeadFire.Count != 0)
            {
                FireFireStopped(this, new MultiFireEventArgs(m_DeadFire.ToArray(), false));
                m_DeadFire = new List<Fire>();
            }
            
        }

        public void Draw(Graphics g, Rectangle r)
        {
            float w = (float)r.Width / m_Map.NoCase;
            List<AbsEntity> ToDraw = null;
            lock (SomeLock)
            {
                ToDraw = m_Entities.ToList();
            }
            using (SolidBrush b = new SolidBrush(Color.Black))
            {
                foreach (AbsEntity e in ToDraw)
                    e.Draw(g, r, b, w / 30);
                for (int i = 0; i < 4; i++)
                    if (m_tJoueur[i] != null && m_tJoueur[i].IsDead == false)
                        m_tJoueur[i].Draw(g, r, b, w / 30);
            }
        }

    }
}
