# SchulApp Monitoring

Das Monitoring der SchulApp basiert auf **Prometheus** und **Grafana** und wird über Docker Compose betrieben. Es erfasst Metriken der REST API und SOAP API und zeigt zusätzlich den Datenbankstatus an.

Zur Projektübersicht: [`../README.md`](../README.md)

## Komponenten

| Komponente | Version / Aufgabe |
|---|---|
| Prometheus | `v3.13.1`, sammelt und speichert Metriken |
| Grafana | `13.1.0`, visualisiert die Prometheus-Daten |
| REST API | stellt `/metrics` bereit und meldet den Datenbankstatus |
| SOAP API | stellt `/metrics` bereit |
| Docker Compose | startet Prometheus und Grafana mit persistenten Volumes |

## Voraussetzungen

- Docker Desktop installiert und gestartet
- Docker Compose verfügbar
- .NET 10 SDK installiert
- SQL Server 2022 läuft
- `SchulAppDB` ist eingerichtet

## Normaler Start

Im Repository-Root:

```powershell
.\start.ps1
```

Das Startskript:

1. prüft Docker und Docker Compose
2. startet Prometheus und Grafana
3. wartet auf beide Monitoring-Dienste
4. startet REST und SOAP
5. wartet auf REST API und SOAP-WSDL
6. prüft beide `/metrics`-Endpunkte
7. startet danach die WinForms-Anwendung

Beim normalen Beenden werden die Container gestoppt. Die Docker-Volumes bleiben erhalten.

## Lokale URLs

| Dienst | URL |
|---|---|
| Grafana | `http://localhost:3000` |
| Prometheus | `http://localhost:9090` |
| REST Metrics | `http://localhost:63636/metrics` |
| SOAP Metrics | `http://localhost:5210/metrics` |
| Health Check | `https://localhost:63635/health` |
| Swagger | `https://localhost:63635/swagger` |

Grafana Login:

```text
Benutzer: admin
Passwort: schulapp
```

## Prometheus-Konfiguration

Konfiguration:

```text
Monitoring/prometheus/prometheus.yml
```

Prometheus fragt die REST- und SOAP-Metriken alle **5 Sekunden** ab.

Da Prometheus in Docker läuft und die APIs lokal auf Windows gestartet werden, greift der Container über `host.docker.internal` auf die Host-Dienste zu.

Die HTTP-Endpunkte der APIs sind deshalb für das lokale Monitoring erreichbar. Die REST API behält zusätzlich ihren HTTPS-Endpunkt für normale API-Aufrufe.

## Datenspeicherung

Prometheus verwendet das Volume:

```text
prometheus-data
```

Grafana verwendet:

```text
grafana-data
```

Prometheus ist aktuell mit einer Retention von **30 Tagen** konfiguriert.

Das normale `docker compose down` löscht diese Volumes nicht.

## Grafana Provisioning

Grafana wird automatisch provisioniert.

Wichtige Pfade:

```text
Monitoring/grafana/provisioning/datasources/prometheus.yml
Monitoring/grafana/provisioning/dashboards/dashboards.yml
Monitoring/grafana/dashboards/schulapp-monitoring.json
```

Dadurch stehen Prometheus als Datenquelle und das SchulApp-Dashboard ohne manuelle Einrichtung zur Verfügung.

## Angezeigte Informationen

Das Dashboard verwendet die von den APIs bereitgestellten Prometheus-Metriken. Dazu gehören unter anderem:

- REST API Requests
- SOAP API Requests
- Antwortzeiten
- HTTP-Statuscodes
- Fehler
- Request-Verläufe
- Datenbankstatus
- Erreichbarkeit der überwachten Komponenten

## Health Check und Monitoring

Der Health Check und Prometheus haben unterschiedliche Aufgaben.

### Health Check

```text
https://localhost:63635/health
```

Er prüft beim Aufruf den aktuellen Zustand von:

- REST API
- SQL-Datenbank
- SOAP API

Erwartete Statuscodes:

- HTTP `200` bei gesundem Zustand
- HTTP `503`, wenn eine benötigte Abhängigkeit nicht erreichbar ist

### Prometheus

Prometheus sammelt fortlaufend technische Metriken und speichert deren Verlauf. Dadurch können beispielsweise steigende Fehlerzahlen oder Antwortzeiten über einen längeren Zeitraum erkannt werden.

## Monitoring manuell starten

Aus dem Repository-Root:

```powershell
docker compose -f .\Monitoring\docker-compose.yml up -d
```

Status prüfen:

```powershell
docker compose -f .\Monitoring\docker-compose.yml ps
```

## Monitoring manuell stoppen

```powershell
docker compose -f .\Monitoring\docker-compose.yml down
```

Die gespeicherten Daten bleiben erhalten.

## Monitoring-Daten vollständig löschen

```powershell
docker compose -f .\Monitoring\docker-compose.yml down -v
```

Dieser Befehl löscht auch die gespeicherte Prometheus- und Grafana-Historie.

## Troubleshooting

### Prometheus nicht bereit

Prüfen:

```text
http://localhost:9090/-/ready
```

### Grafana nicht bereit

Prüfen:

```text
http://localhost:3000/api/health
```

### REST Metrics nicht erreichbar

Prüfen:

```text
http://localhost:63636/metrics
```

### SOAP Metrics nicht erreichbar

Prüfen:

```text
http://localhost:5210/metrics
```

### Container erreichen die APIs nicht

Prüfen:

- REST API läuft auf Port `63636`
- SOAP API läuft auf Port `5210`
- `host.docker.internal` ist im Container erreichbar
- Docker Desktop läuft
- lokale Firewall blockiert die Ports nicht

Weitere allgemeine Fehlerbehebung steht in [`../SETUP.md`](../SETUP.md).
