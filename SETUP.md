# SchulApp Setup

Diese Anleitung beschreibt die lokale Einrichtung der aktuellen SchulApp mit WinForms, SQL Server, REST API, SOAP API, Tests sowie Prometheus und Grafana.

Für einen schnellen Überblick über das Projekt siehe [`README.md`](README.md).

## Voraussetzungen

Benötigt werden:

- Windows
- .NET 10 SDK
- Microsoft SQL Server 2022
- Docker Desktop
- Docker Compose
- PowerShell
- optional Visual Studio mit .NET-10-Unterstützung

Version prüfen:

```powershell
dotnet --version
docker --version
docker compose version
```

Die WinForms-Anwendung verwendet `net10.0-windows`. REST und SOAP verwenden `net10.0`.

## Repository-Struktur

```text
SchulApp/
├── .github/workflows/
│   └── build-and-test.yml
├── Monitoring/
│   ├── docker-compose.yml
│   ├── prometheus/
│   ├── grafana/
│   └── README.md
├── Postman/
├── Screenshots/
├── SchulApp/
│   ├── Data/
│   ├── Models/
│   ├── Services/
│   ├── SQL/
│   └── SchulApp.csproj
├── schulAppREST/
│   └── schulAppREST.csproj
├── SchulappSOAP/
│   └── SchulappSOAP.csproj
├── SchulApp.Tests/
│   └── SchulApp.Tests.csproj
├── SchulApp.slnx
├── README.md
└── start.ps1
```

## Repository klonen

```powershell
git clone https://github.com/FionnLaesser/SchulApp.git
cd SchulApp
```

## NuGet-Pakete wiederherstellen

Die gesamte Solution kann mit einem Befehl wiederhergestellt werden:

```powershell
dotnet restore .\SchulApp.slnx
```

Alternativ einzeln:

```powershell
dotnet restore .\SchulApp\SchulApp.csproj
dotnet restore .\schulAppREST\schulAppREST.csproj
dotnet restore .\SchulappSOAP\SchulappSOAP.csproj
dotnet restore .\SchulApp.Tests\SchulApp.Tests.csproj
```

## SQL Server und Datenbank

Die Anwendung verwendet standardmässig eine lokale SQL-Server-Instanz und die Datenbank:

```text
SchulAppDB
```

Für eine neue lokale Installation das zentrale Skript ausführen:

```text
SchulApp/SQL/SchulAppDB.sql
```

Das Skript enthält die Datenbankstruktur für die Schulverwaltung und die zusätzlichen Funktionen wie Benutzer, Profil, Einstellungen, PingPong, Bestenliste und Audit Log.

Für eine vollständige lokale Zurücksetzung existiert zusätzlich:

```text
SchulApp/SQL/DropDatabaseForErrors.sql
```

Dieses Skript nur verwenden, wenn die lokale Datenbank bewusst gelöscht und danach neu erstellt werden soll.

## Anwendung starten

Vor dem Start sicherstellen, dass:

- SQL Server läuft
- Docker Desktop läuft
- Docker Compose verfügbar ist
- die Datenbank `SchulAppDB` eingerichtet ist

Danach im Repository-Root ausführen:

```powershell
.\start.ps1
```

Das Skript übernimmt den Startablauf automatisch:

1. Docker und Docker Compose prüfen
2. Prometheus und Grafana starten
3. auf Prometheus und Grafana warten
4. REST API starten
5. SOAP API starten
6. auf REST API und SOAP-WSDL warten
7. REST- und SOAP-`/metrics` prüfen
8. WinForms-Anwendung starten
9. beim Beenden REST, SOAP und Monitoring wieder stoppen

Die Docker-Volumes bleiben beim normalen Stoppen bestehen. Dadurch bleiben Prometheus- und Grafana-Daten erhalten.

## Lokale Endpunkte

| Dienst | URL |
|---|---|
| REST Swagger | `https://localhost:63635/swagger` |
| REST HTTPS | `https://localhost:63635` |
| REST HTTP | `http://localhost:63636` |
| REST Health Check | `https://localhost:63635/health` |
| REST Metrics | `http://localhost:63636/metrics` |
| SOAP Service | `http://localhost:5210/SchuelerService.svc` |
| SOAP WSDL | `http://localhost:5210/SchuelerService.svc?wsdl` |
| SOAP Metrics | `http://localhost:5210/metrics` |
| Prometheus | `http://localhost:9090` |
| Grafana | `http://localhost:3000` |

