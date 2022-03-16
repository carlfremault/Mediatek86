Feature: SupprimerLivre
	Suppression puis recherche d'un livre

@SupprimerLivre
Scenario: Supprimer un livre
	Given je saisie le numero de livre 100
	And je clic sur le bouton pour Rechercher
	And Je clic sur le bouton pour Supprimer
	When je saisis le titre du livre testTitre
	Then Aucune valeur ne doit être retrouvée