/*
===============================================================================
 SchulAppDB
 Datenbank-Setup für die SchulApp
===============================================================================

 Eigenschaften:
 - Mehrfach ausführbar
 - Bestehende Daten werden nicht gelöscht
 - Datenbank, Tabellen, Spalten und Indexe werden nur bei Bedarf erstellt
 - Enthält die Erweiterungen für Benutzerprofile, PingPong und BallSpeed
 - Enthält optionale Testdaten und Kontrollabfragen

===============================================================================
*/

-- ============================================================================
-- 1. DATENBANK ERSTELLEN
-- ============================================================================

USE master;
GO

IF DB_ID(N'SchulAppDB') IS NULL
BEGIN
    CREATE DATABASE SchulAppDB;
END;
GO

USE SchulAppDB;
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;
GO


-- ============================================================================
-- 2. BASIS-TABELLEN
-- ============================================================================

IF OBJECT_ID(N'dbo.Lehrer', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Lehrer
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
END;
GO


IF OBJECT_ID(N'dbo.LehrerInformationen', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.LehrerInformationen
    (
        LehrerInformationId INT IDENTITY(1,1) NOT NULL,
        LehrerId INT NOT NULL,
        Titel NVARCHAR(100) NOT NULL,
        Information NVARCHAR(500) NOT NULL,

        CONSTRAINT PK_LehrerInformationen
            PRIMARY KEY (LehrerInformationId),

        CONSTRAINT UQ_LehrerInformationen_Lehrer_Titel
            UNIQUE (LehrerId, Titel),

        CONSTRAINT CK_LehrerInformationen_Titel
            CHECK (LEN(LTRIM(RTRIM(Titel))) > 0),

        CONSTRAINT CK_LehrerInformationen_Information
            CHECK (LEN(LTRIM(RTRIM(Information))) > 0),

        CONSTRAINT FK_LehrerInformationen_Lehrer
            FOREIGN KEY (LehrerId)
            REFERENCES dbo.Lehrer(LehrerId)
            ON DELETE CASCADE
    );
END;
GO


IF OBJECT_ID(N'dbo.Klassen', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Klassen
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
            REFERENCES dbo.Lehrer(LehrerId)
    );
END;
GO


IF OBJECT_ID(N'dbo.Schueler', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Schueler
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
            REFERENCES dbo.Klassen(KlassenId)
    );
END;
GO


IF OBJECT_ID(N'dbo.Kurse', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Kurse
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
            REFERENCES dbo.Klassen(KlassenId),

        CONSTRAINT FK_Kurse_Lehrer
            FOREIGN KEY (LehrerId)
            REFERENCES dbo.Lehrer(LehrerId)
    );
END;
GO


IF OBJECT_ID(N'dbo.Stundenplan', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Stundenplan
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
            REFERENCES dbo.Kurse(KursId),

        CONSTRAINT CK_Stundenplan_Wochentag
            CHECK (Wochentag BETWEEN 1 AND 5),

        CONSTRAINT CK_Stundenplan_Zeit
            CHECK (Endzeit > Startzeit)
    );
END;
GO

IF OBJECT_ID(N'dbo.Einstellung', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Einstellung
    (
        Id INT IDENTITY(1,1) NOT NULL,
        HintergrundFarbe INT NOT NULL,
        TextFarbe INT NOT NULL,
        BallSpeed INT NOT NULL
            CONSTRAINT DF_Einstellung_BallSpeed DEFAULT (6),

        CONSTRAINT PK_Einstellung
            PRIMARY KEY (Id),

        CONSTRAINT CK_Einstellung_BallSpeed
            CHECK (BallSpeed BETWEEN 1 AND 20)
    );

    -- Erstellt die Standard-Einstellung. Bei einer neuen Tabelle erhält sie Id 1.
    INSERT INTO dbo.Einstellung
    (
        HintergrundFarbe,
        TextFarbe,
        BallSpeed
    )
    VALUES
    (
        -1250856,
        -16777216,
        6
    );
END;
GO

-- ============================================================================
-- 3. BENUTZER UND PROFIL
-- ============================================================================

IF OBJECT_ID(N'dbo.Benutzer', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Benutzer
    (
        Id INT IDENTITY(1,1) NOT NULL,
        Benutzername NVARCHAR(50) NOT NULL,
        PasswortHash NVARCHAR(255) NOT NULL,
        Rolle NVARCHAR(20) NOT NULL
            CONSTRAINT DF_Benutzer_Rolle DEFAULT N'Admin',
        ErstelltAm DATETIME2 NOT NULL,

        Vorname NVARCHAR(100) NULL,
        Nachname NVARCHAR(100) NULL,
        Email NVARCHAR(255) NULL,
        Profilbild VARBINARY(MAX) NULL,

        PingPongSiege INT NOT NULL
            CONSTRAINT DF_Benutzer_PingPongSiege DEFAULT (0),
        PingPongPunkte INT NOT NULL
            CONSTRAINT DF_Benutzer_PingPongPunkte DEFAULT (0),
        PingPongToreErzielt INT NOT NULL
            CONSTRAINT DF_Benutzer_PingPongToreErzielt DEFAULT (0),
        PingPongToreKassiert INT NOT NULL
            CONSTRAINT DF_Benutzer_PingPongToreKassiert DEFAULT (0),

        CONSTRAINT PK_Benutzer
            PRIMARY KEY (Id)
    );
END;
GO

IF COL_LENGTH(N'dbo.Benutzer', N'Rolle') IS NULL
BEGIN
    ALTER TABLE dbo.Benutzer
    ADD Rolle NVARCHAR(20) NOT NULL
        CONSTRAINT DF_Benutzer_Rolle DEFAULT N'Admin';
END;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_Benutzer_Benutzername'
      AND object_id = OBJECT_ID(N'[dbo].[Benutzer]')
)
BEGIN
    CREATE UNIQUE INDEX IX_Benutzer_Benutzername
        ON dbo.Benutzer(Benutzername);
END;
GO

-- Kompatibilität mit bereits vorhandenen Datenbanken
IF COL_LENGTH(N'dbo.Benutzer', N'Vorname') IS NULL
BEGIN
    ALTER TABLE dbo.Benutzer
    ADD Vorname NVARCHAR(100) NULL;
END;
GO

IF COL_LENGTH(N'dbo.Benutzer', N'Nachname') IS NULL
BEGIN
    ALTER TABLE dbo.Benutzer
    ADD Nachname NVARCHAR(100) NULL;
END;
GO

IF COL_LENGTH(N'dbo.Benutzer', N'Email') IS NULL
BEGIN
    ALTER TABLE dbo.Benutzer
    ADD Email NVARCHAR(255) NULL;
END;
GO


IF COL_LENGTH(N'dbo.Benutzer', N'Profilbild') IS NULL
BEGIN
    ALTER TABLE dbo.Benutzer
    ADD Profilbild VARBINARY(MAX) NULL;
END;
GO
-- ============================================================================
-- 4. PINGPONG UND BESTENLISTE
-- ============================================================================

IF COL_LENGTH(N'dbo.Benutzer', N'PingPongSiege') IS NULL
BEGIN
    ALTER TABLE dbo.Benutzer
    ADD PingPongSiege INT NOT NULL
        CONSTRAINT DF_Benutzer_PingPongSiege DEFAULT (0) WITH VALUES;
END;
GO

IF COL_LENGTH(N'dbo.Benutzer', N'PingPongPunkte') IS NULL
BEGIN
    ALTER TABLE dbo.Benutzer
    ADD PingPongPunkte INT NOT NULL
        CONSTRAINT DF_Benutzer_PingPongPunkte DEFAULT (0) WITH VALUES;
END;
GO

IF COL_LENGTH(N'dbo.Benutzer', N'PingPongToreErzielt') IS NULL
BEGIN
    ALTER TABLE dbo.Benutzer
    ADD PingPongToreErzielt INT NOT NULL
        CONSTRAINT DF_Benutzer_PingPongToreErzielt DEFAULT (0) WITH VALUES;
END;
GO

IF COL_LENGTH(N'dbo.Benutzer', N'PingPongToreKassiert') IS NULL
BEGIN
    ALTER TABLE dbo.Benutzer
    ADD PingPongToreKassiert INT NOT NULL
        CONSTRAINT DF_Benutzer_PingPongToreKassiert DEFAULT (0) WITH VALUES;
END;
GO

IF OBJECT_ID(N'dbo.PingPongSpiel', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.PingPongSpiel
    (
        Id INT IDENTITY(1,1) NOT NULL,
        SpielerLinksId INT NOT NULL,
        SpielerRechtsId INT NOT NULL,
        ToreLinks INT NOT NULL,
        ToreRechts INT NOT NULL,
        GewinnerId INT NOT NULL,
        GespieltAm DATETIME2 NOT NULL
            CONSTRAINT DF_PingPongSpiel_GespieltAm
            DEFAULT SYSUTCDATETIME(),

        CONSTRAINT PK_PingPongSpiel
            PRIMARY KEY (Id),

        CONSTRAINT FK_PingPongSpiel_SpielerLinks
            FOREIGN KEY (SpielerLinksId)
            REFERENCES dbo.Benutzer(Id),

        CONSTRAINT FK_PingPongSpiel_SpielerRechts
            FOREIGN KEY (SpielerRechtsId)
            REFERENCES dbo.Benutzer(Id),

        CONSTRAINT FK_PingPongSpiel_Gewinner
            FOREIGN KEY (GewinnerId)
            REFERENCES dbo.Benutzer(Id),

        CONSTRAINT CK_PingPongSpiel_Ergebnis
            CHECK
            (
                ToreLinks BETWEEN 0 AND 5
                AND ToreRechts BETWEEN 0 AND 5
                AND ToreLinks <> ToreRechts
                AND (ToreLinks = 5 OR ToreRechts = 5)
            )
    );
END;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE object_id = OBJECT_ID(N'dbo.PingPongSpiel')
      AND name = N'IX_PingPongSpiel_GespieltAm'
)
BEGIN
    CREATE INDEX IX_PingPongSpiel_GespieltAm
        ON dbo.PingPongSpiel(GespieltAm DESC);
END;
GO

-- ============================================================================
-- 5. PINGPONG-BALLGESCHWINDIGKEIT
-- ============================================================================
-- Standard: 6
-- Gültiger Bereich: 1 bis 20
-- X- und Y-Geschwindigkeit verwenden in der Anwendung denselben Wert.

IF COL_LENGTH(N'dbo.Einstellung', N'BallSpeed') IS NULL
BEGIN
    ALTER TABLE dbo.Einstellung
    ADD BallSpeed INT NOT NULL
        CONSTRAINT DF_Einstellung_BallSpeed DEFAULT (6);
END;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.check_constraints
    WHERE name = N'CK_Einstellung_BallSpeed'
      AND parent_object_id = OBJECT_ID(N'dbo.Einstellung')
)
BEGIN
    ALTER TABLE dbo.Einstellung
    ADD CONSTRAINT CK_Einstellung_BallSpeed
        CHECK (BallSpeed BETWEEN 1 AND 20);
END;
GO
-- ============================================================================
-- 6. INDEXE
-- ============================================================================
-- Fremdschlüssel erhalten passende Indexe.
-- Falls eine normale Statistik denselben Namen wie der gewünschte Index hat,
-- wird nur diese Statistik entfernt. Bestehende Indexe werden nicht gelöscht.

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE object_id = OBJECT_ID(N'dbo.LehrerInformationen')
      AND name = N'IX_LehrerInformationen_LehrerId'
)
BEGIN
    IF EXISTS
    (
        SELECT 1
        FROM sys.stats
        WHERE object_id = OBJECT_ID(N'dbo.LehrerInformationen')
          AND name = N'IX_LehrerInformationen_LehrerId'
    )
    BEGIN
        DROP STATISTICS dbo.LehrerInformationen.IX_LehrerInformationen_LehrerId;
    END;

    CREATE INDEX IX_LehrerInformationen_LehrerId
        ON dbo.LehrerInformationen(LehrerId);
END;
GO


IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE object_id = OBJECT_ID(N'dbo.Klassen')
      AND name = N'IX_Klassen_KlassenlehrerId'
)
BEGIN
    IF EXISTS
    (
        SELECT 1
        FROM sys.stats
        WHERE object_id = OBJECT_ID(N'dbo.Klassen')
          AND name = N'IX_Klassen_KlassenlehrerId'
    )
    BEGIN
        DROP STATISTICS dbo.Klassen.IX_Klassen_KlassenlehrerId;
    END;

    CREATE INDEX IX_Klassen_KlassenlehrerId
        ON dbo.Klassen(KlassenlehrerId);
