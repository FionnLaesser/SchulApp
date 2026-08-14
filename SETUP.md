# Setup

Diese Anleitung beschreibt die Einrichtung der aktuellen SchulApp inklusive **WinForms-Anwendung**, **SQL Server**, **Entity Framework Core**, **schulAppREST**, **PingPong** und **Bestenliste**.

## 1. Voraussetzungen

Benötigt werden:

- Windows
- Visual Studio 2026
- .NET 10 SDK
- Microsoft SQL Server 2022
- Zugriff auf die lokale `SchulAppDB`

Prüfe die installierte .NET-Version:

```powershell
dotnet --version
```

Das Projekt verwendet:

```text
net10.0-windows
```

## 2. Projektstruktur

Die wichtigsten Projekte liegen getrennt:

```text
Schulapp
│
├── SchulApp
│   └── SchulApp.csproj
│
└── schulAppREST
    └── schulAppREST.csproj
```

`SchulApp` ist die Windows-Forms-Anwendung.

`schulAppREST` ist die ASP.NET Core REST API.

Für PingPong und die Bestenliste müssen **beide** Anwendungen laufen.

## 3. NuGet-Pakete wiederherstellen

Visual Studio stellt die NuGet-Pakete normalerweise automatisch wieder her.

Alternativ kann das WinForms-Projekt explizit wiederhergestellt werden:

```powershell
cd .\SchulApp
dotnet restore .\SchulApp.csproj
```

Danach kann es gebaut werden:

```powershell
dotnet build .\SchulApp.csproj
```

Falls im Ordner sowohl `SchulApp.csproj` als auch `SchulApp.slnx` liegen, sollte das gewünschte Build-Ziel explizit angegeben werden. Ein einfaches `dotnet build` kann sonst zu `MSB1011` führen.

Die REST API kann separat gebaut werden:

```powershell
cd ..\schulAppREST
dotnet build .\schulAppREST.csproj
```

## 4. SQL Server vorbereiten

Stelle sicher, dass der lokale Microsoft SQL Server läuft.

Die Anwendung verwendet die Datenbank:

```text
SchulAppDB
```

Führe zuerst das bestehende Hauptskript aus:

```text
SchulApp/SQL/SchulAppDB.sql
```

Dadurch werden die grundlegenden Tabellen der SchulApp eingerichtet.

## 5. REST API starten

PingPong und die Bestenliste speichern beziehungsweise laden ihre Daten über die vorhandene REST API.

Öffne ein zweites Terminal:

```powershell
cd .\schulAppREST
dotnet run
```

Mit dem aktuellen Development-Profil hört die API standardmässig auf:

```text
https://localhost:63635
http://localhost:63636
```

Lass das API-Terminal während der Verwendung von PingPong und der Bestenliste geöffnet.

## 6. REST API testen

Wenn die API läuft, kann die Bestenliste im Browser oder mit Postman getestet werden:

```text
https://localhost:63635/api/PingPong/bestenliste
```

Bei einer funktionierenden Verbindung wird JSON zurückgegeben.

Beispiel:

```json
[
  {
    "benutzerId": 1,
    "benutzername": "Admin",
    "siege": 0,
    "punkte": 0,
    "toreErzielt": 0,
    "toreKassiert": 0,
    "torverhaeltnis": 0
  }
]
```

## 7. Optionale API-Adresse konfigurieren

Die WinForms-Anwendung verwendet standardmässig:

```text
https://localhost:63635/
```

Falls die REST API auf einer anderen Adresse läuft, kann die Umgebungsvariable `SCHULAPP_API_BASE_URL` gesetzt werden.

Beispiel für PowerShell:

```powershell
$env:SCHULAPP_API_BASE_URL="https://localhost:7001/"
dotnet run --project .\SchulApp.csproj
```

Die URL sollte auf die laufende `schulAppREST` API zeigen.

## 8. WinForms-Anwendung starten

Nachdem SQL Server und die REST API laufen, kann die SchulApp gestartet werden.

Im Ordner `SchulApp`:

```powershell
dotnet run --project .\SchulApp.csproj
```

Oder normal über Visual Studio.

Beim Start passiert vereinfacht:

1. Die Anwendung wird initialisiert.
2. Eine vorhandene `.env` Datei kann eingelesen werden.
3. Der LoadingScreen wird angezeigt.
4. Die SQL-Datenbankverbindung wird geprüft.
5. Das Login wird geöffnet.
6. Nach erfolgreichem Login wird das Theme des Benutzers geladen.
7. Das Hauptmenü wird geöffnet.

## 9. Benutzer registrieren

Falls noch kein Benutzer vorhanden ist, öffne über das Login die Registrierungsseite.

Für neue Benutzer gelten unter anderem:

