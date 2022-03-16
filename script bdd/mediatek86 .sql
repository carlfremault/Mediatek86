-- phpMyAdmin SQL Dump
-- version 5.1.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1:3306
-- Generation Time: Mar 16, 2022 at 05:01 PM
-- Server version: 5.7.36
-- PHP Version: 7.3.21

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: mediatek86
--

DELIMITER $$
--
-- Procedures
--
DROP PROCEDURE IF EXISTS `abonnementsFin30`$$
CREATE DEFINER=`root`@`localhost` PROCEDURE `abonnementsFin30` ()  BEGIN
SELECT a.dateFinAbonnement, a.idRevue, d.titre
		FROM abonnement a
		JOIN revue r ON a.idRevue = r.id
		JOIN document d ON r.id = d.id
		WHERE DATEDIFF(a.dateFinAbonnement, CURDATE()) < 30
		ORDER BY a.dateFinAbonnement ASC;
END$$

DELIMITER ;

-- --------------------------------------------------------

--
-- Table structure for table abonnement
--

DROP TABLE IF EXISTS abonnement;
CREATE TABLE abonnement (
  id varchar(5) NOT NULL,
  dateFinAbonnement date DEFAULT NULL,
  idRevue varchar(10) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Dumping data for table abonnement
--

INSERT INTO abonnement (id, dateFinAbonnement, idRevue) VALUES
('101', '2023-03-13', '10011'),
('102', '2022-03-25', '10002'),
('103', '2022-04-12', '10001'),
('104', '2022-03-20', '10006');

--
-- Triggers abonnement
--
DROP TRIGGER IF EXISTS `insAbonnement`;
DELIMITER $$
CREATE TRIGGER `insAbonnement` BEFORE INSERT ON `abonnement` FOR EACH ROW BEGIN
	DECLARE countId INTEGER;
	SELECT COUNT(*) INTO countId FROM commandedocument WHERE id = new.id;
		IF countId > 0 THEN
		SIGNAL SQLSTATE "45000"
		SET MESSAGE_TEXT = "Une commande avec cet identifiant existe déjà.";
	END IF;
END
$$
DELIMITER ;

-- --------------------------------------------------------

--
-- Table structure for table commande
--

DROP TABLE IF EXISTS commande;
CREATE TABLE commande (
  id varchar(5) NOT NULL,
  dateCommande date DEFAULT NULL,
  montant double DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Dumping data for table commande
--

INSERT INTO commande (id, dateCommande, montant) VALUES
('1', '2022-03-13', 100),
('101', '2022-03-13', 100),
('102', '2022-03-13', 100),
('103', '2022-03-13', 100),
('104', '2022-03-13', 100),
('2', '2022-03-13', 100),
('3', '2022-03-13', 100),
('4', '2022-03-13', 200),
('5', '2022-03-13', 18);

-- --------------------------------------------------------

--
-- Table structure for table commandedocument
--

DROP TABLE IF EXISTS commandedocument;
CREATE TABLE commandedocument (
  id varchar(5) NOT NULL,
  nbExemplaire int(11) DEFAULT NULL,
  idLivreDvd varchar(10) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Dumping data for table commandedocument
--

INSERT INTO commandedocument (id, nbExemplaire, idLivreDvd) VALUES
('1', 2, '00015'),
('2', 4, '00016'),
('3', 4, '00017'),
('4', 2, '00018'),
('5', 2, '20004');

--
-- Triggers commandedocument
--
DROP TRIGGER IF EXISTS `insCommande`;
DELIMITER $$
CREATE TRIGGER `insCommande` BEFORE INSERT ON `commandedocument` FOR EACH ROW BEGIN
	DECLARE countId INTEGER;
	SELECT COUNT(*) INTO countId FROM abonnement WHERE id = new.id;
		IF countId > 0 THEN
		SIGNAL SQLSTATE "45000"
		SET MESSAGE_TEXT = "Un abonnement avec cet identifiant existe déjà.";
	END IF;
END
$$
DELIMITER ;

-- --------------------------------------------------------

--
-- Table structure for table document
--

DROP TABLE IF EXISTS document;
CREATE TABLE document (
  id varchar(10) NOT NULL,
  titre varchar(60) DEFAULT NULL,
  image varchar(100) DEFAULT NULL,
  idRayon varchar(5) NOT NULL,
  idPublic varchar(5) NOT NULL,
  idGenre varchar(5) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Dumping data for table document
--

INSERT INTO document (id, titre, image, idRayon, idPublic, idGenre) VALUES
('00001', 'Quand sort la recluse', '', 'LV003', '00002', '10014'),
('00002', 'Un pays à l\'aube', '', 'LV001', '00002', '10004'),
('00003', 'Et je danse aussi', '', 'LV002', '00003', '10013'),
('00004', 'L\'armée furieuse', '', 'LV003', '00002', '10014'),
('00005', 'Les anonymes', '', 'LV001', '00002', '10014'),
('00006', 'La marque jaune', '', 'BD001', '00003', '10001'),
('00007', 'Dans les coulisses du musée', '', 'LV001', '00003', '10006'),
('00008', 'Histoire du juif errant', '', 'LV002', '00002', '10006'),
('00009', 'Pars vite et reviens tard', '', 'LV003', '00002', '10014'),
('00010', 'Le vestibule des causes perdues', '', 'LV001', '00002', '10006'),
('00011', 'L\'île des oubliés', '', 'LV002', '00003', '10006'),
('00012', 'La souris bleue', '', 'LV002', '00003', '10006'),
('00013', 'Sacré Pêre Noël', '', 'JN001', '00001', '10001'),
('00014', 'Mauvaise étoile', '', 'LV003', '00003', '10014'),
('00015', 'La confrérie des téméraires', '', 'JN002', '00004', '10014'),
('00016', 'Le butin du requin', '', 'JN002', '00004', '10014'),
('00017', 'Catastrophes au Brésil', 'C:\\imgMediatek\\51Cc0m4ZNOL.jpg', 'JN002', '00004', '10014'),
('00018', 'Le Routard - Maroc', '', 'DV005', '00003', '10011'),
('00019', 'Guide Vert - Iles Canaries', '', 'DV005', '00003', '10011'),
('00020', 'Guide Vert - Irlande', '', 'DV005', '00003', '10011'),
('00021', 'Les déferlantes', '', 'LV002', '00002', '10006'),
('00022', 'Une part de Ciel', '', 'LV002', '00002', '10006'),
('00023', 'Le secret du janissaire', '', 'BD001', '00002', '10001'),
('00024', 'Pavillon noir', '', 'BD001', '00002', '10001'),
('00025', 'L\'archipel du danger', '', 'BD001', '00002', '10001'),
('00026', 'La planète des singes', '', 'LV002', '00003', '10002'),
('10001', 'Arts Magazine', '', 'PR002', '00002', '10016'),
('10002', 'Alternatives Economiques', '', 'PR002', '00002', '10015'),
('10003', 'Challenges', '', 'PR002', '00002', '10015'),
('10004', 'Rock and Folk', '', 'PR002', '00002', '10016'),
('10005', 'Les Echos', '', 'PR001', '00002', '10015'),
('10006', 'Le Monde', '', 'PR001', '00002', '10018'),
('10007', 'Telerama', '', 'PR002', '00002', '10016'),
('10008', 'L\'Obs', '', 'PR002', '00002', '10018'),
('10009', 'L\'Equipe', '', 'PR001', '00002', '10017'),
('10010', 'L\'Equipe Magazine', '', 'PR002', '00002', '10017'),
('10011', 'Geo', 'C:\\imgMediatek\\AVT_Geo-magazine_2658.jpeg', 'PR002', '00003', '10016'),
('20001', 'Star Wars 5 L\'empire contre attaque', '', 'DF001', '00003', '10002'),
('20002', 'Le seigneur des anneaux : la communauté de l\'anneau', '', 'DF001', '00003', '10019'),
('20003', 'Jurassic Park', '', 'DF001', '00003', '10002'),
('20004', 'Matrix', 'C:\\imgMediatek\\51vpnbwFHrL._AC_SY445_.jpg', 'DF001', '00003', '10002');

--
-- Triggers document
--
DROP TRIGGER IF EXISTS `delDocument`;
DELIMITER $$
CREATE TRIGGER `delDocument` BEFORE DELETE ON `document` FOR EACH ROW BEGIN
	DECLARE countExemplaire INTEGER;
	DECLARE countAbonnement INTEGER;
	DECLARE countCommandeDoc INTEGER;
	SELECT COUNT(*) INTO countExemplaire FROM exemplaire WHERE id = old.id;
	SELECT COUNT(*) INTO countAbonnement FROM abonnement WHERE idRevue = old.id;
	SELECT COUNT(*) INTO countCommandeDoc FROM commandedocument WHERE idLivreDvd = old.id;
	IF countExemplaire = 1 OR countAbonnement = 1 OR countCommandeDoc = 1 THEN
		SIGNAL SQLSTATE "45000";
	END IF;
END
$$
DELIMITER ;

-- --------------------------------------------------------

--
-- Table structure for table dvd
--

DROP TABLE IF EXISTS dvd;
CREATE TABLE dvd (
  id varchar(10) NOT NULL,
  synopsis text,
  realisateur varchar(20) DEFAULT NULL,
  duree int(6) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Dumping data for table dvd
--

INSERT INTO dvd (id, synopsis, realisateur, duree) VALUES
('20001', 'Luc est entraîné par Yoda pendant que Han et Leia tentent de se cacher dans la cité des nuages.', 'George Lucas', 124),
('20002', 'L\'anneau unique, forgé par Sauron, est porté par Fraudon qui l\'amène à Foncombe. De là, des représentants de peuples différents vont s\'unir pour aider Fraudon à amener l\'anneau à la montagne du Destin.', 'Peter Jackson', 228),
('20003', 'Un milliardaire et des généticiens créent des dinosaures à partir de clonage.', 'Steven Spielberg', 128),
('20004', 'Un informaticien réalise que le monde dans lequel il vit est une simulation gérée par des machines.', 'Les Wachowski', 136);

--
-- Triggers dvd
--
DROP TRIGGER IF EXISTS `insDvd`;
DELIMITER $$
CREATE TRIGGER `insDvd` BEFORE INSERT ON `dvd` FOR EACH ROW BEGIN
	DECLARE countId INTEGER;
	SELECT COUNT(*) INTO countId FROM livre WHERE id = new.id;
	IF countId > 0 THEN
		SIGNAL SQLSTATE "45000"
		SET MESSAGE_TEXT = "Un livre avec cet identifiant existe déjà.";
	END IF;
END
$$
DELIMITER ;

-- --------------------------------------------------------

--
-- Table structure for table etat
--

DROP TABLE IF EXISTS etat;
CREATE TABLE etat (
  id char(5) NOT NULL,
  libelle varchar(20) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Dumping data for table etat
--

INSERT INTO etat (id, libelle) VALUES
('00001', 'neuf'),
('00002', 'usagé'),
('00003', 'détérioré'),
('00004', 'inutilisable');

-- --------------------------------------------------------

--
-- Table structure for table exemplaire
--

DROP TABLE IF EXISTS exemplaire;
CREATE TABLE exemplaire (
  id varchar(10) NOT NULL,
  numero int(11) NOT NULL,
  dateAchat date DEFAULT NULL,
  photo varchar(100) NOT NULL,
  idEtat char(5) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Dumping data for table exemplaire
--

INSERT INTO exemplaire (id, numero, dateAchat, photo, idEtat) VALUES
('00015', 1, '2022-03-01', '', '00001'),
('00015', 2, '2022-03-13', '', '00001'),
('00015', 3, '2022-03-13', '', '00001'),
('00016', 1, '2022-03-13', '', '00001'),
('00016', 2, '2022-03-13', '', '00001'),
('00016', 3, '2022-03-13', '', '00001'),
('00016', 4, '2022-03-13', '', '00001'),
('00017', 1, '2022-03-13', '', '00001'),
('00017', 2, '2022-03-13', '', '00001'),
('00017', 3, '2022-03-13', '', '00001'),
('00017', 4, '2022-03-13', '', '00001'),
('00018', 1, '2022-03-13', '', '00001'),
('00018', 2, '2022-03-13', '', '00001'),
('10001', 50, '2022-03-15', '', '00001'),
('10002', 418, '2021-12-01', '', '00001'),
('10007', 3237, '2021-11-23', '', '00001'),
('10007', 3238, '2021-11-30', '', '00001'),
('10007', 3239, '2021-12-07', '', '00001'),
('10007', 3240, '2021-12-21', '', '00001'),
('10010', 100, '2022-03-13', '', '00001'),
('10011', 506, '2021-04-01', '', '00001'),
('10011', 507, '2021-05-03', '', '00001'),
('10011', 508, '2021-06-05', '', '00001'),
('10011', 509, '2021-07-01', '', '00001'),
('10011', 510, '2021-08-04', '', '00001'),
('10011', 511, '2021-09-01', '', '00001'),
('10011', 512, '2021-10-06', '', '00001'),
('10011', 513, '2021-11-01', '', '00001'),
('10011', 514, '2021-12-01', '', '00001'),
('20004', 1, '2022-03-13', '', '00001'),
('20004', 2, '2022-03-13', '', '00001');

-- --------------------------------------------------------

--
-- Table structure for table genre
--

DROP TABLE IF EXISTS genre;
CREATE TABLE genre (
  id varchar(5) NOT NULL,
  libelle varchar(20) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Dumping data for table genre
--

INSERT INTO genre (id, libelle) VALUES
('10000', 'Humour'),
('10001', 'Bande dessinée'),
('10002', 'Science Fiction'),
('10003', 'Biographie'),
('10004', 'Historique'),
('10006', 'Roman'),
('10007', 'Aventures'),
('10008', 'Essai'),
('10009', 'Documentaire'),
('10010', 'Technique'),
('10011', 'Voyages'),
('10012', 'Drame'),
('10013', 'Comédie'),
('10014', 'Policier'),
('10015', 'Presse Economique'),
('10016', 'Presse Culturelle'),
('10017', 'Presse sportive'),
('10018', 'Actualités'),
('10019', 'Fantazy');

-- --------------------------------------------------------

--
-- Table structure for table livre
--

DROP TABLE IF EXISTS livre;
CREATE TABLE livre (
  id varchar(10) NOT NULL,
  ISBN varchar(13) DEFAULT NULL,
  auteur varchar(20) DEFAULT NULL,
  collection varchar(50) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Dumping data for table livre
--

INSERT INTO livre (id, ISBN, auteur, collection) VALUES
('00001', '1234569877896', 'Fred Vargas', 'Commissaire Adamsberg'),
('00002', '1236547896541', 'Dennis Lehanne', ''),
('00003', '6541236987410', 'Anne-Laure Bondoux', ''),
('00004', '3214569874123', 'Fred Vargas', 'Commissaire Adamsberg'),
('00005', '3214563214563', 'RJ Ellory', ''),
('00006', '3213213211232', 'Edgar P. Jacobs', 'Blake et Mortimer'),
('00007', '6541236987541', 'Kate Atkinson', ''),
('00008', '1236987456321', 'Jean d\'Ormesson', ''),
('00009', '3,21457E+12', 'Fred Vargas', 'Commissaire Adamsberg'),
('00010', '3,21457E+12', 'Manon Moreau', ''),
('00011', '3,21457E+12', 'Victoria Hislop', ''),
('00012', '3,21457E+12', 'Kate Atkinson', ''),
('00013', '3,21457E+12', 'Raymond Briggs', ''),
('00014', '3,21457E+12', 'RJ Ellory', ''),
('00015', '3,21457E+12', 'Floriane Turmeau', ''),
('00016', '3,21457E+12', 'Julian Press', ''),
('00017', '3,21457E+12', 'Philippe Masson', ''),
('00018', '3,21457E+12', '', 'Guide du Routard'),
('00019', '3,21457E+12', '', 'Guide Vert'),
('00020', '3,21457E+12', '', 'Guide Vert'),
('00021', '3,21457E+12', 'Claudie Gallay', ''),
('00022', '3,21457E+12', 'Claudie Gallay', ''),
('00023', '3,21457E+12', 'Ayrolles - Masbou', 'De cape et de crocs'),
('00024', '3,21457E+12', 'Ayrolles - Masbou', 'De cape et de crocs'),
('00025', '3,21457E+12', 'Ayrolles - Masbou', 'De cape et de crocs'),
('00026', '', 'Pierre Boulle', 'Julliard');

--
-- Triggers livre
--
DROP TRIGGER IF EXISTS `insLivre`;
DELIMITER $$
CREATE TRIGGER `insLivre` BEFORE INSERT ON `livre` FOR EACH ROW BEGIN
	DECLARE countId INTEGER;
	SELECT COUNT(*) INTO countId FROM dvd WHERE id = new.id;
	IF countId > 0 THEN
		SIGNAL SQLSTATE "45000"
		SET MESSAGE_TEXT = "Un DVD avec cet identifiant existe déjà.";
	END IF;
END
$$
DELIMITER ;

-- --------------------------------------------------------

--
-- Table structure for table livres_dvd
--

DROP TABLE IF EXISTS livres_dvd;
CREATE TABLE livres_dvd (
  id varchar(10) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Dumping data for table livres_dvd
--

INSERT INTO livres_dvd (id) VALUES
('00001'),
('00002'),
('00003'),
('00004'),
('00005'),
('00006'),
('00007'),
('00008'),
('00009'),
('00010'),
('00011'),
('00012'),
('00013'),
('00014'),
('00015'),
('00016'),
('00017'),
('00018'),
('00019'),
('00020'),
('00021'),
('00022'),
('00023'),
('00024'),
('00025'),
('00026'),
('20001'),
('20002'),
('20003'),
('20004');

--
-- Triggers livres_dvd
--
DROP TRIGGER IF EXISTS `insLivresDvd`;
DELIMITER $$
CREATE TRIGGER `insLivresDvd` BEFORE INSERT ON `livres_dvd` FOR EACH ROW BEGIN
	DECLARE countId INTEGER;
	SELECT COUNT(*) INTO countId FROM revue WHERE id = new.id;
	IF countId > 0 THEN
		SIGNAL SQLSTATE "45000"
		SET MESSAGE_TEXT = "Une revue avec cet identifiant existe déjà.";
	END IF;
END
$$
DELIMITER ;

-- --------------------------------------------------------

--
-- Table structure for table public
--

DROP TABLE IF EXISTS public;
CREATE TABLE public (
  id varchar(5) NOT NULL,
  libelle varchar(50) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Dumping data for table public
--

INSERT INTO public (id, libelle) VALUES
('00001', 'Jeunesse'),
('00002', 'Adultes'),
('00003', 'Tous publics'),
('00004', 'Ados');

-- --------------------------------------------------------

--
-- Table structure for table rayon
--

DROP TABLE IF EXISTS rayon;
CREATE TABLE rayon (
  id char(5) NOT NULL,
  libelle varchar(30) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Dumping data for table rayon
--

INSERT INTO rayon (id, libelle) VALUES
('BD001', 'BD Adultes'),
('BL001', 'Beaux Livres'),
('DF001', 'DVD films'),
('DV001', 'Sciences'),
('DV002', 'Maison'),
('DV003', 'Santé'),
('DV004', 'Littérature classique'),
('DV005', 'Voyages'),
('JN001', 'Jeunesse BD'),
('JN002', 'Jeunesse romans'),
('LV001', 'Littérature étrangère'),
('LV002', 'Littérature française'),
('LV003', 'Policiers français étrangers'),
('PR001', 'Presse quotidienne'),
('PR002', 'Magazines');

-- --------------------------------------------------------

--
-- Table structure for table revue
--

DROP TABLE IF EXISTS revue;
CREATE TABLE revue (
  id varchar(10) NOT NULL,
  empruntable tinyint(1) DEFAULT NULL,
  periodicite varchar(2) DEFAULT NULL,
  delaiMiseADispo int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Dumping data for table revue
--

INSERT INTO revue (id, empruntable, periodicite, delaiMiseADispo) VALUES
('10001', 1, 'MS', 52),
('10002', 1, 'MS', 52),
('10003', 1, 'HB', 15),
('10004', 1, 'HB', 15),
('10005', 0, 'QT', 5),
('10006', 0, 'QT', 5),
('10007', 1, 'HB', 26),
('10008', 1, 'HB', 26),
('10009', 0, 'QT', 5),
('10010', 1, 'HB', 12),
('10011', 1, 'MS', 52);

--
-- Triggers revue
--
DROP TRIGGER IF EXISTS `insRevue`;
DELIMITER $$
CREATE TRIGGER `insRevue` BEFORE INSERT ON `revue` FOR EACH ROW BEGIN
	DECLARE countId INTEGER;
	SELECT COUNT(*) INTO countId FROM livres_dvd WHERE id = new.id;
	IF countId > 0 THEN
		SIGNAL SQLSTATE "45000"
		SET MESSAGE_TEXT = "Un livre ou DVD avec cet identifiant existe déjà.";
	END IF;
END
$$
DELIMITER ;

-- --------------------------------------------------------

--
-- Table structure for table service
--

DROP TABLE IF EXISTS service;
CREATE TABLE service (
  ID int(2) NOT NULL,
  LIBELLE varchar(30) COLLATE utf8_unicode_ci DEFAULT NULL
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

--
-- Dumping data for table service
--

INSERT INTO service (ID, LIBELLE) VALUES
(4, 'culture'),
(3, 'prêt'),
(2, 'services administratifs'),
(1, 'admin');

-- --------------------------------------------------------

--
-- Table structure for table suivi
--

DROP TABLE IF EXISTS suivi;
CREATE TABLE suivi (
  id int(3) NOT NULL,
  libelle varchar(20) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Dumping data for table suivi
--

INSERT INTO suivi (id, libelle) VALUES
(1, 'En cours'),
(2, 'Relancée'),
(3, 'Livrée'),
(4, 'Réglée');

-- --------------------------------------------------------

--
-- Table structure for table suivicommandedoc
--

DROP TABLE IF EXISTS suivicommandedoc;
CREATE TABLE suivicommandedoc (
  idsuivi int(3) NOT NULL,
  idcommande varchar(5) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Dumping data for table suivicommandedoc
--

INSERT INTO suivicommandedoc (idsuivi, idcommande) VALUES
(3, '1'),
(3, '2'),
(3, '3'),
(3, '4'),
(4, '5');

--
-- Triggers suivicommandedoc
--
DROP TRIGGER IF EXISTS `updateSuiviCommandeDoc`;
DELIMITER $$
CREATE TRIGGER `updateSuiviCommandeDoc` AFTER UPDATE ON `suivicommandedoc` FOR EACH ROW BEGIN	
	DECLARE idDoc VARCHAR(10);
	DECLARE nombre INTEGER;
	DECLARE date DATE;
	DECLARE numeroExemplaire INTEGER;
	/* trigger uniquement en cas de passage à l'étape 3 (livrée) */
	IF new.idSuivi = 3 THEN
		
		SELECT idLivreDvd, nbExemplaire, dateCommande INTO idDoc, nombre, date
			FROM suivicommandedoc scd 
			JOIN commandedocument cd ON scd.idcommande = cd.id 
			JOIN commande c ON cd.id = c.id
			WHERE scd.idcommande = old.idcommande;
			
		/* récupération du numéro d'exemplaire le plus élévé du document concerné */
		SELECT MAX(numero) INTO numeroExemplaire FROM exemplaire e WHERE e.id = idDoc;
		
		IF numeroExemplaire IS NULL THEN 
			SET numeroExemplaire = 1;
		ELSE
			SET numeroExemplaire = numeroExemplaire +1;
		END IF;
		
		/* ajout des exemplaires */
		REPEAT
			INSERT INTO exemplaire (id, numero, dateAchat, photo, idEtat) VALUES(idDoc, numeroExemplaire, date, "", "00001");
			SET numeroExemplaire = numeroExemplaire + 1;
			SET nombre = nombre - 1;
		UNTIL nombre = 0 END REPEAT;
	END IF;
END
$$
DELIMITER ;

-- --------------------------------------------------------

--
-- Table structure for table utilisateur
--

DROP TABLE IF EXISTS utilisateur;
CREATE TABLE utilisateur (
  NOM varchar(30) COLLATE utf8_unicode_ci NOT NULL,
  ID int(2) NOT NULL,
  MDP varchar(50) COLLATE utf8_unicode_ci DEFAULT NULL
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

--
-- Dumping data for table utilisateur
--

INSERT INTO utilisateur (NOM, ID, MDP) VALUES
('admin', 1, '21232f297a57a5a743894a0e4a801fc3'),
('serviceadmin', 2, 'b186f841d646eb03b34761000b630c6f'),
('pret', 3, '5e18cf11d8566f62ce38f2a00fb8410e'),
('culture', 4, '3f7039a836c00d92ecf87fd7d338c4db');

--
-- Indexes for dumped tables
--

--
-- Indexes for table abonnement
--
ALTER TABLE abonnement
  ADD PRIMARY KEY (id),
  ADD KEY idRevue (idRevue);

--
-- Indexes for table commande
--
ALTER TABLE commande
  ADD PRIMARY KEY (id);

--
-- Indexes for table commandedocument
--
ALTER TABLE commandedocument
  ADD PRIMARY KEY (id),
  ADD KEY idLivreDvd (idLivreDvd);

--
-- Indexes for table document
--
ALTER TABLE document
  ADD PRIMARY KEY (id),
  ADD KEY idRayon (idRayon),
  ADD KEY idPublic (idPublic),
  ADD KEY idGenre (idGenre);

--
-- Indexes for table dvd
--
ALTER TABLE dvd
  ADD PRIMARY KEY (id);

--
-- Indexes for table etat
--
ALTER TABLE etat
  ADD PRIMARY KEY (id);

--
-- Indexes for table exemplaire
--
ALTER TABLE exemplaire
  ADD PRIMARY KEY (id,numero),
  ADD KEY idEtat (idEtat);

--
-- Indexes for table genre
--
ALTER TABLE genre
  ADD PRIMARY KEY (id);

--
-- Indexes for table livre
--
ALTER TABLE livre
  ADD PRIMARY KEY (id);

--
-- Indexes for table livres_dvd
--
ALTER TABLE livres_dvd
  ADD PRIMARY KEY (id);

--
-- Indexes for table public
--
ALTER TABLE public
  ADD PRIMARY KEY (id);

--
-- Indexes for table rayon
--
ALTER TABLE rayon
  ADD PRIMARY KEY (id);

--
-- Indexes for table revue
--
ALTER TABLE revue
  ADD PRIMARY KEY (id);

--
-- Indexes for table service
--
ALTER TABLE service
  ADD PRIMARY KEY (ID);

--
-- Indexes for table suivi
--
ALTER TABLE suivi
  ADD PRIMARY KEY (id);

--
-- Indexes for table suivicommandedoc
--
ALTER TABLE suivicommandedoc
  ADD PRIMARY KEY (idsuivi,idcommande),
  ADD KEY suivicommandedoc_fk2 (idcommande);

--
-- Indexes for table utilisateur
--
ALTER TABLE utilisateur
  ADD PRIMARY KEY (NOM),
  ADD KEY I_FK_UTILISATEUR_SERVICE (ID);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table service
--
ALTER TABLE service
  MODIFY ID int(2) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=14;

--
-- Constraints for dumped tables
--

--
-- Constraints for table abonnement
--
ALTER TABLE abonnement
  ADD CONSTRAINT abonnement_ibfk_1 FOREIGN KEY (id) REFERENCES commande (id),
  ADD CONSTRAINT abonnement_ibfk_2 FOREIGN KEY (idRevue) REFERENCES revue (id);

--
-- Constraints for table commandedocument
--
ALTER TABLE commandedocument
  ADD CONSTRAINT commandedocument_ibfk_1 FOREIGN KEY (id) REFERENCES commande (id),
  ADD CONSTRAINT commandedocument_ibfk_2 FOREIGN KEY (idLivreDvd) REFERENCES livres_dvd (id);

--
-- Constraints for table document
--
ALTER TABLE document
  ADD CONSTRAINT document_ibfk_1 FOREIGN KEY (idRayon) REFERENCES rayon (id),
  ADD CONSTRAINT document_ibfk_2 FOREIGN KEY (idPublic) REFERENCES public (id),
  ADD CONSTRAINT document_ibfk_3 FOREIGN KEY (idGenre) REFERENCES genre (id);

--
-- Constraints for table dvd
--
ALTER TABLE dvd
  ADD CONSTRAINT dvd_ibfk_1 FOREIGN KEY (id) REFERENCES livres_dvd (id);

--
-- Constraints for table exemplaire
--
ALTER TABLE exemplaire
  ADD CONSTRAINT exemplaire_ibfk_1 FOREIGN KEY (id) REFERENCES document (id),
  ADD CONSTRAINT exemplaire_ibfk_2 FOREIGN KEY (idEtat) REFERENCES etat (id);

--
-- Constraints for table livre
--
ALTER TABLE livre
  ADD CONSTRAINT livre_ibfk_1 FOREIGN KEY (id) REFERENCES livres_dvd (id);

--
-- Constraints for table livres_dvd
--
ALTER TABLE livres_dvd
  ADD CONSTRAINT livres_dvd_ibfk_1 FOREIGN KEY (id) REFERENCES document (id);

--
-- Constraints for table revue
--
ALTER TABLE revue
  ADD CONSTRAINT revue_ibfk_1 FOREIGN KEY (id) REFERENCES document (id);

--
-- Constraints for table suivicommandedoc
--
ALTER TABLE suivicommandedoc
  ADD CONSTRAINT suivicommandedoc_fk1 FOREIGN KEY (idsuivi) REFERENCES suivi (id),
  ADD CONSTRAINT suivicommandedoc_fk2 FOREIGN KEY (idcommande) REFERENCES commandedocument (id);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
