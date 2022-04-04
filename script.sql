DROP TABLE GroupeMdp;
DROP TABLE CompteGroupe;
DROP TABLE Groupe;
DROP TABLE CarnetAdresse;
DROP TABLE MotDePasse;
DROP TABLE Compte;

--CREATE DATABASE GestionMdp;

--USE GestionMdp;

CREATE TABLE Compte
(
    id int PRIMARY KEY IDENTITY(1, 1) NOT NULL,
    nom varchar(200) NOT NULL,
    prenom varchar(200) NOT NULL,

    mail varchar(250) NOT NULL,
    mdp varchar(500) NOT NULL,
    hashCle varchar(200) NOT NULL
);

CREATE TABLE MotDePasse
(
    id int PRIMARY KEY IDENTITY(1, 1) NOT NULL,

    titre varchar(200) NOT NULL,
    login varchar(200) NOT NULL,
    mdp varchar(500) NOT NULL,
    url varchar(500) NOT NULL,
    dateExpiration varchar(200) NOT NULL,
    description varchar(2000) NOT NULL,

    idCompteCreateur int NOT NULL,

    FOREIGN KEY (idCompteCreateur) REFERENCES Compte(id) ON DELETE CASCADE
);

CREATE TABLE CarnetAdresse
(
    idCompte int NOT NULL,
    idCompteCarnet int NOT NULL,

    PRIMARY KEY(idCompte, idCompteCarnet),

    FOREIGN KEY (idCompte) REFERENCES Compte(id),
    FOREIGN KEY (idCompteCarnet) REFERENCES Compte(id)
);

CREATE TABLE Groupe
(
    id int PRIMARY KEY IDENTITY(1, 1) NOT NULL,
    idCompteCreateur int NOT NULL,
    titre varchar(100) NOT NULL,

    FOREIGN KEY (idCompteCreateur) REFERENCES Compte(id) ON DELETE CASCADE
);

CREATE TABLE CompteGroupe
(
    idGroupe int NOT NULL,
    idCompte int NOT NULL,

    PRIMARY KEY (idGroupe, idCompte),

    FOREIGN KEY (idGroupe) REFERENCES Groupe(id) ON DELETE CASCADE,
    FOREIGN KEY (idCompte) REFERENCES Compte(id)
);

CREATE TABLE GroupeMdp
(
    idGroupe int NOT NULL,
    idMdp int NOT NULL,

    PRIMARY KEY(idGroupe, idMdp),

    FOREIGN KEY (idGroupe) REFERENCES Groupe(id) ON DELETE CASCADE,
    FOREIGN KEY (idMdp) REFERENCES MotDePasse(id)
);

/*
  "nom": "jeton",
  "prenom": "peche",
  "mail": "a@a.com",
  "mdp": "azerty123"
*/
SET IDENTITY_INSERT Compte ON;
INSERT INTO Compte(id, nom, prenom, mail, mdp, hashCle) VALUES
(1, 'eA9j9P1aDVAE1xf7RDEREw==', 'axPLSVId4mZJzM+i+26yQw==', 'a@a.com', '$2a$11$PfqXnKc88m6vWoRqAI39COUk4xGt4/rQe3EO4XfN9hpAVTgB/IdYW', '$2a$11$5FPfTSv/dy3XWMDx9d7wPuHiBUuyfSsDEnXNnmlh04ChKFHdTZgU.'), 
(2, 'eA9j9P1aDVAE1xf7RDEREw==', 'axPLSVId4mZJzM+i+26yQw==', 'a@a.com', '$2a$11$PfqXnKc88m6vWoRqAI39COUk4xGt4/rQe3EO4XfN9hpAVTgB/IdYW', '$2a$11$5FPfTSv/dy3XWMDx9d7wPuHiBUuyfSsDEnXNnmlh04ChKFHdTZgU.');
SET IDENTITY_INSERT Compte OFF;

/*
    DateExpiration: "2022-04-03"
    Description: ""
    Id: 1
    Login: "login"
    Mdp: "azerty123"
    Titre: "titre"
    Url: "http://youtube.com"
*/
SET IDENTITY_INSERT MotDePasse ON;
INSERT INTO MotDePasse(id, titre, login, mdp, url, dateExpiration, description, idCompteCreateur) VALUES
(1, 'XzkHa7XGiIG/3ha/qk2xvg==', 'p9K5pO6g7mjvoiR8wbX36g==', 'nbLrArT7f5C5Y4xhCA+Jyw==', 'E1AEsWKv81vSQ2X+lNE6AvCc23gg4ITjvlWOIlAAc3k=', 'Zog0I/ZB1+7xkEd/vzbF1w==', '', 1),
(2, 'XzkHa7XGiIG/3ha/qk2xvg==', 'p9K5pO6g7mjvoiR8wbX36g==', 'nbLrArT7f5C5Y4xhCA+Jyw==', 'E1AEsWKv81vSQ2X+lNE6AvCc23gg4ITjvlWOIlAAc3k=', 'Zog0I/ZB1+7xkEd/vzbF1w==', '', 1);
SET IDENTITY_INSERT MotDePasse OFF;

INSERT INTO CarnetAdresse(idCompte, idCompteCarnet) VALUES (1, 2), (2, 1);

SET IDENTITY_INSERT Groupe ON;
INSERT INTO Groupe(id, idCompteCreateur, titre) VALUES (1, 2, 'Groupe 1');
SET IDENTITY_INSERT Groupe OFF;

INSERT INTO CompteGroupe(idCompte, idGroupe) VALUES (1, 1), (2, 1);

INSERT INTO GroupeMdp(idGroupe, idMdp) VALUES (1, 1), (1, 2);
