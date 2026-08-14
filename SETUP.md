# Setup

Diese Anleitung beschreibt die lokale Einrichtung und den Start der aktuellen **SchulApp** mit **WinForms**, **SQL Server**, **schulAppREST**, **PingPong** und **Bestenliste**.

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
└── schulAppREST
    └── schulAppREST.csproj
```

`SchulApp` ist die Windows-Forms-Anwendung.

`schulAppREST` ist die ASP.NET Core REST API.

`start.ps1` startet beide Anwendungen in der richtigen Reihenfolge.

## 3. NuGet-Pakete

Visual Studio beziehungsweise `dotnet run` stellt die benötigten NuGet-Pakete normalerweise automatisch wieder her.

Falls eine manuelle Wiederherstellung nötig ist:

```powershell
dotnet restore .\SchulApp\SchulApp.csproj
dotnet restore .\schulAppREST\schulAppREST.csproj
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
2. Das Skript wartet, bis die REST API erreichbar ist.
3. Danach wird die WinForms-Anwendung `SchulApp` gestartet.
4. Der LoadingScreen prüft die SQL-Datenbankverbindung.
5. Anschliessend wird das Login geöffnet.

Für den normalen Start müssen `schulAppREST` und `SchulApp` deshalb nicht mehr separat mit `dotnet run` gestartet werden.

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

## 8. PingPong testen

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

## 9. Bestenliste testen

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

## 10. `.env` Datei

Die `.env` Datei wird nicht für Benutzername oder Passwort des Logins benötigt.

Benutzerkonten werden direkt in der SQL-Datenbank gespeichert.

Eine vorhandene `.env` Datei kann weiterhin für optionale lokale Konfigurationen verwendet werden.

## 11. Häufige Probleme

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

## 12. Kurzstart

Nach der einmaligen Einrichtung reicht normalerweise:

```text
1. SQL Server starten
2. PowerShell im Hauptordner Schulapp öffnen
3. ./start.ps1 ausführen
4. Anmelden
```

Beim allerersten Einrichten muss davor einmal ausgeführt werden:

```text
SchulApp/SQL/SchulAppDB.sql
```
