USE master;
GO

IF DB_ID(N'SchulAppDB') IS NULL
BEGIN
    CREATE DATABASE SchulAppDB;
END
GO

USE SchulAppDB;
GO


/* =========================================================
   LEHRER
   ========================================================= */

CREATE TABLE Lehrer
(
    LehrerId INT IDENTITY(1,1) NOT NULL,
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(255) NULL,
    Telefon NVARCHAR(50) NULL,

    CONSTRAINT PK_Lehrer
        PRIMARY KEY (LehrerId),

    CONSTRAINT CK_Lehrer_Name
        CHECK (LEN(LTRIM(RTRIM(Name))) > 0)
);
GO


/* =========================================================
   KLASSEN
   Eine Klasse kann einen Klassenlehrer haben.
   ========================================================= */

CREATE TABLE Klassen
(
    KlassenId INT IDENTITY(1,1) NOT NULL,
    Bezeichnung NVARCHAR(50) NOT NULL,
    KlassenlehrerId INT NULL,

    CONSTRAINT PK_Klassen
        PRIMARY KEY (KlassenId),

    CONSTRAINT UQ_Klassen_Bezeichnung
        UNIQUE (Bezeichnung),

    CONSTRAINT CK_Klassen_Bezeichnung
        CHECK (LEN(LTRIM(RTRIM(Bezeichnung))) > 0),

    CONSTRAINT FK_Klassen_Lehrer
        FOREIGN KEY (KlassenlehrerId)
        REFERENCES Lehrer(LehrerId)
);
GO


/* =========================================================
   SCHUELER
   Jeder Schüler gehört zu einer Klasse.
   ========================================================= */

CREATE TABLE Schueler
(
    SchuelerId INT IDENTITY(1,1) NOT NULL,
    Name NVARCHAR(100) NOT NULL,
    KlasseId INT NOT NULL,

    CONSTRAINT PK_Schueler
        PRIMARY KEY (SchuelerId),

    CONSTRAINT CK_Schueler_Name
        CHECK (LEN(LTRIM(RTRIM(Name))) > 0),

    CONSTRAINT FK_Schueler_Klassen
        FOREIGN KEY (KlasseId)
        REFERENCES Klassen(KlassenId)
);
GO


/* =========================================================
   KURSE
   Ein Kurs gehört zu einer Klasse und einem Lehrer.
   ========================================================= */

CREATE TABLE Kurse
(
    KursId INT IDENTITY(1,1) NOT NULL,
    Name NVARCHAR(100) NOT NULL,
    KlasseId INT NOT NULL,
    LehrerId INT NOT NULL,

    CONSTRAINT PK_Kurse
        PRIMARY KEY (KursId),

    CONSTRAINT CK_Kurse_Name
        CHECK (LEN(LTRIM(RTRIM(Name))) > 0),

    CONSTRAINT FK_Kurse_Klassen
        FOREIGN KEY (KlasseId)
        REFERENCES Klassen(KlassenId),

    CONSTRAINT FK_Kurse_Lehrer
        FOREIGN KEY (LehrerId)
        REFERENCES Lehrer(LehrerId)
);
GO


/* =========================================================
   STUNDENPLAN

   Wochentag:
   1 = Montag
   2 = Dienstag
   3 = Mittwoch
   4 = Donnerstag
   5 = Freitag
   ========================================================= */

CREATE TABLE Stundenplan
(
    StundenplanId INT IDENTITY(1,1) NOT NULL,
    KursId INT NOT NULL,
    Wochentag TINYINT NOT NULL,
    Startzeit TIME(0) NOT NULL,
    Endzeit TIME(0) NOT NULL,
    Raum NVARCHAR(50) NULL,

    CONSTRAINT PK_Stundenplan
        PRIMARY KEY (StundenplanId),

    CONSTRAINT FK_Stundenplan_Kurse
        FOREIGN KEY (KursId)
        REFERENCES Kurse(KursId),

    CONSTRAINT CK_Stundenplan_Wochentag
        CHECK (Wochentag BETWEEN 1 AND 5),

    CONSTRAINT CK_Stundenplan_Zeit
        CHECK (Endzeit > Startzeit)
);
GO


/* =========================================================
   INDEXE FÜR FOREIGN KEYS
   ========================================================= */

CREATE INDEX IX_Schueler_KlasseId
ON Schueler(KlasseId);
GO

CREATE INDEX IX_Klassen_KlassenlehrerId
ON Klassen(KlassenlehrerId);
GO

CREATE INDEX IX_Kurse_KlasseId
ON Kurse(KlasseId);
GO

CREATE INDEX IX_Kurse_LehrerId
ON Kurse(LehrerId);
GO

CREATE INDEX IX_Stundenplan_KursId
ON Stundenplan(KursId);
GO


/* =========================================================
   TESTDATEN
   ========================================================= */

INSERT INTO Lehrer (Name, Email, Telefon)
VALUES
    (N'Markus Müller', N'markus.mueller@schule.ch', N'071 111 11 11'),
    (N'Anna Keller', N'anna.keller@schule.ch', N'071 222 22 22');
GO


INSERT INTO Klassen (Bezeichnung, KlassenlehrerId)
VALUES
    (N'AP24a', 1),
    (N'AP24b', 2);
GO


INSERT INTO Schueler (Name, KlasseId)
VALUES
    (N'Max Muster', 1),
    (N'Lea Beispiel', 1),
    (N'Tom Test', 2);
GO


INSERT INTO Kurse (Name, KlasseId, LehrerId)
VALUES
    (N'Mathematik', 1, 1),
    (N'Programmieren', 1, 2),
    (N'Mathematik', 2, 1);
GO


INSERT INTO Stundenplan
(
    KursId,
    Wochentag,
    Startzeit,
    Endzeit,
    Raum
)
VALUES
    (1, 1, '08:00', '08:45', N'101'),
    (2, 1, '09:00', '09:45', N'204'),
    (3, 2, '08:00', '08:45', N'101');
GO