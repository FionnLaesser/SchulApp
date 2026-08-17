# Setup

Diese Anleitung beschreibt die lokale Einrichtung und den Start der aktuellen **SchulApp** mit **WinForms**, **SQL Server**, **schulAppREST**, **SchulappSOAP**, **PingPong** und **Bestenliste**.

## 1. Voraussetzungen

Benötigt werden:

- Windows
- Visual Studio 2026 (Optional)
- .NET 10 SDK
- Microsoft SQL Server 2022
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
Schulapp
│
├── start.ps1
│
├── SchulApp
│   ├── SchulApp.csproj
│   └── SQL
│       └── SchulAppDB.sql
│
├── schulAppREST
│   └── schulAppREST.csproj
│
└── SchulappSOAP
    └── SchulappSOAP.csproj
```

`SchulApp` ist die Windows-Forms-Anwendung.

`schulAppREST` ist die ASP.NET Core REST API.

`SchulappSOAP` ist die CoreWCF SOAP API.

`start.ps1` startet beide APIs und danach die WinForms-Anwendung in der richtigen Reihenfolge.

## 3. NuGet-Pakete

Visual Studio beziehungsweise `dotnet run` stellt die benötigten NuGet-Pakete normalerweise automatisch wieder her.

Falls eine manuelle Wiederherstellung nötig ist:

```powershell
dotnet restore .\SchulApp\SchulApp.csproj
dotnet restore .\schulAppREST\schulAppREST.csproj
dotnet restore .\SchulappSOAP\SchulappSOAP.csproj
```

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

Öffne PowerShell im Hauptordner `Schulapp`.

Beispiel:

```text
C:\Users\<Benutzer>\Desktop\Cs\Schulapp
```

Starte danach:

```powershell
./start.ps1
```

Das Skript übernimmt den normalen Startablauf:

1. `schulAppREST` wird gestartet.
2. `SchulappSOAP` wird gestartet.
3. Das Skript wartet, bis die REST API erreichbar ist.
4. Das Skript wartet, bis die SOAP-WSDL erreichbar ist.
5. Danach wird die WinForms-Anwendung `SchulApp` gestartet.
6. Der LoadingScreen prüft die SQL-Datenbankverbindung.
7. Anschliessend wird das Login geöffnet.

Für den normalen Start müssen `schulAppREST`, `SchulappSOAP` und `SchulApp` deshalb nicht mehr separat mit `dotnet run` gestartet werden.

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

## 11. `.env` Datei

Die `.env` Datei wird nicht für Benutzername oder Passwort des Logins benötigt.

Benutzerkonten werden direkt in der SQL-Datenbank gespeichert.

Eine vorhandene `.env` Datei kann weiterhin für optionale lokale Konfigurationen verwendet werden.

## 12. Häufige Probleme

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

## 13. Kurzstart

Nach der einmaligen Einrichtung reicht normalerweise:

```text
1. SQL Server starten
2. PowerShell im Hauptordner Schulapp öffnen
3. ./start.ps1 ausführen
4. Warten, bis REST API und SOAP API bereit sind
5. Anmelden
```

Beim allerersten Einrichten muss davor einmal ausgeführt werden:

```text
SchulApp/SQL/SchulAppDB.sql
```
