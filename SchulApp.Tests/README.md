# API Unit Tests

Dieses Testprojekt enthält Unit Tests für die bestehende SchulApp sowie für die REST und SOAP API.

## REST API

Getestet werden die Schüler-Endpunkte für:

- GET alle Schüler
- GET Schüler nach ID
- POST Schüler erstellen
- PUT Schüler bearbeiten
- DELETE Schüler löschen
- Nicht vorhandene IDs

## SOAP API

Getestet werden alle aktuell vorhandenen Funktionen des `SchuelerService`:

- Schüler laden
- Schüler nach ID laden
- Schüler löschen
- Nicht vorhandene IDs

## Testdatenbank

Die API Tests verwenden `Microsoft.EntityFrameworkCore.InMemory`. Dadurch greifen die Tests nicht auf die produktive SQL Server Datenbank zu.

## Ausführen

Im Root-Ordner des Projekts:

```powershell
dotnet test
```

Oder nur dieses Testprojekt:

```powershell
dotnet test .\SchulApp.Tests\SchulApp.Tests.csproj
```
