# SchulApp

SchulApp ist eine Schulverwaltungsanwendung, die mit C# und Windows Forms entwickelt wurde.  
Die Anwendung dient dazu, wichtige Schuldaten wie Schüler, Lehrer, Klassen, Kurse und Stundenpläne zentral und übersichtlich zu verwalten.

Die Daten werden in einer lokalen Microsoft-SQL-Server-Datenbank gespeichert. Für mehrere Bereiche der Anwendung wird Entity Framework Core verwendet, damit Datenbankabfragen nicht mehr vollständig von Hand mit SQL geschrieben werden müssen.

## Inhaltsverzeichnis

- [Funktionen](#funktionen)
- [App-Demo](#app-demo)
- [Zusätzliche Erweiterungen](#zusätzliche-erweiterungen)
- [Verwaltete Daten](#verwaltete-daten)
  - [Schüler](#schüler)
  - [Lehrer](#lehrer)
  - [Klassen](#klassen)
  - [Kurse](#kurse)
  - [Stundenplan](#stundenplan)
- [Technologien](#technologien)
- [Entity Framework Core](#entity-framework-core)
- [Benutzeroberfläche](#benutzeroberfläche)
- [CRUD-Funktionen](#crud-funktionen)
- [Projektstruktur](#projektstruktur)
- [Unit-Tests](#unit-tests)
- [Definition of Done](#definition-of-done)

## Funktionen

Die Anwendung ermöglicht die Verwaltung verschiedener Schuldaten.

Zu den wichtigsten Funktionen gehören:

- Schüler anzeigen, erstellen, bearbeiten und löschen
- Schüler einer Klasse zuweisen
- Schüler übersichtlich in einer Tabelle anzeigen
- Lehrer anzeigen, erstellen, bearbeiten und löschen
- Informationen zu Lehrern speichern
- Klassen anzeigen, erstellen, bearbeiten und löschen
- Schüler Klassen zuordnen
- Klassenlehrer einer Klasse zuweisen
- Kurse anzeigen, erstellen, bearbeiten und löschen
- Lehrer einem Kurs zuordnen
- Klassen einem Kurs zuordnen
- Stundenplaneinträge anzeigen, erstellen, bearbeiten und löschen
- Wochentag und Uhrzeit eines Stundenplaneintrags anzeigen
- Kurs, Klasse und Lehrer eines Stundenplaneintrags anzeigen
- Raum eines Stundenplaneintrags speichern
- Aktuelles Datum und aktuelle Uhrzeit anzeigen
- Stundenplaneinträge nach Wochentag und Uhrzeit sortiert darstellen
- Daten übersichtlich in `DataGridView`-Tabellen darstellen
- Löschen von Datensätzen vor dem Ausführen bestätigen
- Verknüpfte Daten wie Lehrer, Klassen, Schüler und Kurse gemeinsam darstellen

## App-Demo

Im Ordner `Screenshots` befindet sich ein eigenes README, das die verschiedenen Seiten und Funktionen der Anwendung mit Bildern zeigt.

[Screenshots](Screenshots/README.md)

## Zusätzliche Erweiterungen

Die ursprünglich geplanten Anforderungen des Projekts konnten frühzeitig fertiggestellt werden. Deshalb wurde die Anwendung anschliessend freiwillig um zusätzliche Funktionen erweitert, die über den ursprünglich geplanten Umfang hinausgehen.

Zu diesen zusätzlichen Erweiterungen gehören:

- Eine eigene Einstellungsseite für die Darstellung der Anwendung
- Frei wählbare Hintergrundfarbe über einen `ColorDialog`
- Frei wählbare Textfarbe über einen `ColorDialog`
- Prüfung, dass Hintergrundfarbe und Textfarbe nicht identisch sein können, damit die Anwendung lesbar bleibt
- Ein zentraler `ThemeManager`, der das gewählte Design auf verschiedene Fenster und Steuerelemente der Anwendung anwendet
- Speicherung der gewählten Hintergrund- und Textfarbe in der SQL-Datenbank
- Automatisches Laden des zuletzt verwendeten Designs beim Start der Anwendung
- Sofortige Aktualisierung des Designs bei bereits geöffneten Fenstern
- Möglichkeit, das Design wieder auf die Standardfarben zurückzusetzen
- Ein Loading- beziehungsweise Splashscreen beim Start der Anwendung
- Überprüfung der SQL-Verbindung während des Startvorgangs
- Anzeige von aktuellem Datum und aktueller Uhrzeit im Stundenplan
- Verwendung eigener Bilder aus dem `images`-Ordner innerhalb der Benutzeroberfläche
- Verwendung eines eigenen App-Icons
- Einführung von Entity Framework Core für den Datenbankzugriff
- Zentrale Datenbankkonfiguration über einen `DbContext`
- Abbildung der SQL-Tabellen als C#-Models
- Verwendung von Beziehungen zwischen Models, zum Beispiel zwischen Klassen, Lehrern, Schülern und Kursen
- Reduzierung von selbst geschriebenen `SELECT`, `INSERT`, `UPDATE` und `DELETE`-SQL-Abfragen
- Verwendung von `SaveChanges()` zum Speichern von Änderungen über Entity Framework

Diese Funktionen waren nicht Bestandteil der ursprünglichen Anforderungen, sondern wurden zusätzlich umgesetzt, da die geplanten Aufgaben bereits früher abgeschlossen waren.

## Verwaltete Daten

### Schüler

Für Schüler werden unter anderem folgende Daten verwaltet:

- ID
- Name
- Klasse

### Lehrer

Für Lehrer können folgende Daten gespeichert werden:

- ID
- Name
- E-Mail
- Telefonnummer
- Informationen und Titel
- Fach

### Klassen

Für Klassen werden folgende Daten verwaltet:

- ID
- Klassenname
- Klassenlehrer
- Anzahl zugeordneter Schüler

### Kurse

Für Kurse werden folgende Daten verwaltet:

- ID
- Kursname
- Lehrer
- Klasse

### Stundenplan

Ein Stundenplaneintrag kann folgende Daten enthalten:

- ID
- Wochentag
- Startzeit
- Endzeit
- Kurs
- Lehrer
- Raum
- Klasse

## Technologien

Für die Entwicklung werden folgende Technologien verwendet:

- C#
- .NET
- Windows Forms
- Visual Studio 2022
- Microsoft SQL Server 2022
- Entity Framework Core
- Microsoft Entity Framework Core SQL Server Provider
- Microsoft.Data.SqlClient
- Git
- GitHub
- GitLab

### NuGet-Pakete

Für Entity Framework Core werden unter anderem folgende NuGet-Pakete verwendet:

```text
Microsoft.EntityFrameworkCore.SqlServer
Microsoft.EntityFrameworkCore.Tools
```

## Entity Framework Core

Für mehrere Datenbankzugriffe wurde die Anwendung von direkt geschriebenen SQL-Abfragen auf Entity Framework Core umgestellt.

Dabei wird ein zentraler `SchulAppContext` verwendet. Dieser erbt von `DbContext` und stellt über `DbSet` die verschiedenen Tabellen der Datenbank zur Verfügung.

Beispielsweise können Datensätze dadurch direkt über C# geladen werden:

```csharp
using SchulAppContext context = new SchulAppContext();

var klassen = context.Klassen.ToList();
```

Ein neuer Datensatz kann über das entsprechende Model erstellt und gespeichert werden:

```csharp
context.Klassen.Add(neueKlasse);
context.SaveChanges();
```

Auch Bearbeiten und Löschen können über die Models durchgeführt werden, ohne die SQL-Befehle vollständig von Hand zu schreiben.

Die SQL-Tabellen werden dabei unter anderem durch folgende Models dargestellt:

- `Einstellung`
- `SchuelerModel`
- `LehrerModel`
- `KlasseModel`
- `KursModel`
- `StundenplanModel`

Zwischen den Models können Beziehungen verwendet werden. Dadurch können beispielsweise bei einem Kurs direkt die zugehörige Klasse und der zugehörige Lehrer abgefragt werden.

Entity Framework Core wird insbesondere für Datenbankzugriffe in Bereichen wie Einstellungen, Klassen, Kurse und Stundenplan eingesetzt.

## Benutzeroberfläche

Die grafische Benutzeroberfläche wird mit Windows Forms umgesetzt.

Dabei werden unter anderem folgende Elemente verwendet:

- `Label`
- `TextBox`
- `ComboBox`
- `Button`
- `DataGridView`
- `PictureBox`
- `ColorDialog`
- `DateTimePicker`

Die verschiedenen Bereiche der Anwendung sind in eigene Ansichten aufgeteilt:

- Hauptmenü
- Schüler
- Lehrer
- Klassen
- Kurse
- Stundenplan
- Einstellungen
- LoadingScreen

## CRUD-Funktionen

Die Anwendung verwendet CRUD-Operationen zur Verwaltung der Daten.

| Operation | Bedeutung |
|---|---|
| Create | Neue Daten erstellen |
| Read | Gespeicherte Daten anzeigen |
| Update | Bestehende Daten bearbeiten |
| Delete | Daten löschen |

Diese Funktionen werden für die verschiedenen Bereiche der Schulverwaltung verwendet.

Mit Entity Framework können diese Operationen beispielsweise über `Add()`, Abfragen mit LINQ, Änderungen an Model-Eigenschaften, `Remove()` und `SaveChanges()` durchgeführt werden.

## Projektstruktur

Die Anwendung ist in verschiedene Bereiche aufgeteilt. Eine vereinfachte Projektstruktur sieht folgendermassen aus:

```text
SchulApp
│
├── Data
│   └── SchulAppContext.cs
│
├── Models
│   ├── Einstellung.cs
│   ├── SchuelerModel.cs
│   ├── LehrerModel.cs
│   ├── KlasseModel.cs
│   ├── KursModel.cs
│   └── StundenplanModel.cs
│
├── Services
│
├── images
│
├── Screenshots
│   └── README.md
│
├── Hauptmenue.cs
├── Schueler.cs
├── Lehrer.cs
├── Klassen.cs
├── Kurse.cs
├── Stundenplan.cs
├── Einstellungen.cs
├── LoadingScreen.cs
├── ThemeManager.cs
└── Program.cs
```

Die einzelnen Bereiche haben unterschiedliche Aufgaben:

- `Data` enthält den Datenbankzugriff und den Entity-Framework-`DbContext`.
- `Models` enthalten die C#-Abbildungen der Datenbanktabellen und deren Beziehungen.
- `Services` können zusätzliche Programmlogik und Repository-Funktionen enthalten.
- Die Windows-Forms-Dateien enthalten die grafische Benutzeroberfläche und deren Ereignisse.
- `ThemeManager` verwaltet die Darstellung der Anwendung zentral.
- `images` enthält Bilder und Icons der Anwendung.
- `Screenshots` enthält Bilder für die Dokumentation der Anwendung.

## Unit-Tests

Für das Projekt werden mindestens drei Unit-Tests erstellt.

Die Tests überprüfen wichtige CRUD-Funktionen.

### Daten erstellen

Es wird überprüft, ob ein neuer Schüler korrekt erstellt und gespeichert werden kann.

### Daten bearbeiten

Es wird überprüft, ob die Daten eines bestehenden Schülers geändert und korrekt gespeichert werden können.

### Daten löschen

Es wird überprüft, ob ein Schüler korrekt gelöscht wird und danach nicht mehr vorhanden ist.

## Definition of Done

Das Projekt gilt als abgeschlossen, wenn:

- die Windows-Forms-Anwendung funktioniert
- Lehrer verwaltet werden können
- Schüler verwaltet werden können
- Klassen verwaltet werden können
- Kurse verwaltet werden können
- Stundenpläne verwaltet werden können
- Daten erstellt werden können
- Daten bearbeitet werden können
- Daten gelöscht werden können
- die Daten korrekt in Microsoft SQL Server gespeichert werden
- die Datenbankverbindung beim Start überprüft werden kann
- die Anwendung ohne Fehler startet
- mindestens drei Unit-Tests vorhanden sind
- das Erstellen von Daten getestet wird
- das Bearbeiten von Daten getestet wird
- das Löschen von Daten getestet wird
- alle Unit-Tests erfolgreich durchlaufen
