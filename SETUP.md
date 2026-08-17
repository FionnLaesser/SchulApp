# Setup

Diese Anleitung beschreibt die lokale Einrichtung und den Start der aktuellen **SchulApp** mit **WinForms**, **SQL Server**, **schulAppREST**, **SchulappSOAP**, **PingPong**, **Bestenliste**, **Health Check**, **Unit-Tests** sowie **Prometheus- und Grafana-Monitoring**.

## 1. Voraussetzungen

Benötigt werden:

- Windows
- Visual Studio 2026 (Optional)
- .NET 10 SDK
- Microsoft SQL Server 2022
- Docker Desktop mit Docker Compose
- PowerShell

Prüfe die installierte .NET-Version:

```powershell
dotnet --version
```

Die WinForms-Anwendung verwendet:

```text
net10.0-windows
```

## 2. Projektstruktur

Die wichtigsten Dateien und Projekte liegen so:

```text
SchulApp
│
├── .github
│   └── workflows
│       └── build-and-test.yml
│
├── Monitoring
│   ├── README.md
│   ├── docker-compose.yml
│   ├── prometheus
│   └── grafana
│
├── SchulApp.Tests
│   ├── README.md
│   └── SchulApp.Tests.csproj
│
├── SchulApp
│   ├── SchulApp.csproj
│   └── SQL
│       └── SchulAppDB.sql
│
├── schulAppREST
│   └── schulAppREST.csproj
│
├── SchulappSOAP
│   └── SchulappSOAP.csproj
│
├── SchulApp.slnx
└── start.ps1
```

`SchulApp` ist die Windows-Forms-Anwendung.

`schulAppREST` ist die ASP.NET Core REST API mit Swagger, Health Check und Prometheus-Metriken.

`SchulappSOAP` ist die CoreWCF SOAP API mit Prometheus-Metriken.

`SchulApp.Tests` enthält die xUnit-Tests für Service-Schicht, REST API und SOAP API.

`Monitoring` enthält Docker Compose, Prometheus-Konfiguration und das automatisch provisionierte Grafana-Dashboard.

`start.ps1` startet Monitoring, beide APIs und danach die WinForms-Anwendung in der richtigen Reihenfolge.

## 3. NuGet-Pakete

Visual Studio beziehungsweise `dotnet` stellt die benötigten NuGet-Pakete normalerweise automatisch wieder her.

Am einfachsten wird die komplette Solution wiederhergestellt:

```powershell
dotnet restore .\SchulApp.slnx
```

Falls einzelne Projekte separat wiederhergestellt werden sollen:

```powershell
dotnet restore .\SchulApp\SchulApp.csproj
dotnet restore .\schulAppREST\schulAppREST.csproj
dotnet restore .\SchulappSOAP\SchulappSOAP.csproj
dotnet restore .\SchulApp.Tests\SchulApp.Tests.csproj
```

Zu den zusätzlichen aktuellen Paketen gehören unter anderem `prometheus-net.AspNetCore`, `Microsoft.EntityFrameworkCore.InMemory`, `xunit` und `Swashbuckle.AspNetCore`.

## 4. SQL Server und Datenbank vorbereiten

Stelle sicher, dass der lokale **Microsoft SQL Server 2022** läuft.

Die Anwendung verwendet die Datenbank:

```text
SchulAppDB
```

Beim ersten Einrichten muss nur **ein einziges SQL-Skript** ausgeführt werden:

```text
SchulApp/SQL/SchulAppDB.sql
```

Dieses Skript richtet die für die SchulApp benötigte Datenbankstruktur ein, inklusive der Datenbankobjekte für PingPong und die Bestenliste.

Es müssen keine zusätzlichen SQL-Skripte separat ausgeführt werden.

## 5. Anwendung starten

Öffne PowerShell im Hauptordner `SchulApp`.

Beispiel:

```text
C:\Users\<Benutzer>\Desktop\Cs\SchulApp
```

Stelle vor dem Start sicher, dass **SQL Server** und **Docker Desktop** laufen.

Starte danach:

