# Setup

Damit die Anwendung gestartet werden kann, müssen zuerst die Umgebungsvariablen eingerichtet werden.

## 1. `.env` Datei erstellen

Kopiere die vorhandene `.env.example` Datei und benenne die Kopie in `.env` um.

```powershell
copy .env.example .env
```

Die `.env` Datei enthält lokale Einstellungen und sollte nicht auf GitHub hochgeladen werden.

## 2. Passwort-Hash erstellen

Öffne die Datei:

```text
BcryptHasher.cs
```

Trage dort das gewünschte Passwort ein und starte den Hasher.

Dadurch wird ein BCrypt-Hash des Passworts erstellt.

## 3. Hash in `.env` eintragen

Kopiere den generierten BCrypt-Hash und füge ihn in die entsprechende Variable der `.env` Datei ein.

Beispiel:

```env
ADMIN_PASSWORD_HASH=hier_den_generierten_hash_einfügen
```

## 4. Anwendung starten

Nachdem die `.env` Datei eingerichtet und der Passwort-Hash eingetragen wurde, kann die Anwendung normal gestartet werden.
