CREATE DATABASE
IF NOT EXISTS MinionsDB;

USE MinionsDB;

CREATE TABLE Countries(
Id INT AUTO_INCREMENT PRIMARY KEY,
Name VARCHAR(255) NOT NULL);

INSERT INTO Countries(Name)
VALUES ("USA"),("UK"),("Germany"),
("Italy"),("France");

CREATE TABLE Towns(
Id INT AUTO_INCREMENT PRIMARY KEY,
Name VARCHAR(255) NOT NULL,
CountryCode INT,
FOREIGN KEY (CountryCode)
REFERENCES Countries(Id)
);

INSERT INTO Towns(Name,CountryCode)
VALUES ("New York",1),("London",2),
("Berlin",3),("Rome",4),("Paris",5);

CREATE TABLE Minions(
Id INT AUTO_INCREMENT PRIMARY KEY,
Name VARCHAR(255) NOT NULL,
Age INT NOT NULL,
TownId INT,
FOREIGN KEY (TownId) REFERENCES
Towns(Id));

INSERT INTO Minions(Name,Age,TownId)
VALUES ("Bob",10,1),("Kevin",10,2),("Stuart",10,3),
("Tim",10,4),("Kevin",10,5);


CREATE TABLE EvilnessFactors(
Id INT AUTO_INCREMENT PRIMARY KEY,
Name VARCHAR(255) NOT NULL);

INSERT INTO EvilnessFactors(Name)
VALUES ("super good"),("good"),
("bad"),("evil"),("super evil");

CREATE TABLE Villains(
Id INT AUTO_INCREMENT PRIMARY KEY,
Name VARCHAR(255) NOT NULL,
EvilnessFactorId INT,
FOREIGN KEY (EvilnessFactorId)
REFERENCES EvilnessFactors(Id));

INSERT INTO Villains(Name,EvilnessFactorId)
VALUES ("Gru",5),("Victor",5),
("Balthazar",5),("Scarlet",5),("Jilly",5);

CREATE TABLE MinionsVillains(
MinionId INT,
VillainId INT,
FOREIGN KEY (MinionId)
REFERENCES Minions(Id),
FOREIGN KEY (VillainId)
REFERENCES Villains(Id),
PRIMARY KEY (MinionId,VillainId));

INSERT INTO MinionsVillains
VALUES (1,5),(2,5),(3,5),(4,5),(5,5);