```powershell
.\start.ps1
```

Das Skript übernimmt den aktuellen Startablauf:

1. Docker, Docker Compose und Docker Desktop werden geprüft.
2. Prometheus und Grafana werden mit `Monitoring/docker-compose.yml` gestartet.
3. Das Skript wartet auf `http://localhost:9090/-/ready`.
4. Das Skript wartet auf `http://localhost:3000/api/health`.
5. `schulAppREST` wird gestartet.
6. `SchulappSOAP` wird gestartet.
7. Das Skript wartet auf die REST API.
8. Das Skript wartet auf die SOAP-WSDL.
9. Der REST-`/metrics`-Endpunkt wird geprüft.
10. Der SOAP-`/metrics`-Endpunkt wird geprüft.
11. Danach wird die WinForms-Anwendung `SchulApp` gestartet.
12. Der LoadingScreen prüft die SQL-Datenbankverbindung.
13. Anschliessend wird das Login geöffnet.

Beim Beenden der WinForms-Anwendung werden REST API, SOAP API, Prometheus und Grafana beendet. Die Docker-Volumes werden nicht gelöscht, damit die Monitoring-Historie erhalten bleibt.

Für den normalen Start müssen die drei .NET-Projekte nicht separat mit `dotnet run` gestartet werden.

## 6. Benutzer registrieren

Falls noch kein Benutzer vorhanden ist, kann über die Login-Seite ein Benutzer registriert werden.

Für neue Benutzer gelten unter anderem:

- Benutzername mindestens 3 Zeichen
- Passwort mindestens 8 Zeichen
- Benutzername muss eindeutig sein
- eine Rolle muss ausgewählt werden

Passwörter werden mit BCrypt gehasht gespeichert.

Für ein PingPong-Spiel müssen mindestens **zwei unterschiedliche Benutzerkonten** vorhanden sein.

## 7. REST API testen

Die REST API wird durch `start.ps1` automatisch gestartet.

Lokale Adressen:

```text
Swagger:      https://localhost:63635/swagger
HTTPS API:    https://localhost:63635
HTTP API:     http://localhost:63636
Health Check: https://localhost:63635/health
Metriken:     http://localhost:63636/metrics
```

Beim direkten Start des REST-Projekts wird im Development-Modus automatisch Swagger geöffnet.

Die Bestenliste kann testweise über folgenden Endpoint aufgerufen werden:

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

### Health Check

Der Health Check prüft REST API, SQL-Datenbank und SOAP API:

```text
https://localhost:63635/health
```

Wenn alle Komponenten erreichbar sind, wird HTTP `200` mit `Healthy` zurückgegeben. Bei einem Fehler in Datenbank oder SOAP API wird HTTP `503` mit `Unhealthy` zurückgegeben.

Die SOAP-WSDL-Adresse wird in `schulAppREST/appsettings.json` unter `SoapApi:HealthUrl` konfiguriert.

Die WinForms-Anwendung verwendet standardmässig:

```text
https://localhost:63635/
```

Falls die API bewusst auf einer anderen Adresse gestartet wird, kann optional `SCHULAPP_API_BASE_URL` gesetzt werden.

## 8. SOAP API testen

Die SOAP API wird durch `start.ps1` automatisch gestartet.

Der aktuelle lokale SOAP-Endpunkt lautet:

```text
http://localhost:5210/SchuelerService.svc
```

Die SOAP API stellt zusätzlich Prometheus-Metriken bereit:

```text
http://localhost:5210/metrics
```

Die WSDL kann direkt im Browser getestet werden:

```text
http://localhost:5210/SchuelerService.svc?wsdl
```

Wenn die SOAP API korrekt läuft, wird dort ein XML-Dokument mit der Servicebeschreibung angezeigt.

Der Schüler-Service bietet aktuell folgende Operationen:

```text
GetSchueler
GetSchuelerById
DeleteSchueler
```

Die WinForms-Anwendung verwendet dafür eine generierte **WCF Web Service Reference**.

