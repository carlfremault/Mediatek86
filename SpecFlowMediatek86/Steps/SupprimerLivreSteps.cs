using Mediatek86.vue;
using System.Windows.Forms;
using TechTalk.SpecFlow;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mediatek86.controleur;

namespace SpecFlowMediatek86.Steps
{
    [Binding]
    public class SupprimerLivreSteps
    {
        private readonly FrmMediatek frmMediatek = new FrmMediatek(new Controle());

        [Given(@"je saisie le numero de livre (.*)")]
        public void GivenJeSaisieLeNumeroDeLivre(string valeur)
        {
            TextBox TxtValeur = (TextBox)frmMediatek.Controls["tabOngletsApplication"].Controls["tabLivres"].Controls["grpLivresRecherche"].Controls["txbLivresNumRecherche"];
            frmMediatek.Visible = true;
            TxtValeur.Text = valeur;
        }
        
        [Given(@"je clic sur le bouton pour Rechercher")]
        public void GivenJeClicSurLeBoutonPourRechercher()
        {
            Button BtnRechercher = (Button)frmMediatek.Controls["tabOngletsApplication"].Controls["tabLivres"].Controls["grpLivresRecherche"].Controls["btnLivresNumRecherche"];
            frmMediatek.Visible = true;
            BtnRechercher.PerformClick();
        }
        
        [Given(@"Je clic sur le bouton pour Supprimer")]
        public void GivenJeClicSurLeBoutonPourSupprimer()
        {
            Button BtnSupprimer = (Button)frmMediatek.Controls["tabOngletsApplication"].Controls["tabLivres"].Controls["grpGestionLivres"].Controls["btnSupprLivre"];
            frmMediatek.Visible = true;
            BtnSupprimer.PerformClick();
        }
        
        [When(@"je saisis le titre du livre (.*)")]
        public void WhenJeSaisisLeTitreDuLivreTestTitre(string titre)
        {
            TextBox TxtValeur = (TextBox)frmMediatek.Controls["tabOngletsApplication"].Controls["tabLivres"].Controls["grpLivresRecherche"].Controls["txbLivresTitreRecherche"];
            frmMediatek.Visible = true;
            TxtValeur.Text = titre;
        }
        
        [Then(@"Aucune valeur ne doit être retrouvée")]
        public void ThenAucuneValeurNeDoitEtreRetrouvee()
        {
            TextBox TxtTitre = (TextBox)frmMediatek.Controls["tabOngletsApplication"].Controls["tabLivres"].Controls["grpLivresInfos"].Controls["txbLivresTitre"];
            string titreObtenu = TxtTitre.Text;
            Assert.AreEqual("", titreObtenu);
        }
    }
}
