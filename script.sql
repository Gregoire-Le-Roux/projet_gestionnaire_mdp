DROP TABLE GroupeMdp;
DROP TABLE CompteGroupe;
DROP TABLE Groupe;
DROP TABLE CarnetAdresse;
DROP TABLE MotDePasse;
DROP TABLE Compte;

CREATE DATABASE GestionMdp;

USE GestionMdp;

CREATE TABLE Compte
(
    id int PRIMARY KEY IDENTITY(1, 1) NOT NULL,
    nom varchar(200) NOT NULL,
    prenom varchar(200) NOT NULL,

    mail varchar(250) NOT NULL,
    mdp varchar(500) NOT NULL
);

CREATE TABLE MotDePasse
(
    id int PRIMARY KEY IDENTITY(1, 1) NOT NULL,
    idCompteCreateur int NOT NULL,

    FOREIGN KEY (idCompteCreateur) REFERENCES Compte(id) ON DELETE CASCADE
);

CREATE TABLE CarnetAdresse
(
    id int PRIMARY KEY IDENTITY(1, 1) NOT NULL,
    idCompte int NOT NULL,

    FOREIGN KEY (idCompte) REFERENCES Compte(id) ON DELETE CASCADE
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