Aktuell werden Schüler über SOAP geladen und gelöscht. Erstellen und Bearbeiten verwenden weiterhin die vorhandene lokale Service- und Entity-Framework-Logik.

Falls der SOAP-Vertrag oder das SOAP-Model geändert wird:

1. `SchulappSOAP` starten.
2. Prüfen, ob die WSDL erreichbar ist.
3. In Visual Studio unter `SchulApp` die vorhandene WCF Web Service Reference öffnen.
4. Die Service Reference aktualisieren.
5. Die Projektmappe neu erstellen.

## 9. PingPong testen

1. Optional unter **Einstellungen** eine Ballgeschwindigkeit zwischen `1` und `20` wählen. Standard ist `6`.
2. Mit Benutzer 1 anmelden.
3. Im Hauptmenü PingPong öffnen.
4. Warten, bis die Benutzer über die REST API geladen wurden.
5. Benutzer 2 als Gegner auswählen.
6. Auf **Spiel starten** klicken.
7. Das Spiel bis 5 Punkte spielen.

Steuerung:

```text
Links:  W / S
Rechts: Pfeiltasten hoch / runter
```

Nach dem fünften Punkt:

- das Spiel wird gestoppt
- der Gewinner wird angezeigt
- das Endergebnis wird angezeigt
- das Ergebnis wird über `POST /api/PingPong/spiel` gespeichert
- der Gewinner erhält 1 Sieg und 3 Bestenlistenpunkte
- erzielte und kassierte Tore werden aktualisiert

## 10. Bestenliste testen

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

## 11. Monitoring prüfen

`start.ps1` startet Prometheus und Grafana automatisch.

Lokale URLs:

```text
Grafana:         http://localhost:3000
Prometheus:      http://localhost:9090
REST Metriken:   http://localhost:63636/metrics
SOAP Metriken:   http://localhost:5210/metrics
```

Grafana Login:

```text
Benutzer: admin
Passwort: schulapp
```

Das Dashboard im Grafana-Ordner `SchulApp` wird automatisch provisioniert.

Prometheus fragt die REST- und SOAP-Metriken alle 5 Sekunden ab. Die Daten werden bis zu 30 Tage im Docker-Volume gespeichert.

Weitere Details stehen in `Monitoring/README.md`.

## 12. Unit-Tests ausführen

Das Testprojekt `SchulApp.Tests` enthält aktuell 16 xUnit-Tests für:

- lokale Schüler-Service-Schicht
- REST-Schüler-Controller
- SOAP-Schüler-Service

Die REST- und SOAP-Tests verwenden eine Entity-Framework-InMemory-Datenbank und verändern `SchulAppDB` nicht.

Alle Tests:

```powershell
dotnet test
```

Nur das Testprojekt:

```powershell
dotnet test .\SchulApp.Tests\SchulApp.Tests.csproj
```

## 13. GitHub Actions

Die Datei `.github/workflows/build-and-test.yml` führt bei jedem Push und Pull Request automatisch aus:

```text
Restore
Build Release
Tests
```

Der Workflow läuft auf `windows-latest` mit .NET 10.

## 14. `.env` Datei

Die `.env` Datei wird nicht für Benutzername oder Passwort des Logins benötigt.

Benutzerkonten werden direkt in der SQL-Datenbank gespeichert.

Eine vorhandene `.env` Datei kann weiterhin für optionale lokale Konfigurationen verwendet werden.

## 15. Häufige Probleme

### Docker oder Monitoring startet nicht

Prüfe:

- Ist Docker Desktop installiert?
- Läuft Docker Desktop?
- Funktioniert `docker compose version`?
- Ist Port `3000` frei?
- Ist Port `9090` frei?

Manueller Test:

```powershell
docker info
docker compose -f .\Monitoring\docker-compose.yml up -d
```

### Grafana oder Prometheus ist nicht erreichbar

Teste:

```text
http://localhost:3000/api/health
http://localhost:9090/-/ready
```

Wenn die Container nicht laufen, prüfe:

```powershell
docker compose -f .\Monitoring\docker-compose.yml ps
```

### Health Check ist `Unhealthy`

