﻿using Mediatek86.metier;
using System.Collections.Generic;
using Mediatek86.bdd;
using System;
using System.Windows.Forms;

namespace Mediatek86.modele
{
    public static class Dao
    {

        private static readonly string server = "localhost";
        private static readonly string userid = "root";
        private static readonly string password = "";
        private static readonly string database = "mediatek86";
        private static readonly string connectionString = "server="+server+";user id="+userid+";password="+password+";database="+database+";SslMode=none";

        /// <summary>
        /// Retourne tous les genres à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Genre</returns>
        public static List<Categorie> GetAllGenres()
        {
            List<Categorie> lesGenres = new List<Categorie>();
            string req = "Select * from genre order by libelle";

            BddMySql curs = BddMySql.GetInstance(connectionString);
            curs.ReqSelect(req, null);

            while (curs.Read())
            {
                Genre genre = new Genre((string)curs.Field("id"), (string)curs.Field("libelle"));
                lesGenres.Add(genre);
            }
            curs.Close();
            return lesGenres;
        }

        /// <summary>
        /// Retourne tous les rayons à partir de la BDD
        /// </summary>
        /// <returns>Collection d'objets Rayon</returns>
        public static List<Categorie> GetAllRayons()
        {
            List<Categorie> lesRayons = new List<Categorie>();
            string req = "Select * from rayon order by libelle";

            BddMySql curs = BddMySql.GetInstance(connectionString);
            curs.ReqSelect(req, null);

            while (curs.Read())
            {
                Rayon rayon = new Rayon((string)curs.Field("id"), (string)curs.Field("libelle"));
                lesRayons.Add(rayon);
            }
            curs.Close();
            return lesRayons;
        }

        /// <summary>
        /// Retourne toutes les catégories de public à partir de la BDD
        /// </summary>
        /// <returns>Collection d'objets Public</returns>
        public static List<Categorie> GetAllPublics()
        {
            List<Categorie> lesPublics = new List<Categorie>();
            string req = "Select * from public order by libelle";

            BddMySql curs = BddMySql.GetInstance(connectionString);
            curs.ReqSelect(req, null);

            while (curs.Read())
            {
                Public lePublic = new Public((string)curs.Field("id"), (string)curs.Field("libelle"));
                lesPublics.Add(lePublic);
            }
            curs.Close();
            return lesPublics;
        }

        /// <summary>
        /// Retourne toutes les livres à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Livre</returns>
        public static List<Livre> GetAllLivres()
        {
            List<Livre> lesLivres = new List<Livre>();
            string req = "Select l.id, l.ISBN, l.auteur, d.titre, d.image, l.collection, ";
            req += "d.idrayon, d.idpublic, d.idgenre, g.libelle as genre, p.libelle as public, r.libelle as rayon ";
            req += "from livre l join document d on l.id=d.id ";
            req += "join genre g on g.id=d.idGenre ";
            req += "join public p on p.id=d.idPublic ";
            req += "join rayon r on r.id=d.idRayon ";
            req += "order by titre ";

            BddMySql curs = BddMySql.GetInstance(connectionString);
            curs.ReqSelect(req, null);

            while (curs.Read())
            {
                string id = (string)curs.Field("id");
                string isbn = (string)curs.Field("ISBN");
                string auteur = (string)curs.Field("auteur");
                string titre = (string)curs.Field("titre");
                string image = (string)curs.Field("image");
                string collection = (string)curs.Field("collection");
                string idgenre = (string)curs.Field("idgenre");
                string idrayon = (string)curs.Field("idrayon");
                string idpublic = (string)curs.Field("idpublic");
                string genre = (string)curs.Field("genre");
                string lepublic = (string)curs.Field("public");
                string rayon = (string)curs.Field("rayon");
                Livre livre = new Livre(id, titre, image, isbn, auteur, collection, idgenre, genre, 
                    idpublic, lepublic, idrayon, rayon);
                lesLivres.Add(livre);
            }
            curs.Close();

            return lesLivres;
        }

