using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Mediatek86.controleur.Tests
{
    [TestClass()]
    public class ControleTests
    {
        private readonly DateTime earlyDate = new DateTime(2022, 1, 1);
        private readonly DateTime middleDate = new DateTime(2022, 2, 1);
        private readonly DateTime lateDate = new DateTime(2022, 3, 1);

        private readonly Controle controleur = new Controle();

        [TestMethod()]
        public void ParutionDansAbonnementTest()
        {
            // Date parution égale à date commande
            bool result1 = controleur.ParutionDansAbonnement(earlyDate, lateDate, earlyDate);
            Assert.AreEqual(false, result1, "Devrait réussir, dateparution égale à date commande donne false");
            // Date parution égale à date fin abonnement
            bool result2 = controleur.ParutionDansAbonnement(earlyDate, lateDate, lateDate);
            Assert.AreEqual(false, result2, "Devrait réussir, dateparution égale à date fin d'abonnement donne false");
            // Date parution avant date commande
            bool result3 = controleur.ParutionDansAbonnement(middleDate, lateDate, earlyDate);
            Assert.AreEqual(false, result3, "Devrait réussir, dateparution avant date commande donne false");
            // Date parution après date fin abonnement
            bool result4 = controleur.ParutionDansAbonnement(earlyDate, middleDate, lateDate);
            Assert.AreEqual(false, result4, "Devrait réussir, dateparution après date fin abonnement donne false");
            // Date parution comprise entre date abonnement et date fin abonnement
            bool result5 = controleur.ParutionDansAbonnement(earlyDate, lateDate, middleDate);
            Assert.AreEqual(true, result5, "Devrait réussir, dateparution comprise entre dates commande et fin abonnement donne true");
        }
    }
}