END;
GO


IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE object_id = OBJECT_ID(N'dbo.Schueler')
      AND name = N'IX_Schueler_KlasseId'
)
BEGIN
    IF EXISTS
    (
        SELECT 1
        FROM sys.stats
        WHERE object_id = OBJECT_ID(N'dbo.Schueler')
          AND name = N'IX_Schueler_KlasseId'
    )
    BEGIN
        DROP STATISTICS dbo.Schueler.IX_Schueler_KlasseId;
    END;

    CREATE INDEX IX_Schueler_KlasseId
        ON dbo.Schueler(KlasseId);
END;
GO


IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE object_id = OBJECT_ID(N'dbo.Kurse')
      AND name = N'IX_Kurse_KlasseId'
)
BEGIN
    IF EXISTS
    (
        SELECT 1
        FROM sys.stats
        WHERE object_id = OBJECT_ID(N'dbo.Kurse')
          AND name = N'IX_Kurse_KlasseId'
    )
    BEGIN
        DROP STATISTICS dbo.Kurse.IX_Kurse_KlasseId;
    END;

    CREATE INDEX IX_Kurse_KlasseId
        ON dbo.Kurse(KlasseId);
END;
GO


IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE object_id = OBJECT_ID(N'dbo.Kurse')
      AND name = N'IX_Kurse_LehrerId'
)
BEGIN
    IF EXISTS
    (
        SELECT 1
        FROM sys.stats
        WHERE object_id = OBJECT_ID(N'dbo.Kurse')
          AND name = N'IX_Kurse_LehrerId'
    )
    BEGIN
        DROP STATISTICS dbo.Kurse.IX_Kurse_LehrerId;
    END;

    CREATE INDEX IX_Kurse_LehrerId
        ON dbo.Kurse(LehrerId);