        /// <summary>
        /// Retourne toutes les dvd à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Dvd</returns>
        public static List<Dvd> GetAllDvd()
        {
            List<Dvd> lesDvd = new List<Dvd>();
            string req = "Select l.id, l.duree, l.realisateur, d.titre, d.image, l.synopsis, ";
            req += "d.idrayon, d.idpublic, d.idgenre, g.libelle as genre, p.libelle as public, r.libelle as rayon ";
            req += "from dvd l join document d on l.id=d.id ";
            req += "join genre g on g.id=d.idGenre ";
            req += "join public p on p.id=d.idPublic ";
            req += "join rayon r on r.id=d.idRayon ";
            req += "order by titre ";

            BddMySql curs = BddMySql.GetInstance(connectionString);
            curs.ReqSelect(req, null);

            while (curs.Read())
            {
                string id = (string)curs.Field("id");
                int duree = (int)curs.Field("duree");
                string realisateur = (string)curs.Field("realisateur");
                string titre = (string)curs.Field("titre");
                string image = (string)curs.Field("image");
                string synopsis = (string)curs.Field("synopsis");
                string idgenre = (string)curs.Field("idgenre");
                string idrayon = (string)curs.Field("idrayon");
                string idpublic = (string)curs.Field("idpublic");
                string genre = (string)curs.Field("genre");
                string lepublic = (string)curs.Field("public");
                string rayon = (string)curs.Field("rayon");
                Dvd dvd = new Dvd(id, titre, image, duree, realisateur, synopsis, idgenre, genre,
                    idpublic, lepublic, idrayon, rayon);
                lesDvd.Add(dvd);
            }
            curs.Close();

            return lesDvd;
        }

        /// <summary>
        /// Retourne toutes les revues à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Revue</returns>
        public static List<Revue> GetAllRevues()
        {
            List<Revue> lesRevues = new List<Revue>();
            string req = "Select l.id, l.empruntable, l.periodicite, d.titre, d.image, l.delaiMiseADispo, ";
            req += "d.idrayon, d.idpublic, d.idgenre, g.libelle as genre, p.libelle as public, r.libelle as rayon ";
            req += "from revue l join document d on l.id=d.id ";
            req += "join genre g on g.id=d.idGenre ";
            req += "join public p on p.id=d.idPublic ";
            req += "join rayon r on r.id=d.idRayon ";
            req += "order by titre ";

            BddMySql curs = BddMySql.GetInstance(connectionString);
            curs.ReqSelect(req, null);

            while (curs.Read())
            {
                string id = (string)curs.Field("id");
                bool empruntable = (bool)curs.Field("empruntable");
                string periodicite = (string)curs.Field("periodicite");
                string titre = (string)curs.Field("titre");
                string image = (string)curs.Field("image");
                int delaiMiseADispo = (int)curs.Field("delaimiseadispo");
                string idgenre = (string)curs.Field("idgenre");
                string idrayon = (string)curs.Field("idrayon");
                string idpublic = (string)curs.Field("idpublic");
                string genre = (string)curs.Field("genre");
                string lepublic = (string)curs.Field("public");
                string rayon = (string)curs.Field("rayon");
                Revue revue = new Revue(id, titre, image, idgenre, genre,
                    idpublic, lepublic, idrayon, rayon, empruntable, periodicite, delaiMiseADispo);
                lesRevues.Add(revue);
            }
            curs.Close();

            return lesRevues;
        }

        /// <summary>
        /// Retourne les exemplaires d'une revue
        /// </summary>
        /// <returns>Liste d'objets Exemplaire</returns>
        public static List<Exemplaire> GetExemplairesRevue(string idDoc)
        {
            List<Exemplaire> lesExemplaires = new List<Exemplaire>();
            string req = "Select e.id, e.numero, e.dateAchat, e.photo, e.idEtat ";
            req += "from exemplaire e join document d on e.id=d.id ";
            req += "where e.id = @id ";
            req += "order by e.dateAchat DESC";
            Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@id", idDoc}
                };

            BddMySql curs = BddMySql.GetInstance(connectionString);
            curs.ReqSelect(req, parameters);

            while (curs.Read())
            {
                string idDocument = (string)curs.Field("id");
                int numero = (int)curs.Field("numero");
                DateTime dateAchat = (DateTime)curs.Field("dateAchat");
                string photo = (string)curs.Field("photo");
                string idEtat = (string)curs.Field("idEtat");
                Exemplaire exemplaire = new Exemplaire(numero, dateAchat, photo, idEtat, idDocument);
                lesExemplaires.Add(exemplaire);
            }
            curs.Close();

