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

    titre varchar(100) NOT NULL,
    login varchar(200) NOT NULL,
    mdp varchar(500) NOT NULL,
    url varchar(500) NOT NULL,
    dateExpiration date NOT NULL,
    description varchar(2000) DEFAULT NULL,

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

SET IDENTITY_INSERT Compte ON;
INSERT INTO Compte(id, nom, prenom, mail, mdp, hashCle) VALUES

-- Infos chiffré AES
(1, 'i3ELcdA4JqnCV67nfPMTLA==', 'JX+8JR6Tk90VR2KJBzqXbQ==', 'GuH0Yx0bW/onRUAQr5+u5g==', '$2a$11$ur8Y92GjGPM6ac1qKs.i6OI89M1NbOLybqvILOoI1OYaOC7Uj/eT.', '$2a$11$CMpQ93U1EcdEwV1U3yQGx.nCWbcEsYUJrezHjYhBNMgdHJFanmeU2'), 
(2, 'i3ELcdA4JqnCV67nfPMTLA==', 'JX+8JR6Tk90VR2KJBzqXbQ==', 'GuH0Yx0bW/onRUAQr5+u5g==', '$2a$11$ur8Y92GjGPM6ac1qKs.i6OI89M1NbOLybqvILOoI1OYaOC7Uj/eT.', '$2a$11$CMpQ93U1EcdEwV1U3yQGx.nCWbcEsYUJrezHjYhBNMgdHJFanmeU2');
SET IDENTITY_INSERT Compte OFF;

SET IDENTITY_INSERT MotDePasse ON;
INSERT INTO MotDePasse(id, titre, login, mdp, url, dateExpiration, description, idCompteCreateur) VALUES
(1, 'Amazon', 'login', 'mot de passe', 'http:/amazon.fr', '1996-08-21', 'aze', 1),
(2, 'Amazon', 'login', 'mot de passe', 'http:/youtube.fr', '1996-08-21', 'coucou', 2);
SET IDENTITY_INSERT MotDePasse OFF;

INSERT INTO CarnetAdresse(idCompte, idCompteCarnet) VALUES (1, 2), (2, 1);

SET IDENTITY_INSERT Groupe ON;
INSERT INTO Groupe(id, idCompteCreateur, titre) VALUES (1, 2, 'Groupe 1');
SET IDENTITY_INSERT Groupe OFF;

INSERT INTO CompteGroupe(idCompte, idGroupe) VALUES (1, 1), (2, 1);

INSERT INTO GroupeMdp(idGroupe, idMdp) VALUES (1, 1), (1, 2);