END;
GO


IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE object_id = OBJECT_ID(N'dbo.Stundenplan')
      AND name = N'IX_Stundenplan_KursId'
)
BEGIN
    IF EXISTS
    (
        SELECT 1
        FROM sys.stats
        WHERE object_id = OBJECT_ID(N'dbo.Stundenplan')
          AND name = N'IX_Stundenplan_KursId'
    )
    BEGIN
        DROP STATISTICS dbo.Stundenplan.IX_Stundenplan_KursId;
    END;

    CREATE INDEX IX_Stundenplan_KursId
        ON dbo.Stundenplan(KursId);
END;
GO


IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE object_id = OBJECT_ID(N'dbo.Stundenplan')
      AND name = N'IX_Stundenplan_Wochentag_Startzeit'
)
BEGIN
    IF EXISTS
    (
        SELECT 1
        FROM sys.stats
        WHERE object_id = OBJECT_ID(N'dbo.Stundenplan')
          AND name = N'IX_Stundenplan_Wochentag_Startzeit'
    )
    BEGIN
        DROP STATISTICS dbo.Stundenplan.IX_Stundenplan_Wochentag_Startzeit;
    END;

    CREATE INDEX IX_Stundenplan_Wochentag_Startzeit
        ON dbo.Stundenplan(Wochentag, Startzeit);