- Benutzername mindestens 3 Zeichen
- Passwort mindestens 8 Zeichen
- Benutzername muss eindeutig sein
- eine Rolle muss ausgewählt werden

Passwörter werden mit BCrypt gehasht gespeichert.

Für ein PingPong-Spiel werden mindestens **zwei unterschiedliche Benutzerkonten** benötigt.

## 10. PingPong testen

1. Mit Benutzer 1 anmelden.
2. Im Hauptmenü PingPong öffnen.
3. Warten, bis die Benutzer über die REST API geladen wurden.
4. Benutzer 2 als Gegner auswählen.
5. Auf **Spiel starten** klicken.
6. Das Spiel bis 5 Punkte spielen.

Steuerung:

```text
Links:  W / S
Rechts: Pfeiltasten hoch / runter
```

Nach dem fünften Punkt:

- wird das Spiel gestoppt
- der Gewinner wird angezeigt
- das Endergebnis wird angezeigt
- das Ergebnis wird über `POST /api/PingPong/spiel` gespeichert
- der Gewinner erhält 1 Sieg und 3 Bestenlistenpunkte
- Tore werden für beide Benutzer aktualisiert

## 11. Bestenliste testen

Öffne im Hauptmenü die Bestenliste.

Die Form lädt automatisch:

```http
GET /api/PingPong/bestenliste
```

Angezeigt werden:

- Benutzername
- Siege
- Punkte
- erzielte Tore
- kassierte Tore
- Torverhältnis

Falls die API nicht läuft, erscheint eine Meldung, dass die REST API nicht erreichbar ist.

## 12. `.env` Datei

Die `.env` Datei wird nicht für Login-Benutzername oder Login-Passwort benötigt.

Benutzerkonten werden direkt in der SQL-Datenbank gespeichert.

Eine vorhandene `.env` Datei kann weiterhin für lokale Konfigurationen verwendet werden.

## 13. Häufige Probleme

### Visual Studio zeigt 0 Projekte

Wenn die Solution geöffnet ist, aber im Projektmappen-Explorer `0 Projekte` angezeigt werden, prüfe:

```powershell
dotnet sln .\SchulApp.slnx list
```

Falls `SchulApp.csproj` fehlt:

```powershell
dotnet sln .\SchulApp.slnx add .\SchulApp.csproj
```

Danach die Solution erneut öffnen.

### MSB1011 bei `dotnet build`

Wenn im gleichen Ordner sowohl `.csproj` als auch `.slnx` liegen, verwende:

```powershell
dotnet build .\SchulApp.csproj
```

oder:

```powershell
dotnet build .\SchulApp.slnx
```

### Datenbankverbindung schlägt fehl

Prüfe:

- Läuft Microsoft SQL Server?
- Existiert `SchulAppDB`?
- Wurde `SchulAppDB.sql` ausgeführt?
- Passt die Connection String Konfiguration?
- Hat der Windows-Benutzer Zugriff auf die Datenbank?

### PingPong oder Bestenliste meldet REST API nicht erreichbar

Prüfe zuerst:

```powershell
cd .\schulAppREST
dotnet run
```

Danach im Browser:

```text
https://localhost:63635/api/PingPong/bestenliste
```

Falls keine Verbindung aufgebaut werden kann, läuft die API nicht auf der erwarteten Adresse.

### REST API liefert HTTP 500

Prüfe, ob dieses SQL-Skript bereits ausgeführt wurde:

```text
SchulApp/SQL/SchulAppDB.sql
```

Fehlende PingPong-Spalten oder eine fehlende `PingPongSpiel` Tabelle können API-Fehler verursachen.

### HTTPS-Zertifikat verursacht Probleme

Für lokale Entwicklung kann das .NET-Entwicklungszertifikat geprüft beziehungsweise vertraut werden:

```powershell
dotnet dev-certs https --trust
```

Danach API und WinForms-Anwendung neu starten.

### Login funktioniert nicht

Prüfe:

- Existiert der Benutzer in `dbo.Benutzer`?
- Wurde der Benutzer über die Registrierungsseite erstellt?
- Ist der Benutzername korrekt?
- Wird das richtige Passwort verwendet?

Das Klartext-Passwort kann nicht aus dem gespeicherten BCrypt-Hash zurückgelesen werden.

## 15. Empfohlene Startreihenfolge

Für die lokale Entwicklung ist diese Reihenfolge am einfachsten:

```text
1. SQL Server starten
2. schulAppREST starten
3. SchulApp starten
4. Anmelden
5. PingPong oder Bestenliste verwenden
```

Wenn beide Projekte später gemeinsam in einer Visual-Studio-Solution enthalten sind, können sie auch als mehrere Startprojekte konfiguriert werden.
