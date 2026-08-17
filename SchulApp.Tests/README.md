# API Unit Tests

Dieses Testprojekt enthält automatisierte **xUnit-Tests** für die bestehende SchulApp sowie für die REST und SOAP API.

Aktuell enthält das Projekt **16 Tests**:

- 3 Tests für die lokale `SchuelerService`-Schicht
- 8 Tests für den REST-`SchuelerController`
- 5 Tests für den SOAP-`SchuelerService`

## Lokaler Schüler-Service

`SchuelerServiceTests.cs` verwendet `FakeSchuelerRepository` und testet die Geschäftslogik ohne echte Datenbank.

Getestet werden:

- Schüler erstellen
- Schüler bearbeiten
- Schüler löschen

## REST API

`RestSchuelerControllerTests.cs` testet die Schüler-Endpunkte für:

- GET alle Schüler
- GET Schüler nach ID
- POST Schüler erstellen
- PUT Schüler bearbeiten
- DELETE Schüler löschen
- nicht vorhandene IDs
- erwartete `Ok`- und `NotFound`-Resultate

## SOAP API

`SoapSchuelerServiceTests.cs` testet alle aktuell vorhandenen Funktionen des `SchuelerService`:

- Schüler laden
- Schüler nach ID laden
- Schüler löschen
- nicht vorhandene IDs

## Testdatenbank

Die REST- und SOAP-Tests verwenden `Microsoft.EntityFrameworkCore.InMemory`.

Jeder Test erzeugt eine eigene InMemory-Datenbank mit einem eindeutigen Namen. Dadurch:

- greifen die Tests nicht auf die produktive SQL-Server-Datenbank `SchulAppDB` zu
- beeinflussen sich einzelne Tests nicht gegenseitig
- bleiben echte Schul- und Benutzerdaten unverändert

## Projektabhängigkeiten

Das Testprojekt referenziert:

```text
SchulApp/SchulApp.csproj
schulAppREST/schulAppREST.csproj
SchulappSOAP/SchulappSOAP.csproj
```

Für die REST API wird ein Assembly-Alias verwendet, damit gleichnamige Models und DbContexts sauber voneinander getrennt werden können.

## Ausführen

Im Root-Ordner des Projekts:

```powershell
dotnet test
```

Oder nur dieses Testprojekt:

```powershell
dotnet test .\SchulApp.Tests\SchulApp.Tests.csproj
```

Für einen Release-nahen lokalen Lauf:

```powershell
dotnet build .\SchulApp.slnx --configuration Release
dotnet test .\SchulApp.Tests\SchulApp.Tests.csproj --configuration Release --no-build
```

## GitHub Actions

Die Tests sind in `.github/workflows/build-and-test.yml` in die CI-Pipeline integriert.

Der Workflow läuft automatisch bei:

- `push`
- `pull_request`

Dabei werden auf `windows-latest` mit .NET 10 folgende Schritte ausgeführt:

```text
1. Repository auschecken
2. .NET 10 einrichten
3. SchulApp.slnx wiederherstellen
4. SchulApp.slnx in Release bauen
5. SchulApp.Tests in Release ausführen
```

Damit werden Build-Fehler und fehlschlagende Tests automatisch in GitHub sichtbar.
