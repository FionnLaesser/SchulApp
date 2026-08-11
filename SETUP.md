# Setup

Damit die SchulApp gestartet werden kann, müssen zuerst die lokale SQL-Datenbank und die benötigten Projektabhängigkeiten verfügbar sein.

Benutzerkonten werden direkt über die Anwendung registriert und in der SQL-Datenbank gespeichert. Die `.env` Datei ist deshalb nicht mehr für Admin-Benutzername und Admin-Passwort zuständig.

## 1. Projekt öffnen

Klone das Repository oder lade es von GitHub herunter und öffne das Projekt anschliessend in **Visual Studio 2022**.

Stelle sicher, dass die für das Projekt benötigte .NET-Version installiert ist.

## 2. NuGet-Pakete wiederherstellen

Visual Studio stellt die NuGet-Pakete normalerweise automatisch wieder her.

Unter anderem werden folgende Pakete beziehungsweise Bibliotheken verwendet:

```text
Microsoft.EntityFrameworkCore.SqlServer
Microsoft.EntityFrameworkCore.Tools
Microsoft.Data.SqlClient
DotNetEnv
BCrypt.Net
xunit
```

Falls Pakete fehlen, können sie über die NuGet-Paketverwaltung von Visual Studio wiederhergestellt werden.

## 3. Datenbank vorbereiten

Stelle sicher, dass der lokale **Microsoft SQL Server** läuft.

Richte anschliessend die benötigte Datenbank mit dem SQL-Skript ein:

[SchulAppDB.sql](SchulApp/SQL/SchulAppDB.sql)

Die Anwendung verwendet `SchulAppContext` mit Entity Framework Core für den Zugriff auf die SQL-Datenbank.

Die Datenbank enthält unter anderem Tabellen beziehungsweise Daten für:

- Einstellungen
- Klassen
- Lehrer
- Schüler
- Lehrerinformationen
- Kurse
- Stundenplan
- Benutzer

Beim Start prüft die Anwendung automatisch mit Entity Framework Core, ob eine Verbindung zur Datenbank hergestellt werden kann.

Kann keine Verbindung hergestellt werden, zeigt die Anwendung eine Fehlermeldung an und der normale Startvorgang wird nicht fortgesetzt.

## 4. ~~`.env` Datei optional einrichten~~

~~Die Anwendung kann beim Start eine lokale `.env` Datei einlesen.~~

~~Falls für deine lokale Umgebung Werte aus `.env.example` benötigt werden, kopiere die Datei und benenne die Kopie in `.env` um.~~

```powershell
copy .env.example .env
```

~~Die `.env` Datei kann lokale Einstellungen oder Umgebungswerte enthalten und sollte nicht auf GitHub hochgeladen werden.~~

~~Wichtig: Benutzername und Passwort für das Login werden nicht in der `.env` Datei gespeichert. Benutzerkonten werden über die Registrierungsseite angelegt und in der SQL-Datenbank gespeichert.~~
> **Veraltet:** Dieser Abschnitt wird nicht mehr benötigt.


## 5. Anwendung starten

Nachdem SQL Server läuft, die Datenbank eingerichtet ist und die benötigten Pakete verfügbar sind, kann die Anwendung normal über Visual Studio gestartet werden.

Beim Start passiert vereinfacht Folgendes:

1. Die Anwendung wird initialisiert.
2. Eine vorhandene `.env` Datei wird eingelesen.
3. Der LoadingScreen wird angezeigt.
4. Die Datenbankverbindung wird geprüft.
5. Bei erfolgreicher Verbindung wird das Login geöffnet.

## 6. Benutzer registrieren

Falls noch kein passender Benutzer vorhanden ist, öffne über das Login die Registrierungsseite.

Für neue Benutzer gelten unter anderem folgende Anforderungen:

- Benutzername mindestens 3 Zeichen
- Passwort mindestens 8 Zeichen
- Benutzername muss eindeutig sein

Das Passwort wird vor dem Speichern mit **BCrypt** gehasht. In der Datenbank wird nur der Passwort-Hash gespeichert.

## 7. Anmelden

Melde dich anschliessend mit dem registrierten Benutzernamen und dem zugehörigen Passwort an.

Beim Login wird das eingegebene Passwort mit BCrypt gegen den in der Datenbank gespeicherten Hash geprüft.

Erst nach erfolgreicher Anmeldung wird die eigentliche SchulApp geöffnet und die gespeicherten Design-Einstellungen werden geladen.

## Hinweise bei Problemen

### Datenbankverbindung schlägt fehl

Prüfe:

- Läuft Microsoft SQL Server?
- Wurde `SchulAppDB.sql` ausgeführt?
- Existiert die benötigte Datenbank?
- Passt die im `SchulAppContext` konfigurierte SQL-Server-Verbindung zur lokalen Installation?
- Hat der aktuell angemeldete Windows-Benutzer Zugriff auf die Datenbank?

### Login funktioniert nicht

Prüfe:

- Existiert der Benutzer bereits in der Benutzertabelle?
- Wurde der Benutzer über die Registrierungsseite erstellt?
- Ist der Benutzername korrekt geschrieben?
- Wird das richtige Passwort verwendet?

Das Klartext-Passwort kann nicht aus dem gespeicherten BCrypt-Hash zurückgelesen werden.
