Feature: AjouterLivrePuisChercher
	Ajouter puis rechercher un livre

@ajouterLivrePuisChercher
Scenario: Ajouter puis rechercher un livre
	Given Je clic sur le bouton ajouter
	And Je saisis le numéro de document 100
	And Je saisis le titre testTitre
	And Je saisis l'auteur testAuteur
	And Je saisis le genre Roman
	And Je saisis le public Adultes
	And Je saisis le rayon Littérature française
	And Je clic sur Enregistrer
	When Je saisis le titre testTitre
	Then Les informations détaillées doivent afficher le numéro de document 100