            return lesExemplaires;
        }

        /// <summary>
        /// Retourne les commandes d'un livre ou d'un DVD
        /// </summary>
        /// <returns>Liste d'objets CommandeDocument</returns>
        public static List<CommandeDocument> GetCommandeDocument(string idDoc)
        {
            List<CommandeDocument> lesCommandes = new List<CommandeDocument>();
            string req = "Select c.id, c.dateCommande, c.montant, cd.nbExemplaire, scd.idSuivi, s.libelle ";
            req += "from commande c join commandedocument cd on c.id=cd.id ";
            req += "join suivicommandedoc scd on c.id=scd.idcommande ";
            req += "join suivi s on scd.idsuivi = s.id ";
            req += "where cd.idLivreDvd = @id ";
            req += "order by c.dateCommande DESC";
            Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@id", idDoc}
                };

            BddMySql curs = BddMySql.GetInstance(connectionString);
            curs.ReqSelect(req, parameters);

            while (curs.Read())
            {
                string id = (string)curs.Field("id");
                DateTime dateCommande = (DateTime)curs.Field("datecommande");
                double montant = (double)curs.Field("montant");
                int nbExemplaire = (int)curs.Field("nbExemplaire");
                int idSuivi = (int)curs.Field("idSuivi");
                string libelleSuivi = (string)curs.Field("libelle");
                CommandeDocument commandeDocument = new CommandeDocument(id, dateCommande, montant, nbExemplaire, idSuivi, libelleSuivi);
                lesCommandes.Add(commandeDocument);
            }
            curs.Close();