Grafana verwendet lokal:

```text
Benutzer: admin
Passwort: schulapp
```

## Benutzer registrieren

Neue Benutzer werden direkt in der WinForms-Anwendung registriert.

Aktuelle Grundregeln:

- Benutzername mindestens 3 Zeichen
- Passwort mindestens 8 Zeichen
- Benutzername eindeutig
- Rolle muss ausgewählt werden

Rollen:

```text
Admin
Lehrer
Schüler
```

Passwörter werden mit BCrypt gehasht gespeichert.

## Rollen im aktuellen Stand

### Admin

- vollständige Schulverwaltung
- Audit Log
- Schüler-CSV-Import
- CSV/PDF-Export
- CSV-Importvorlage herunterladen

### Lehrer

- Schüler verwalten
- Stundenpläne bearbeiten

### Schüler

- Stundenplan ansehen

Profil, Einstellungen, PingPong und Bestenliste sind zusätzlich im normalen Benutzerbereich verfügbar.

## Datenimport und Export testen

Der Datenübertragungsbereich ist aktuell im Admin-Hauptmenü verfügbar.

### CSV-Importvorlage

Über die Import-/Export-Funktion kann eine Vorlage gespeichert werden. Sie enthält:

```csv
Name;KlasseId
Müller Anna;1
```

Die `KlasseId` muss einer vorhandenen Klasse entsprechen.

### Schüler importieren

Beim CSV-Import werden die Daten vor dem Speichern validiert. Unter anderem werden geprüft:

- `Name`
- `KlasseId`
- gültige numerische Werte
- existierende Klassen
- doppelte Schüler innerhalb derselben Klasse

Wenn Validierungsfehler vorhanden sind, wird der Import abgebrochen und es werden keine Teilmengen gespeichert.

### Daten exportieren

Aktuell exportierbar:

- Schüler
- Klassen
- Stundenplan
- Audit Log

Formate:

```text
CSV
PDF
```

## REST API testen

Swagger öffnen:

```text
https://localhost:63635/swagger
```

Die REST API besitzt Controller für:

- Schüler
- Lehrer
- Klassen
- Kurse
- Stundenplan
- Einstellungen
- PingPong und Bestenliste
- Health Check

Eine Postman-Collection liegt unter:

```text
Postman/SchulAppRESTAPI.postman_collection
```

### Health Check

```text
https://localhost:63635/health
```

Der Health Check prüft den aktuellen Status der REST API, SQL-Datenbank und SOAP API.

Erwartete Statuscodes:

- HTTP `200` bei gesundem Zustand
- HTTP `503`, wenn eine benötigte Abhängigkeit nicht erreichbar ist

### Global Exception Handling

Unerwartete REST-Fehler werden durch `GlobalExceptionMiddleware` behandelt. Dadurch erhält der Client strukturierte JSON-Fehlerantworten anstelle ungefilterter interner Exceptions.

## SOAP API testen

WSDL öffnen:

```text
http://localhost:5210/SchuelerService.svc?wsdl
```

Der aktuelle CoreWCF-Vertrag bietet:

```text
GetSchueler
GetSchuelerById
AddSchueler
UpdateSchueler
DeleteSchueler
```

Die WinForms-Anwendung verwendet eine generierte WCF Web Service Reference.

Wenn der SOAP-Vertrag geändert wurde:

1. `SchulappSOAP` starten
2. WSDL im Browser prüfen
3. Service Reference in Visual Studio aktualisieren
4. Solution neu bauen

## Monitoring testen

Das Monitoring wird durch `start.ps1` automatisch gestartet.

Prometheus:

```text
http://localhost:9090
```

Grafana:

```text
http://localhost:3000
```

REST Metrics:

```text
http://localhost:63636/metrics
```

SOAP Metrics:

```text
http://localhost:5210/metrics
```

Weitere Informationen stehen in [`Monitoring/README.md`](Monitoring/README.md).

## Tests ausführen

Das Testprojekt enthält aktuell 32 xUnit-Tests.