END;
GO


-- ============================================================================
-- 7. TESTDATEN
-- ============================================================================
-- Die Testdaten werden nur eingefügt, wenn sie noch nicht vorhanden sind.

IF NOT EXISTS
(
    SELECT 1
    FROM dbo.Lehrer
    WHERE Email = N'markus.mueller@schule.ch'
)
BEGIN
    INSERT INTO dbo.Lehrer (Name, Email, Telefon)
    VALUES (N'Markus Müller', N'markus.mueller@schule.ch', N'071 111 11 11');
END;


IF NOT EXISTS
(
    SELECT 1
    FROM dbo.Lehrer
    WHERE Email = N'anna.keller@schule.ch'
)
BEGIN
    INSERT INTO dbo.Lehrer (Name, Email, Telefon)
    VALUES (N'Anna Keller', N'anna.keller@schule.ch', N'071 222 22 22');
END;
GO


DECLARE @MarkusId INT =
(
    SELECT TOP (1) LehrerId
    FROM dbo.Lehrer
    WHERE Email = N'markus.mueller@schule.ch'
);

DECLARE @AnnaId INT =
(
    SELECT TOP (1) LehrerId
    FROM dbo.Lehrer
    WHERE Email = N'anna.keller@schule.ch'
);

IF NOT EXISTS
(
    SELECT 1
    FROM dbo.Klassen
    WHERE Bezeichnung = N'AP24a'
)
BEGIN
    INSERT INTO dbo.Klassen (Bezeichnung, KlassenlehrerId)
    VALUES (N'AP24a', @MarkusId);