            return lesCommandes;
        }

        /// <summary>
        /// Retourne les commandes d'un livre ou d'un DVD
        /// </summary>
        /// <returns>Liste d'objets CommandeDocument</returns>
        public static List<CommandeDocument> GetAbonnement(string idDoc)
        {
            List<CommandeDocument> lesCommandes = new List<CommandeDocument>();
            string req = "Select c.id, c.dateCommande, c.montant, cd.nbExemplaire, scd.idSuivi, s.libelle ";
            req += "from commande c join commandedocument cd on c.id=cd.id ";
            req += "join suivicommandedoc scd on c.id=scd.idcommande ";
            req += "join suivi s on scd.idsuivi = s.id ";
            req += "where cd.idLivreDvd = @id ";
            req += "order by c.dateCommande DESC";
            Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@id", idDoc}
                };

            BddMySql curs = BddMySql.GetInstance(connectionString);
            curs.ReqSelect(req, parameters);

            while (curs.Read())
            {
                string id = (string)curs.Field("id");
                DateTime dateCommande = (DateTime)curs.Field("datecommande");
                double montant = (double)curs.Field("montant");
                int nbExemplaire = (int)curs.Field("nbExemplaire");
                int idSuivi = (int)curs.Field("idSuivi");
                string libelleSuivi = (string)curs.Field("libelle");
                CommandeDocument commandeDocument = new CommandeDocument(id, dateCommande, montant, nbExemplaire, idSuivi, libelleSuivi);
                lesCommandes.Add(commandeDocument);
            }
            curs.Close();

            return lesCommandes;
        }

        /// <summary>
        /// ecriture d'un exemplaire en base de données
        /// </summary>
        /// <param name="exemplaire"></param>
        /// <returns>true si l'insertion a pu se faire</returns>
        public static bool CreerExemplaire(Exemplaire exemplaire)
        {
            try
            {
                List<string> requetes = new List<string>();
                requetes.Add("insert into exemplaire values (@idDocument,@numero,@dateAchat,@photo,@idEtat)");
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@idDocument", exemplaire.IdDocument},
                    { "@numero", exemplaire.Numero},
                    { "@dateAchat", exemplaire.DateAchat},
                    { "@photo", exemplaire.Photo},
                    { "@idEtat",exemplaire.IdEtat}
                };
                BddMySql curs = BddMySql.GetInstance(connectionString);
                curs.ReqUpdate(requetes, parameters);
                curs.Close();
                return true;
            }catch{
                return false;
            }
        }

        /// <summary>
        /// écriture d'un livre en base de données
        /// </summary>
        /// <param name="livre">le livre à ajouter</param>
        /// <returns>true si l'insertion a pu se faire</returns>
        public static string CreerLivre(Livre livre)
        {
            try
            {
                List<string> requetes = new List<string>();
                requetes.Add("insert into document values (@id, @titre, @image, @idRayon, @idPublic, @idGenre)");
                requetes.Add("insert into livres_dvd values (@id)");
                requetes.Add("insert into livre values (@id, @isbn, @auteur, @collection)");
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    {"@id", livre.Id },
                    {"@titre", livre.Titre },
                    {"@image", livre.Image },
                    {"@idRayon", livre.IdRayon },
                    {"@idPublic", livre.IdPublic },
                    {"@idGenre", livre.IdGenre },
                    {"@isbn", livre.Isbn },
                    {"@auteur", livre.Auteur },
                    {"@collection", livre.Collection },
                };
                BddMySql curs = BddMySql.GetInstance(connectionString);              
                curs.ReqUpdate(requetes, parameters);
                curs.Close();
                return "Ajout de livre réussi!";
            }
            catch (Exception e)
            {
                return e.Message;
            }           
        }

        /// <summary>
        /// modification d'un livre en base de données
        /// </summary>
        /// <param name="livre">le livre à modifier</param>
        /// <returns>true si la modification a pu se faire</returns>
        public static bool ModifLivre(Livre livre)
        {
            try
            {
                List<string> requetes = new List<string>();
                requetes.Add("update document set titre=@titre, image=@image, idRayon=@idRayon, idPublic=@idPublic, idGenre=@idGenre where id=@id");
                requetes.Add("update livre set isbn=@isbn, auteur=@auteur, collection=@collection where id=@id");
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    {"@id", livre.Id },
                    {"@titre", livre.Titre },
                    {"@image", livre.Image },
                    {"@idRayon", livre.IdRayon },
                    {"@idPublic", livre.IdPublic },
                    {"@idGenre", livre.IdGenre },
                    {"@isbn", livre.Isbn },
                    {"@auteur", livre.Auteur },
                    {"@collection", livre.Collection },
                };
                BddMySql curs = BddMySql.GetInstance(connectionString);
                curs.ReqUpdate(requetes, parameters);
                curs.Close();
                return true;
            }
            catch
            {
                return false;
            }            
        }

        /// <summary>
        /// Suppression d'un livre de la base de données
        /// </summary>
        /// <param name="id">identifiant du livre à supprimer</param>
        /// <returns>true si la modification a pu se faire</returns>
        public static bool SupprLivre(string id)
        {
            try
            {
                List<string> requetes = new List<string>();
                requetes.Add("delete from livre  where id=@id");
                requetes.Add("delete from livres_dvd  where id=@id");
                requetes.Add("delete from document  where id=@id");
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    {"@id", id },
                };
                BddMySql curs = BddMySql.GetInstance(connectionString);
                curs.ReqUpdate(requetes, parameters);
                curs.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// écriture d'un DVD en base de données
        /// </summary>
        /// <param name="dvd">le DVD à ajouter</param>
        /// <returns>true si l'insertion a pu se faire</returns>
        public static string CreerDvd(Dvd dvd)
        {
            try
            {
                List<string> requetes = new List<string>();
                requetes.Add("insert into document values (@id, @titre, @image, @idRayon, @idPublic, @idGenre)");
                requetes.Add("insert into livres_dvd values (@id)");
                requetes.Add("insert into dvd values (@id, @synopsis, @realisateur, @duree)");
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    {"@id", dvd.Id },
                    {"@titre", dvd.Titre },
                    {"@image", dvd.Image },
                    {"@idRayon", dvd.IdRayon },
                    {"@idPublic", dvd.IdPublic },
                    {"@idGenre", dvd.IdGenre },
                    {"@synopsis", dvd.Synopsis },
                    {"@realisateur", dvd.Realisateur },
                    {"@duree", dvd.Duree },
                };
                BddMySql curs = BddMySql.GetInstance(connectionString);
                curs.ReqUpdate(requetes, parameters);
                curs.Close();
                return "Ajout de DVD réussi!";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        /// <summary>
        /// modification d'un DVD en base de données
        /// </summary>
        /// <param name="dvd">le DVD à modifier</param>
        /// <returns>true si la modification a pu se faire</returns>
        public static bool ModifDvd(Dvd dvd)
        {
            try
            {
                List<string> requetes = new List<string>();
                requetes.Add("update document set titre=@titre, image=@image, idRayon=@idRayon, idPublic=@idPublic, idGenre=@idGenre where id=@id");
                requetes.Add("update dvd set synopsis=@synopsis, realisateur=@realisateur, duree=@duree where id=@id");
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    {"@id", dvd.Id },
                    {"@titre", dvd.Titre },
                    {"@image", dvd.Image },
                    {"@idRayon", dvd.IdRayon },
                    {"@idPublic", dvd.IdPublic },
                    {"@idGenre", dvd.IdGenre },
                    {"@synopsis", dvd.Synopsis },
                    {"@realisateur", dvd.Realisateur },
                    {"@duree", dvd.Duree },
                };
                BddMySql curs = BddMySql.GetInstance(connectionString);
                curs.ReqUpdate(requetes, parameters);
                curs.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Suppression d'un DVD de la base de données
        /// </summary>
        /// <param name="id">identifiant du DVD à supprimer</param>
        /// <returns>true si la modification a pu se faire</returns>
        public static bool SupprDvd(string id)
        {
            try
            {
                List<string> requetes = new List<string>();
                requetes.Add("delete from dvd  where id=@id");
                requetes.Add("delete from livres_dvd  where id=@id");
                requetes.Add("delete from document  where id=@id");
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    {"@id", id },
                };
                BddMySql curs = BddMySql.GetInstance(connectionString);
                curs.ReqUpdate(requetes, parameters);
                curs.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// écriture d'une revue en base de données
        /// </summary>
        /// <param name="revue">la revue à ajouter</param>
        /// <returns>true si l'insertion a pu se faire</returns>
        public static string CreerRevue(Revue revue)
        {
            try
            {
                List<string> requetes = new List<string>();
                requetes.Add("insert into document values (@id, @titre, @image, @idRayon, @idPublic, @idGenre)");
                requetes.Add("insert into revue values (@id, @empruntable, @periodicite, @delaiMiseADispo)");
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    {"@id", revue.Id },
                    {"@titre", revue.Titre },
                    {"@image", revue.Image },
                    {"@idRayon", revue.IdRayon },
                    {"@idPublic", revue.IdPublic },
                    {"@idGenre", revue.IdGenre },
                    {"@empruntable", revue.Empruntable },
                    {"@periodicite", revue.Periodicite },
                    {"@delaiMiseADispo", revue.DelaiMiseADispo }
                };
                BddMySql curs = BddMySql.GetInstance(connectionString);
                curs.ReqUpdate(requetes, parameters);
                curs.Close();
                return "Ajout de Revue réussi!";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        /// <summary>
        /// modification d'une revue en base de données
        /// </summary>
        /// <param name="revue">la revue à modifier</param>
        /// <returns>true si la modification a pu se faire</returns>
        public static bool ModifRevue(Revue revue)
        {
            try
            {
                List<string> requetes = new List<string>();
                requetes.Add("update document set titre=@titre, image=@image, idRayon=@idRayon, idPublic=@idPublic, idGenre=@idGenre where id=@id");
                requetes.Add("update revue set empruntable=@empruntable, periodicite=@periodicite, delaiMiseADispo=@delaiMiseADispo where id=@id");
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    {"@id", revue.Id },
                    {"@titre", revue.Titre },
                    {"@image", revue.Image },
                    {"@idRayon", revue.IdRayon },
                    {"@idPublic", revue.IdPublic },
                    {"@idGenre", revue.IdGenre },
                    {"@empruntable", revue.Empruntable },
                    {"@periodicite", revue.Periodicite },
                    {"@delaiMiseADispo", revue.DelaiMiseADispo }
                };
                BddMySql curs = BddMySql.GetInstance(connectionString);
                curs.ReqUpdate(requetes, parameters);
                curs.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Suppression d'une revue de la base de données
        /// </summary>
        /// <param name="id">identifiant de la revue à supprimer</param>
        /// <returns>true si la modification a pu se faire</returns>
        public static bool SupprRevue(string id)
        {
            try
            {
                List<string> requetes = new List<string>();
                requetes.Add("delete from revue  where id=@id");
                requetes.Add("delete from document  where id=@id");
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    {"@id", id },
                };
                BddMySql curs = BddMySql.GetInstance(connectionString);
                curs.ReqUpdate(requetes, parameters);
                curs.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
