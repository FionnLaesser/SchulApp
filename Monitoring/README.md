# SchulApp Monitoring

Das Monitoring besteht aus Prometheus und Grafana und wird mit Docker Compose gestartet.

## Voraussetzungen

- Docker Desktop ist installiert und gestartet.
- REST API, SOAP API und SQL Server laufen lokal wie bisher.
- Die Dateien aus diesem ZIP werden in die entsprechenden Projektordner kopiert.

## Start

Normal wie bisher im Projektordner:

```powershell
.\start.ps1
```

Das Skript startet automatisch:

1. Prometheus in Docker
2. Grafana in Docker
3. REST API
4. SOAP API
5. SchulApp

Beim Beenden der SchulApp werden REST API, SOAP API, Prometheus und Grafana beendet. Die Docker-Volumes werden nicht geloescht, deshalb bleibt die Monitoring-Historie erhalten.

## URLs

- Grafana: http://localhost:3000
- Prometheus: http://localhost:9090
- REST Metriken: http://localhost:63636/metrics
- SOAP Metriken: http://localhost:5210/metrics

Grafana Login:

- Benutzer: `admin`
- Passwort: `schulapp`

Das Dashboard wird automatisch im Grafana-Ordner `SchulApp` geladen.

## Was wird angezeigt?

- REST API erreichbar oder nicht
- SOAP API erreichbar oder nicht
- Datenbank erreichbar oder nicht
- Anzahl REST Requests
- Anzahl SOAP Requests
- HTTP-Fehler
- durchschnittliche Antwortzeiten
- Requests pro Minute als Verlauf
- Antwortzeiten als Verlauf
- Fehler als Verlauf

Prometheus speichert die Metriken bis zu 30 Tage in einem Docker-Volume.

## Warum wurden die HTTP-Bindings angepasst?

Prometheus läuft in einem Docker-Container. Damit der Container die lokal gestarteten APIs erreichen kann, lauschen die HTTP-Endpunkte der REST API und SOAP API auf `0.0.0.0`. Prometheus greift aus Docker über `host.docker.internal` darauf zu.

Die REST API behält ihren HTTPS-Endpunkt auf `https://localhost:63635`. Der `/metrics`-Endpunkt bleibt absichtlich über HTTP erreichbar.

## Monitoring manuell starten

```powershell
cd Monitoring
docker compose up -d
```

## Monitoring manuell stoppen

```powershell
cd Monitoring
docker compose down
```

## Monitoring-Daten komplett loeschen

Achtung: Dieser Befehl loescht auch die gespeicherte Prometheus- und Grafana-Historie.

```powershell
cd Monitoring
docker compose down -v
```