END;

IF NOT EXISTS
(
    SELECT 1
    FROM dbo.Klassen
    WHERE Bezeichnung = N'AP24b'
)
BEGIN
    INSERT INTO dbo.Klassen (Bezeichnung, KlassenlehrerId)
    VALUES (N'AP24b', @AnnaId);
END;
GO


DECLARE @KlasseAP24a INT =
(
    SELECT KlassenId
    FROM dbo.Klassen
    WHERE Bezeichnung = N'AP24a'
);

DECLARE @KlasseAP24b INT =
(
    SELECT KlassenId
    FROM dbo.Klassen
    WHERE Bezeichnung = N'AP24b'
);

IF NOT EXISTS
(
    SELECT 1
    FROM dbo.Schueler
    WHERE Name = N'Max Muster'
      AND KlasseId = @KlasseAP24a
)
BEGIN
    INSERT INTO dbo.Schueler (Name, KlasseId)
    VALUES (N'Max Muster', @KlasseAP24a);
END;

IF NOT EXISTS
(
    SELECT 1
    FROM dbo.Schueler
    WHERE Name = N'Lea Beispiel'
      AND KlasseId = @KlasseAP24a
)
BEGIN
    INSERT INTO dbo.Schueler (Name, KlasseId)
    VALUES (N'Lea Beispiel', @KlasseAP24a);
END;

IF NOT EXISTS
(
    SELECT 1
    FROM dbo.Schueler
    WHERE Name = N'Tom Test'
      AND KlasseId = @KlasseAP24b
)
BEGIN
    INSERT INTO dbo.Schueler (Name, KlasseId)
    VALUES (N'Tom Test', @KlasseAP24b);
END;
GO


DECLARE @MarkusId INT =
(
    SELECT TOP (1) LehrerId
    FROM dbo.Lehrer
    WHERE Email = N'markus.mueller@schule.ch'
);

DECLARE @AnnaId INT =
(
    SELECT TOP (1) LehrerId
    FROM dbo.Lehrer
    WHERE Email = N'anna.keller@schule.ch'
);

DECLARE @KlasseAP24a INT =
(
    SELECT KlassenId
    FROM dbo.Klassen
    WHERE Bezeichnung = N'AP24a'
);

DECLARE @KlasseAP24b INT =
(
    SELECT KlassenId
    FROM dbo.Klassen
    WHERE Bezeichnung = N'AP24b'
);

IF NOT EXISTS
(
    SELECT 1
    FROM dbo.Kurse
    WHERE Name = N'Mathematik'
      AND KlasseId = @KlasseAP24a
      AND LehrerId = @MarkusId
)
BEGIN
    INSERT INTO dbo.Kurse (Name, KlasseId, LehrerId)
    VALUES (N'Mathematik', @KlasseAP24a, @MarkusId);
END;

IF NOT EXISTS
(
    SELECT 1
    FROM dbo.Kurse
    WHERE Name = N'Programmieren'
      AND KlasseId = @KlasseAP24a
      AND LehrerId = @AnnaId
)
BEGIN
    INSERT INTO dbo.Kurse (Name, KlasseId, LehrerId)
    VALUES (N'Programmieren', @KlasseAP24a, @AnnaId);
END;

