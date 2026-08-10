# SchulApp

SchulApp ist eine Schulverwaltungsanwendung, die mit C# und Windows Forms entwickelt wurde.  
Die Anwendung dient dazu, wichtige Schuldaten wie Schüler, Lehrer, Klassen, Kurse und Stundenpläne zentral und übersichtlich zu verwalten.

## Inhaltsverzeichnis

- [Funktionen](#funktionen)
- [Zusätzliche Erweiterungen](#zusätzliche-erweiterungen)
- [Verwaltete Daten](#verwaltete-daten)
  - [Schüler](#schüler)
  - [Lehrer](#lehrer)
  - [Klassen](#klassen)
  - [Kurse](#kurse)
  - [Stundenplan](#stundenplan)
- [Technologien](#technologien)
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
- Schüler nach ID sortiert anzeigen
- Lehrer anzeigen, erstellen, bearbeiten und löschen
- Informationen zu Lehrern speichern
- Klassen anzeigen, erstellen, bearbeiten und löschen
- Schüler Klassen zuordnen
- Kurse anzeigen, erstellen, bearbeiten und löschen
- Lehrer einem Kurs zuordnen
- Klassen einem Kurs zuordnen
- Stundenpläne anzeigen
- Wochentag und Uhrzeit eines Stundenplaneintrags anzeigen
- Kurs, Klasse und Lehrer eines Stundenplaneintrags anzeigen
- Aktuelles Datum und aktuelle Uhrzeit anzeigen
- Stundenplaneinträge nach Datum und Uhrzeit sortiert darstellen
- Daten übersichtlich in Tabellen darstellen
- Löschen von Datensätzen bestätigen

## Zusätzliche Erweiterungen

Die ursprünglich geplanten Anforderungen des Projekts konnten frühzeitig fertiggestellt werden. Deshalb wurde die Anwendung anschliessend freiwillig um zusätzliche Funktionen erweitert, die über den ursprünglich geplanten Umfang hinausgehen.

Zu diesen zusätzlichen Erweiterungen gehören:

- Eine eigene Einstellungsseite für die Darstellung der Anwendung
- Frei wählbare Hintergrundfarbe über einen `ColorDialog`
- Frei wählbare Textfarbe über einen `ColorDialog`
- Ein zentraler `ThemeManager`, der das gewählte Design auf die verschiedenen Fenster und Steuerelemente der Anwendung anwendet
- Speicherung der gewählten Hintergrund- und Textfarbe in der SQL-Datenbank
- Automatisches Laden des zuletzt verwendeten Designs beim Start der Anwendung
- Sofortige Aktualisierung des Designs bei bereits geöffneten Fenstern
- Möglichkeit, das Design wieder auf die Standardfarben zurückzusetzen
- Ein Loading- beziehungsweise Splashscreen beim Start der Anwendung
- Überprüfung der SQL-Verbindung während des Startvorgangs
- Verwendung eigener Bilder aus dem `images`-Ordner innerhalb der Benutzeroberfläche

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

Die verschiedenen Bereiche der Anwendung sind in eigene Ansichten aufgeteilt:

- Schüler
- Lehrer
- Klassen
- Kurse
- Stundenplan

## CRUD-Funktionen

Die Anwendung verwendet CRUD-Operationen zur Verwaltung der Daten.

| Operation | Bedeutung |
|---|---|
| Create | Neue Daten erstellen |
| Read | Gespeicherte Daten anzeigen |
| Update | Bestehende Daten bearbeiten |
| Delete | Daten löschen |

Diese Funktionen werden für die verschiedenen Bereiche der Schulverwaltung verwendet.

## Projektstruktur

```text
SchulApp
│
├── Models
│   ├── Schueler.cs
│   ├── Lehrer.cs
│   ├── Klasse.cs
│   ├── Kurs.cs
│   └── StundenplanEintrag.cs
│
├── Services
│   └── SchulService.cs
│
├── Forms
│   └── Form1.cs
│
└── Tests
    └── SchulServiceTests.cs
```

Die einzelnen Bereiche haben unterschiedliche Aufgaben:

- `Models` enthalten die verschiedenen Datentypen.
- `Services` enthalten die Programmlogik zum Erstellen, Bearbeiten und Löschen der Daten.
- `Forms` enthalten die grafische Benutzeroberfläche.
- `Tests` enthalten die Unit-Tests der Anwendung.

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

- die Windows Forms Anwendung funktioniert
- Lehrer verwaltet werden können
- Schüler verwaltet werden können
- Klassen verwaltet werden können
- Kurse verwaltet werden können
- Stundenpläne verwaltet oder angezeigt werden können
- Daten erstellt werden können
- Daten bearbeitet werden können
- Daten gelöscht werden können
- die Anwendung ohne Fehler startet
- mindestens drei Unit-Tests vorhanden sind
- das Erstellen von Daten getestet wird
- das Bearbeiten von Daten getestet wird
- das Löschen von Daten getestet wird
- alle Unit-Tests erfolgreich durchlaufen