Alle Tests:

```powershell
dotnet test .\SchulApp.Tests\SchulApp.Tests.csproj
```

Release-Build und Tests:

```powershell
dotnet build .\SchulApp.slnx --configuration Release
dotnet test .\SchulApp.Tests\SchulApp.Tests.csproj --configuration Release --no-build
```

Weitere Details stehen in [`SchulApp.Tests/README.md`](SchulApp.Tests/README.md).

## Code Coverage

GitHub Actions führt die Tests mit XPlat Code Coverage aus:

```powershell
dotnet test SchulApp.Tests/SchulApp.Tests.csproj --configuration Release --no-build --collect:"XPlat Code Coverage" --results-directory TestResults
```

Danach wird mit ReportGenerator ein HTML-Report erzeugt und als GitHub-Actions-Artifact `code-coverage-report` gespeichert.

Der aktuelle Prozentwert wird im Summary des jeweiligen CI-Laufs angezeigt und wird deshalb nicht statisch in der Dokumentation festgeschrieben.

## GitHub Actions

Workflow:

```text
.github/workflows/build-and-test.yml
```

Auslöser:

```text
push
pull_request
```

Pipeline:

```text
Restore
Build Release
Tests
Coverage
ReportGenerator
Coverage Artifact
```

Runner:

```text
windows-latest
```

## Konfiguration und `.env`

Im Repository liegt `.env.example` als mögliche lokale Referenz.

Wichtig: Das automatische Laden einer `.env`-Datei ist in `SchulApp/Program.cs` aktuell auskommentiert. Eine lokale `.env`-Datei wird deshalb nicht automatisch eingelesen, solange diese Funktion nicht wieder aktiviert wird.

Umgebungsvariablen können weiterhin direkt über Windows oder PowerShell gesetzt werden. Die Anwendung kann beispielsweise `SCHULAPP_API_BASE_URL` für eine abweichende REST-Basisadresse verwenden.

## Häufige Probleme

### Docker Desktop läuft nicht

Prüfen:

```powershell
docker info
docker compose version
```

### Grafana oder Prometheus nicht erreichbar

```text
http://localhost:3000/api/health
http://localhost:9090/-/ready
```

Container prüfen:

```powershell
docker compose -f .\Monitoring\docker-compose.yml ps
```

### Health Check ist Unhealthy

Prüfen:

- läuft SQL Server?
- existiert `SchulAppDB`?
- läuft die SOAP API?
- ist die SOAP-WSDL erreichbar?
- stimmt `SoapApi:HealthUrl` in `schulAppREST/appsettings.json`?

### Datenbankverbindung schlägt fehl

Prüfen:

- SQL Server läuft
- `SchulAppDB` existiert
- `SchulApp/SQL/SchulAppDB.sql` wurde ausgeführt
- Windows-Benutzer besitzt Zugriff auf SQL Server
- lokale Connection Strings passen zur Installation

### SOAP API ist nicht erreichbar

```text
http://localhost:5210/SchuelerService.svc?wsdl
```

Wenn die WSDL nach Vertragsänderungen korrekt ist, aber WinForms alte Methoden kennt, die WCF Web Service Reference aktualisieren.

### REST API ist nicht erreichbar

```text
https://localhost:63635/swagger
http://localhost:63636/api/PingPong/bestenliste
```

### HTTPS-Entwicklungszertifikat

Falls das lokale .NET-Zertifikat Probleme verursacht:

```powershell
dotnet dev-certs https --clean
dotnet dev-certs https --trust
```

Danach REST API und Anwendung neu starten.

### Ports bereits belegt

Standardports:

```text
3000   Grafana
5210   SOAP
9090   Prometheus
63635  REST HTTPS
63636  REST HTTP
```

Unter Windows können belegte Ports beispielsweise mit folgendem Befehl geprüft werden:

```powershell
netstat -ano | findstr :5210
```

## Monitoring manuell stoppen oder löschen

Nur stoppen:

```powershell
docker compose -f .\Monitoring\docker-compose.yml down
```

Monitoring-Daten und Volumes vollständig löschen:

```powershell
docker compose -f .\Monitoring\docker-compose.yml down -v
```

Der zweite Befehl löscht die gespeicherte Monitoring-Historie.
