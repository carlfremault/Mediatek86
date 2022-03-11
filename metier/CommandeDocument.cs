using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mediatek86.metier
{
    public class CommandeDocument : Commande
    {
        private readonly int nbExemplaires;
        private readonly int idSuivi;
        private readonly string libelleSuivi;

        public CommandeDocument(string id, DateTime dateCommande, double montant, int nbExemplaires, int idSuivi, string libelleSuivi) : base(id, dateCommande, montant)
        {
            this.nbExemplaires = nbExemplaires;
            this.idSuivi = idSuivi;
            this.libelleSuivi = libelleSuivi;
        }

        public int NbExemplaires { get => nbExemplaires; }
        public int IdSuivi { get => idSuivi; }
        public string LibelleSuivi { get => libelleSuivi; }
    }
}
