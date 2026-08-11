# Setup

Damit die Anwendung gestartet werden kann, müssen zuerst die lokalen Umgebungsvariablen eingerichtet werden.

## 1. `.env` Datei erstellen

Kopiere die vorhandene `.env.example` Datei und benenne die Kopie in `.env` um.

``` powershell
copy .env.example .env
```
Die .env Datei enthält lokale Zugangsdaten und Einstellungen und sollte deshalb nicht auf GitHub hochgeladen werden.

2. Benutzername und Passwort eintragen

Öffne die .env Datei und trage den gewünschten Admin-Benutzernamen und das Passwort ein.

Beispiel:
``` text
ADMIN_USERNAME=Admin
ADMIN_PASSWORD=hier_dein_passwort
```
Das Passwort wird von der Anwendung mit BCrypt verarbeitet und nicht direkt als Klartext für den Passwortvergleich verwendet.

3. Datenbank vorbereiten

Stelle sicher, dass der lokale SQL Server läuft und die benötigte Datenbank eingerichtet wurde. Importiere dafür das SQL aus der Datei [SchulApp](SchulApp/SQL/SchulAppDB.sql)

Die Anwendung prüft beim Start automatisch, ob eine Verbindung zur Datenbank hergestellt werden kann.

4. Anwendung starten

Nachdem die .env Datei eingerichtet und die Datenbank verfügbar ist, kann die Anwendung normal über Visual Studio gestartet werden.