IF NOT EXISTS
(
    SELECT 1
    FROM dbo.Kurse
    WHERE Name = N'Mathematik'
      AND KlasseId = @KlasseAP24b
      AND LehrerId = @MarkusId
)
BEGIN
    INSERT INTO dbo.Kurse (Name, KlasseId, LehrerId)
    VALUES (N'Mathematik', @KlasseAP24b, @MarkusId);
END;
GO


DECLARE @MatheAP24a INT =
(
    SELECT TOP (1) ku.KursId
    FROM dbo.Kurse AS ku
    INNER JOIN dbo.Klassen AS kl
        ON kl.KlassenId = ku.KlasseId
    WHERE ku.Name = N'Mathematik'
      AND kl.Bezeichnung = N'AP24a'
);

DECLARE @ProgrammierenAP24a INT =
(
    SELECT TOP (1) ku.KursId
    FROM dbo.Kurse AS ku
    INNER JOIN dbo.Klassen AS kl
        ON kl.KlassenId = ku.KlasseId
    WHERE ku.Name = N'Programmieren'
      AND kl.Bezeichnung = N'AP24a'
);

DECLARE @MatheAP24b INT =
(
    SELECT TOP (1) ku.KursId
    FROM dbo.Kurse AS ku
    INNER JOIN dbo.Klassen AS kl
        ON kl.KlassenId = ku.KlasseId
    WHERE ku.Name = N'Mathematik'
      AND kl.Bezeichnung = N'AP24b'
);

IF NOT EXISTS
(
    SELECT 1
    FROM dbo.Stundenplan
    WHERE KursId = @MatheAP24a
      AND Wochentag = 1
      AND Startzeit = CAST('08:00' AS TIME(0))
)
BEGIN
    INSERT INTO dbo.Stundenplan
        (KursId, Wochentag, Startzeit, Endzeit, Raum)
    VALUES
        (@MatheAP24a, 1, '08:00', '08:45', N'101');
END;

IF NOT EXISTS
(
    SELECT 1
    FROM dbo.Stundenplan
    WHERE KursId = @ProgrammierenAP24a
      AND Wochentag = 1
      AND Startzeit = CAST('09:00' AS TIME(0))
)
BEGIN
    INSERT INTO dbo.Stundenplan
        (KursId, Wochentag, Startzeit, Endzeit, Raum)
    VALUES
        (@ProgrammierenAP24a, 1, '09:00', '09:45', N'204');
END;

IF NOT EXISTS
(
    SELECT 1
    FROM dbo.Stundenplan
    WHERE KursId = @MatheAP24b
      AND Wochentag = 2
      AND Startzeit = CAST('08:00' AS TIME(0))
)
BEGIN
    INSERT INTO dbo.Stundenplan
        (KursId, Wochentag, Startzeit, Endzeit, Raum)
    VALUES
        (@MatheAP24b, 2, '08:00', '08:45', N'101');
END;
GO

IF NOT EXISTS (
    SELECT 1
    FROM dbo.Benutzer
    WHERE Benutzername = N'PingPongTest'
)
BEGIN
    INSERT INTO dbo.Benutzer (
        Benutzername,
        PasswortHash,
        Rolle,
        ErstelltAm
    )
    VALUES (
        N'PingPongTest',
        N'TESTUSER_NICHT_ZUM_LOGIN',
        N'Benutzer',
        SYSDATETIME()
    );
END;

-- ============================================================================
-- 8. KONTROLLABFRAGEN
-- ============================================================================

SELECT
    LehrerId,
    Name,
    Email,
    Telefon
FROM dbo.Lehrer
ORDER BY Name;

SELECT
    KlassenId,
    Bezeichnung,
    KlassenlehrerId
FROM dbo.Klassen
ORDER BY Bezeichnung;

SELECT
    SchuelerId,
    Name,
    KlasseId
FROM dbo.Schueler
ORDER BY Name;

SELECT
    KursId,
    Name,
    KlasseId,
    LehrerId
FROM dbo.Kurse
ORDER BY Name, KursId;

SELECT
    StundenplanId,
    KursId,
    Wochentag,
    Startzeit,
    Endzeit,
    Raum
FROM dbo.Stundenplan
ORDER BY Wochentag, Startzeit;
GO

-- ============================================================================
-- SETUP ABGESCHLOSSEN
-- ============================================================================