Teste zuerst:

```text
https://localhost:63635/health
```

Prüfe danach insbesondere:

- läuft SQL Server?
- ist `SchulAppDB` erreichbar?
- läuft die SOAP API?
- ist `http://localhost:5210/SchuelerService.svc?wsdl` erreichbar?
- stimmt `SoapApi:HealthUrl` in `schulAppREST/appsettings.json`?

### Datenbankverbindung schlägt fehl

Prüfe:

- Läuft Microsoft SQL Server?
- Existiert `SchulAppDB`?
- Wurde `SchulApp/SQL/SchulAppDB.sql` ausgeführt?
- Passt die Connection-String-Konfiguration?
- Hat der Windows-Benutzer Zugriff auf die Datenbank?

### PingPong oder Bestenliste meldet REST API nicht erreichbar

Starte die Anwendung aus dem Hauptordner erneut mit:

```powershell
./start.ps1
```

Teste danach:

```text
https://localhost:63635/api/PingPong/bestenliste
```

Wenn dort keine Antwort kommt, prüfe das PowerShell-Fenster der REST API auf Fehlermeldungen.


### SOAP API oder WSDL nicht erreichbar

Starte die Anwendung erneut aus dem Hauptordner:

```powershell
./start.ps1
```

Teste danach:

```text
http://localhost:5210/SchuelerService.svc?wsdl
```

Wenn keine WSDL angezeigt wird, prüfe das PowerShell-Fenster der SOAP API auf Fehlermeldungen.

Falls die WinForms-Anwendung nach Änderungen am SOAP-Vertrag noch alte Datentypen verwendet, aktualisiere die WCF Web Service Reference in Visual Studio.

### SOAP meldet einen internen Serverfehler

Bei einem SOAP-Fehler liegt die eigentliche Exception häufig im Serverprojekt.

Prüfe insbesondere:

- läuft SQL Server?
- ist `SchulAppDB` erreichbar?
- stimmen die Properties des `SchuelerModel` mit der Datenbank überein?
- ist `SchuelerId` im SOAP-Model als Primärschlüssel definiert?

Beispiel:

```csharp
[Key]
public int SchuelerId { get; set; }
```

### REST API liefert HTTP 500

Prüfe zuerst, ob das einzige benötigte SQL-Skript vollständig ausgeführt wurde:

```text
SchulApp/SQL/SchulAppDB.sql
```

Fehlende Tabellen oder Spalten können API-Fehler verursachen.

### HTTPS-Zertifikat verursacht Probleme

Für die lokale Entwicklung kann das .NET-Entwicklungszertifikat vertraut werden:

```powershell
dotnet dev-certs https --trust
```

Danach `./start.ps1` erneut ausführen.

### Visual Studio zeigt 0 Projekte

Prüfe im Ordner `SchulApp`:

```powershell
dotnet sln .\SchulApp.slnx list
```

Falls `SchulApp.csproj` fehlt:

```powershell
dotnet sln .\SchulApp.slnx add .\SchulApp.csproj
```

### MSB1011 bei `dotnet build`

Wenn im gleichen Ordner sowohl `.csproj` als auch `.slnx` liegen, gib das Ziel explizit an:

```powershell
dotnet build .\SchulApp.csproj
```

oder:

```powershell
dotnet build .\SchulApp.slnx
```

## 16. Kurzstart

Nach der einmaligen Einrichtung reicht normalerweise:

```text
1. SQL Server starten
2. Docker Desktop starten
3. PowerShell im Hauptordner SchulApp öffnen
4. .\start.ps1 ausführen
5. Warten, bis Prometheus und Grafana bereit sind
6. Warten, bis REST API und SOAP API bereit sind
7. Anmelden
```

Beim allerersten Einrichten muss davor einmal ausgeführt werden:

```text
SchulApp/SQL/SchulAppDB.sql
```

Optional prüfen:

```text
Swagger:      https://localhost:63635/swagger
Health:       https://localhost:63635/health
Grafana:      http://localhost:3000
Prometheus:   http://localhost:9090
```
