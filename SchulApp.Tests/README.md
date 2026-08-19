# SchulApp Tests

Das Projekt `SchulApp.Tests` enthält automatisierte **xUnit-Tests** für die WinForms-Service-Schicht, REST API, SOAP API, Datentransfer-Funktionen und zentrale Fehlerbehandlung.

Zur Projektübersicht: [`../README.md`](../README.md)

## Aktueller Testumfang

Auf dem aktuellen `master` sind **32 Tests** vorhanden.

| Testklasse | Tests | Schwerpunkt |
|---|---:|---|
| `SchuelerServiceTests` | 3 | lokale Schüler-Service-Logik |
| `RestSchuelerControllerTests` | 8 | REST-Schüler-CRUD und NotFound-Szenarien |
| `SoapSchuelerServiceTests` | 7 | SOAP-Schüler-CRUD und Audit Log |
| `GlobalExceptionMiddlewareTests` | 6 | zentrale REST-Fehlerbehandlung |
| `DataTransferServiceTests` | 6 | CSV, PDF, Validierung und Sonderzeichen |
| `StudentImportTemplateServiceTests` | 2 | CSV-Importvorlage und Parser-Kompatibilität |
| **Gesamt** | **32** | |

## Lokale Schüler-Service-Tests

Datei:

```text
SchuelerServiceTests.cs
```

Die Tests verwenden `FakeSchuelerRepository` und benötigen keine echte SQL-Datenbank.

Getestet werden:

- Schüler erstellen
- Schüler bearbeiten
- Schüler löschen

## REST API Tests

Datei:

```text
RestSchuelerControllerTests.cs
```

Getestet werden:

- alle Schüler laden
- Schüler nach ID laden
- nicht vorhandene ID bei GET
- Schüler erstellen
- Schüler bearbeiten
- nicht vorhandene ID bei UPDATE
- Schüler löschen
- nicht vorhandene ID bei DELETE

Die Tests prüfen unter anderem `OkObjectResult` und `NotFoundResult`.

## SOAP API Tests

Datei:

```text
SoapSchuelerServiceTests.cs
```

Der aktuelle SOAP-Service besitzt vollständiges Schüler-CRUD.

Getestet werden:

- `GetSchueler`
- `GetSchuelerById`
- `AddSchueler`
- `UpdateSchueler`
- `DeleteSchueler`
- nicht vorhandene Schüler
- Audit-Log-Einträge für SOAP-Änderungen

Die Tests prüfen bei Create, Update und Delete zusätzlich relevante Audit-Informationen wie Aktion, Entität, Quelle und Änderungen.

## Global Exception Middleware

Datei:

```text
GlobalExceptionMiddlewareTests.cs
```

Getestet werden:

- normaler Request ohne Exception
- unbekannte Exception als HTTP `500`
- `ArgumentException` als HTTP `400`
- `KeyNotFoundException` als HTTP `404`
- `UnauthorizedAccessException` als HTTP `403`
- `InvalidOperationException` als HTTP `409`

Zusätzlich wird geprüft, dass interne Fehlermeldungen nicht ungefiltert an den Client weitergegeben werden.

## Datenimport und Export

Datei:

```text
DataTransferServiceTests.cs
```

Getestet werden unter anderem:

- CSV-Escaping
- Sonderzeichen wie `ä`, `ö` und `ü`
- Import von mindestens 50 gültigen Schülern
- fehlende oder ungültige Daten
- Duplikate
- Trennzeichen innerhalb von Anführungszeichen
- grundlegende PDF-Erzeugung

## CSV-Importvorlage

Datei:

```text
StudentImportTemplateServiceTests.cs
```

Getestet wird:

- die Vorlage enthält `Name;KlasseId`
- eine sinnvolle Beispielzeile ist vorhanden
- die erzeugte Vorlage kann direkt durch den bestehenden Schüler-Importparser validiert werden

## Testdatenbanken

REST- und SOAP-Tests verwenden:

```text
Microsoft.EntityFrameworkCore.InMemory
```

Jeder Test erzeugt eine eigene InMemory-Datenbank mit eindeutigem Namen. Dadurch:

- wird `SchulAppDB` nicht verändert
- beeinflussen sich Tests nicht gegenseitig
- bleiben lokale Schul- und Benutzerdaten unverändert

## Projektabhängigkeiten

Das Testprojekt referenziert:

```text
SchulApp/SchulApp.csproj
schulAppREST/schulAppREST.csproj
SchulappSOAP/SchulappSOAP.csproj
```

Für `schulAppREST` wird ein Assembly-Alias verwendet. Damit können gleichnamige Models und DbContexts aus den verschiedenen Projekten sauber getrennt werden.

## Tests lokal ausführen

Im Repository-Root:

```powershell
dotnet test .\SchulApp.Tests\SchulApp.Tests.csproj
```

Release-Build mit anschliessenden Tests:

```powershell
dotnet build .\SchulApp.slnx --configuration Release
dotnet test .\SchulApp.Tests\SchulApp.Tests.csproj --configuration Release --no-build
```

## Code Coverage lokal

Das Testprojekt verwendet `coverlet.collector`.

Coverage erzeugen:

```powershell
dotnet test .\SchulApp.Tests\SchulApp.Tests.csproj `
    --configuration Release `
    --collect:"XPlat Code Coverage" `
    --results-directory TestResults
```

Dadurch wird eine `coverage.cobertura.xml` unter `TestResults` erzeugt.

## GitHub Actions

Workflow:

```text
.github/workflows/build-and-test.yml
```

Der Workflow läuft bei:

```text
push
pull_request
```

Auf `windows-latest` mit .NET 10 werden folgende Schritte ausgeführt:

1. Repository auschecken
2. .NET 10 einrichten
3. Dependencies wiederherstellen
4. Solution in Release bauen
5. Tests mit XPlat Code Coverage ausführen
6. ReportGenerator installieren
7. HTML-Coverage-Report erzeugen
8. Coverage-Prozentwert im GitHub Actions Summary anzeigen
9. Coverage-Report als Artifact hochladen

Artifact:

```text
code-coverage-report
```

Die Artifact-Retention beträgt aktuell 14 Tage.

## Coverage-Dokumentation

Der konkrete Coverage-Prozentwert wird bewusst nicht statisch in dieser Datei gespeichert. GitHub Actions berechnet ihn bei jedem Lauf neu und zeigt dadurch immer den Wert des jeweiligen Commits.

Für den detaillierten Report das Artifact `code-coverage-report` aus einem Workflow-Lauf herunterladen und `index.html` öffnen.

## Neue Tests hinzufügen

Neue Funktionen sollten nach Möglichkeit Tests für folgende Fälle erhalten:

- erfolgreicher Standardfall
- ungültige Eingaben
- nicht vorhandene Datensätze
- Berechtigungsfehler, wenn relevant
- Datenbankänderungen
- Seiteneffekte wie Audit Logging
- Sonderzeichen und Randfälle bei Dateioperationen

Tests sollen reproduzierbar sein und nicht von einer bereits vorhandenen lokalen `SchulAppDB` abhängen.
