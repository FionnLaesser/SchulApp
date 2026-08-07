/* =========================================================
   SchulAppDB
   Mehrfach ausführbares Setup für die SchulApp
   Bestehende Daten werden nicht gelöscht.
   ========================================================= */

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
GO

/* =========================================================
   TABELLEN
   ========================================================= */

IF OBJECT_ID(N'dbo.Lehrer', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Lehrer
    (
        LehrerId INT IDENTITY(1,1) NOT NULL,
        Name NVARCHAR(100) NOT NULL,
        Email NVARCHAR(255) NULL,
        Telefon NVARCHAR(50) NULL,

        CONSTRAINT PK_Lehrer PRIMARY KEY (LehrerId),
        CONSTRAINT CK_Lehrer_Name CHECK (LEN(LTRIM(RTRIM(Name))) > 0)
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

        CONSTRAINT PK_LehrerInformationen PRIMARY KEY (LehrerInformationId),
        CONSTRAINT UQ_LehrerInformationen_Lehrer_Titel UNIQUE (LehrerId, Titel),
        CONSTRAINT CK_LehrerInformationen_Titel CHECK (LEN(LTRIM(RTRIM(Titel))) > 0),
        CONSTRAINT CK_LehrerInformationen_Information CHECK (LEN(LTRIM(RTRIM(Information))) > 0),
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

        CONSTRAINT PK_Klassen PRIMARY KEY (KlassenId),
        CONSTRAINT UQ_Klassen_Bezeichnung UNIQUE (Bezeichnung),
        CONSTRAINT CK_Klassen_Bezeichnung CHECK (LEN(LTRIM(RTRIM(Bezeichnung))) > 0),
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

        CONSTRAINT PK_Schueler PRIMARY KEY (SchuelerId),
        CONSTRAINT CK_Schueler_Name CHECK (LEN(LTRIM(RTRIM(Name))) > 0),
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

        CONSTRAINT PK_Kurse PRIMARY KEY (KursId),
        CONSTRAINT CK_Kurse_Name CHECK (LEN(LTRIM(RTRIM(Name))) > 0),
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

        CONSTRAINT PK_Stundenplan PRIMARY KEY (StundenplanId),
        CONSTRAINT FK_Stundenplan_Kurse
            FOREIGN KEY (KursId)
            REFERENCES dbo.Kurse(KursId),
        CONSTRAINT CK_Stundenplan_Wochentag CHECK (Wochentag BETWEEN 1 AND 5),
        CONSTRAINT CK_Stundenplan_Zeit CHECK (Endzeit > Startzeit)
    );
END;
GO

/* =========================================================
   INDEXE FÜR FOREIGN KEYS
   ========================================================= */

IF NOT EXISTS
(
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_LehrerInformationen_LehrerId'
      AND object_id = OBJECT_ID(N'dbo.LehrerInformationen')
)
BEGIN
    CREATE INDEX IX_LehrerInformationen_LehrerId
    ON dbo.LehrerInformationen(LehrerId);
END;
GO

IF NOT EXISTS
(
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_Klassen_KlassenlehrerId'
      AND object_id = OBJECT_ID(N'dbo.Klassen')
)
BEGIN
    CREATE INDEX IX_Klassen_KlassenlehrerId
    ON dbo.Klassen(KlassenlehrerId);
END;
GO

IF NOT EXISTS
(
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_Schueler_KlasseId'
      AND object_id = OBJECT_ID(N'dbo.Schueler')
)
BEGIN
    CREATE INDEX IX_Schueler_KlasseId
    ON dbo.Schueler(KlasseId);
END;
GO

IF NOT EXISTS
(
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_Kurse_KlasseId'
      AND object_id = OBJECT_ID(N'dbo.Kurse')
)
BEGIN
    CREATE INDEX IX_Kurse_KlasseId
    ON dbo.Kurse(KlasseId);
END;
GO

IF NOT EXISTS
(
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_Kurse_LehrerId'
      AND object_id = OBJECT_ID(N'dbo.Kurse')
)
BEGIN
    CREATE INDEX IX_Kurse_LehrerId
    ON dbo.Kurse(LehrerId);
END;
GO

IF NOT EXISTS
(
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_Stundenplan_KursId'
      AND object_id = OBJECT_ID(N'dbo.Stundenplan')
)
BEGIN
    CREATE INDEX IX_Stundenplan_KursId
    ON dbo.Stundenplan(KursId);
END;
GO

IF NOT EXISTS
(
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_Stundenplan_Wochentag_Startzeit'
      AND object_id = OBJECT_ID(N'dbo.Stundenplan')
)
BEGIN
    CREATE INDEX IX_Stundenplan_Wochentag_Startzeit
    ON dbo.Stundenplan(Wochentag, Startzeit);
END;
GO

/* =========================================================
   TESTDATEN
   Nur einfügen, wenn sie noch nicht vorhanden sind.
   ========================================================= */

IF NOT EXISTS
(
    SELECT 1 FROM dbo.Lehrer
    WHERE Email = N'markus.mueller@schule.ch'
)
BEGIN
    INSERT INTO dbo.Lehrer (Name, Email, Telefon)
    VALUES (N'Markus Müller', N'markus.mueller@schule.ch', N'071 111 11 11');
END;

IF NOT EXISTS
(
    SELECT 1 FROM dbo.Lehrer
    WHERE Email = N'anna.keller@schule.ch'
)
BEGIN
    INSERT INTO dbo.Lehrer (Name, Email, Telefon)
    VALUES (N'Anna Keller', N'anna.keller@schule.ch', N'071 222 22 22');
END;
GO

DECLARE @MarkusId INT =
(
    SELECT TOP 1 LehrerId
    FROM dbo.Lehrer
    WHERE Email = N'markus.mueller@schule.ch'
);

DECLARE @AnnaId INT =
(
    SELECT TOP 1 LehrerId
    FROM dbo.Lehrer
    WHERE Email = N'anna.keller@schule.ch'
);

IF NOT EXISTS (SELECT 1 FROM dbo.Klassen WHERE Bezeichnung = N'AP24a')
BEGIN
    INSERT INTO dbo.Klassen (Bezeichnung, KlassenlehrerId)
    VALUES (N'AP24a', @MarkusId);
END;

IF NOT EXISTS (SELECT 1 FROM dbo.Klassen WHERE Bezeichnung = N'AP24b')
BEGIN
    INSERT INTO dbo.Klassen (Bezeichnung, KlassenlehrerId)
    VALUES (N'AP24b', @AnnaId);
END;
GO

DECLARE @KlasseAP24a INT =
(
    SELECT KlassenId FROM dbo.Klassen WHERE Bezeichnung = N'AP24a'
);

DECLARE @KlasseAP24b INT =
(
    SELECT KlassenId FROM dbo.Klassen WHERE Bezeichnung = N'AP24b'
);

IF NOT EXISTS
(
    SELECT 1 FROM dbo.Schueler
    WHERE Name = N'Max Muster'
      AND KlasseId = @KlasseAP24a
)
BEGIN
    INSERT INTO dbo.Schueler (Name, KlasseId)
    VALUES (N'Max Muster', @KlasseAP24a);
END;

IF NOT EXISTS
(
    SELECT 1 FROM dbo.Schueler
    WHERE Name = N'Lea Beispiel'
      AND KlasseId = @KlasseAP24a
)
BEGIN
    INSERT INTO dbo.Schueler (Name, KlasseId)
    VALUES (N'Lea Beispiel', @KlasseAP24a);
END;

IF NOT EXISTS
(
    SELECT 1 FROM dbo.Schueler
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
    SELECT TOP 1 LehrerId FROM dbo.Lehrer
    WHERE Email = N'markus.mueller@schule.ch'
);

DECLARE @AnnaId INT =
(
    SELECT TOP 1 LehrerId FROM dbo.Lehrer
    WHERE Email = N'anna.keller@schule.ch'
);

DECLARE @KlasseAP24a INT =
(
    SELECT KlassenId FROM dbo.Klassen WHERE Bezeichnung = N'AP24a'
);

DECLARE @KlasseAP24b INT =
(
    SELECT KlassenId FROM dbo.Klassen WHERE Bezeichnung = N'AP24b'
);

IF NOT EXISTS
(
    SELECT 1 FROM dbo.Kurse
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
    SELECT 1 FROM dbo.Kurse
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
    SELECT 1 FROM dbo.Kurse
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
    SELECT TOP 1 ku.KursId
    FROM dbo.Kurse AS ku
    INNER JOIN dbo.Klassen AS kl ON ku.KlasseId = kl.KlassenId
    WHERE ku.Name = N'Mathematik'
      AND kl.Bezeichnung = N'AP24a'
);

DECLARE @ProgrammierenAP24a INT =
(
    SELECT TOP 1 ku.KursId
    FROM dbo.Kurse AS ku
    INNER JOIN dbo.Klassen AS kl ON ku.KlasseId = kl.KlassenId
    WHERE ku.Name = N'Programmieren'
      AND kl.Bezeichnung = N'AP24a'
);

DECLARE @MatheAP24b INT =
(
    SELECT TOP 1 ku.KursId
    FROM dbo.Kurse AS ku
    INNER JOIN dbo.Klassen AS kl ON ku.KlasseId = kl.KlassenId
    WHERE ku.Name = N'Mathematik'
      AND kl.Bezeichnung = N'AP24b'
);

IF NOT EXISTS
(
    SELECT 1 FROM dbo.Stundenplan
    WHERE KursId = @MatheAP24a
      AND Wochentag = 1
      AND Startzeit = '08:00'
)
BEGIN
    INSERT INTO dbo.Stundenplan (KursId, Wochentag, Startzeit, Endzeit, Raum)
    VALUES (@MatheAP24a, 1, '08:00', '08:45', N'101');
END;

IF NOT EXISTS
(
    SELECT 1 FROM dbo.Stundenplan
    WHERE KursId = @ProgrammierenAP24a
      AND Wochentag = 1
      AND Startzeit = '09:00'
)
BEGIN
    INSERT INTO dbo.Stundenplan (KursId, Wochentag, Startzeit, Endzeit, Raum)
    VALUES (@ProgrammierenAP24a, 1, '09:00', '09:45', N'204');
END;

IF NOT EXISTS
(
    SELECT 1 FROM dbo.Stundenplan
    WHERE KursId = @MatheAP24b
      AND Wochentag = 2
      AND Startzeit = '08:00'
)
BEGIN
    INSERT INTO dbo.Stundenplan (KursId, Wochentag, Startzeit, Endzeit, Raum)
    VALUES (@MatheAP24b, 2, '08:00', '08:45', N'101');
END;
GO

/* =========================================================
   KONTROLLE
   ========================================================= */

SELECT * FROM dbo.Lehrer ORDER BY Name;
SELECT * FROM dbo.Klassen ORDER BY Bezeichnung;
SELECT * FROM dbo.Schueler ORDER BY Name;
SELECT * FROM dbo.Kurse ORDER BY Name;
SELECT * FROM dbo.Stundenplan ORDER BY Wochentag, Startzeit;
GO
