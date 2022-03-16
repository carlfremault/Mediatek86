using Mediatek86.vue;
using System.Windows.Forms;
using TechTalk.SpecFlow;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mediatek86.controleur;

namespace SpecFlowMediatek86.Steps
{
    [Binding]
    public class AjouterLivrePuisChercherSteps
    {
        private readonly FrmMediatek frmMediatek = new FrmMediatek(new Controle());

        [Given(@"Je clic sur le bouton ajouter")]
        public void GivenJeClicSurLeBoutonAjouter()
        {
            Button BtnAjouter = (Button)frmMediatek.Controls["tabOngletsApplication"].Controls["tabLivres"].Controls["grpGestionLivres"].Controls["btnAjoutLivre"];
            frmMediatek.Visible = true;
            BtnAjouter.PerformClick();
        }
        
        [Given(@"Je saisis le numéro de document (.*)")]
        public void GivenJeSaisisLeNumeroDeDocument(string numeroLivre)
        {
            TextBox TxtValeur = (TextBox)frmMediatek.Controls["tabOngletsApplication"].Controls["tabLivres"].Controls["grpLivresInfos"].Controls["txbLivresNumero"];
            frmMediatek.Visible = true;
            TxtValeur.Text = numeroLivre;
        }
        
        [Given(@"Je saisis le titre (.*)")]
        public void GivenJeSaisisLeTitre(string titreLivre)
        {
            TextBox TxtValeur = (TextBox)frmMediatek.Controls["tabOngletsApplication"].Controls["tabLivres"].Controls["grpLivresInfos"].Controls["txbLivresTitre"];
            frmMediatek.Visible = true;
            TxtValeur.Text = titreLivre;
        }
        
        [Given(@"Je saisis l'auteur (.*)")]
        public void GivenJeSaisisLAuteur(string auteurLivre)
        {
            TextBox TxtValeur = (TextBox)frmMediatek.Controls["tabOngletsApplication"].Controls["tabLivres"].Controls["grpLivresInfos"].Controls["txbLivresAuteur"];
            frmMediatek.Visible = true;
            TxtValeur.Text = auteurLivre;
        }
        
        [Given(@"Je saisis le genre (.*)")]
        public void GivenJeSaisisLeGenre(string genreLivre)
        {
            ComboBox CbxGenre = (ComboBox)frmMediatek.Controls["tabOngletsApplication"].Controls["tabLivres"].Controls["grpLivresInfos"].Controls["cbxInfosLivresGenres"];
            frmMediatek.Visible = true;
            CbxGenre.SelectedIndex = CbxGenre.FindString(genreLivre);
        }
        
        [Given(@"Je saisis le public (.*)")]
        public void GivenJeSaisisLePublic(string publicLivre)
        {
            ComboBox CbxPublic = (ComboBox)frmMediatek.Controls["tabOngletsApplication"].Controls["tabLivres"].Controls["grpLivresInfos"].Controls["cbxInfosLivresPublics"];
            frmMediatek.Visible = true;
            CbxPublic.SelectedIndex = CbxPublic.FindString(publicLivre);
        }
        
        [Given(@"Je saisis le rayon (.*)")]
        public void GivenJeSaisisLeRayon(string rayonLivre)
        {
            ComboBox CbxRayon = (ComboBox)frmMediatek.Controls["tabOngletsApplication"].Controls["tabLivres"].Controls["grpLivresInfos"].Controls["cbxInfosLivresRayons"];
            frmMediatek.Visible = true;
            CbxRayon.SelectedIndex = CbxRayon.FindString(rayonLivre);
        }
        
        [Given(@"Je clic sur Enregistrer")]
        public void GivenJeClicSurEnregistrer()
        {
            Button BtnEnregistrer = (Button)frmMediatek.Controls["tabOngletsApplication"].Controls["tabLivres"].Controls["grpLivresInfos"].Controls["btnEnregistrerLivre"];
            frmMediatek.Visible = true;
            BtnEnregistrer.PerformClick();
        }
        
        [When(@"Je saisis le titre (.*)")]
        public void WhenJeSaisisLeTitre(string titreRecherche)
        {
            TextBox TxtRecherche = (TextBox)frmMediatek.Controls["tabOngletsApplication"].Controls["tabLivres"].Controls["grpLivresRecherche"].Controls["txbLivresTitreRecherche"];
            frmMediatek.Visible = true;
            TxtRecherche.Text = titreRecherche;
        }
        
        [Then(@"Les informations détaillées doivent afficher le numéro de document (.*)")]
        public void ThenLesInformationsDetailleesDoiventAfficherLeNumeroDeDocument(string numeroAttendu)
        {
            TextBox TxtNumero = (TextBox)frmMediatek.Controls["tabOngletsApplication"].Controls["tabLivres"].Controls["grpLivresInfos"].Controls["txbLivresNumero"];
            string numeroObtenu = TxtNumero.Text;
            Assert.AreEqual(numeroAttendu, numeroObtenu);
        }
    }
}
