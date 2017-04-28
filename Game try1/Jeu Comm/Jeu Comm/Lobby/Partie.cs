using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Jeu_Comm.Lobby
{
    public struct Partie
    {
        public IPAddress Host;
        public string Nom;

        public Partie(string Nom, IPAddress Host)
        {
            this.Host = Host;
            this.Nom = Nom;
        }
    }
}
