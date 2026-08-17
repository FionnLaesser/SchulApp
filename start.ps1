$startPfad = Get-Location

$restProcess = $null
$soapProcess = $null
$monitoringGestartet = $false
$monitoringCompose = "$PSScriptRoot\Monitoring\docker-compose.yml"

function Wait-ForHttp {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Name,

        [Parameter(Mandatory = $true)]
        [string]$Url,

        [int]$TimeoutSeconds = 60
    )

    Write-Host "Warte auf $Name..."
    $startZeit = Get-Date

    do {
        Start-Sleep -Seconds 1

        try {
            $response = Invoke-WebRequest `
                $Url `
                -UseBasicParsing `
                -TimeoutSec 5

            if ($response.StatusCode -ge 200 -and $response.StatusCode -lt 400) {
                Write-Host "$Name ist bereit."
                return
            }
        }
        catch {
            # Service ist noch nicht bereit.
        }

    } while (((Get-Date) - $startZeit).TotalSeconds -lt $TimeoutSeconds)

    throw "$Name war nach $TimeoutSeconds Sekunden nicht erreichbar: $Url"
}

try {
    Set-Location $PSScriptRoot

    # Docker prüfen
    if (-not (Get-Command docker -ErrorAction SilentlyContinue)) {
        throw "Docker wurde nicht gefunden. Bitte Docker Desktop installieren."
    }

    docker compose version > $null 2>&1
    if ($LASTEXITCODE -ne 0) {
        throw "Docker Compose ist nicht verfügbar."
    }

    docker info > $null 2>&1
    if ($LASTEXITCODE -ne 0) {
        throw "Docker Desktop ist nicht gestartet. Bitte Docker Desktop starten und das Skript erneut ausführen."
    }

    # Prometheus und Grafana starten
    Write-Host "Starte Prometheus und Grafana mit Docker..."
    docker compose -f $monitoringCompose up -d

    if ($LASTEXITCODE -ne 0) {
        throw "Prometheus und Grafana konnten nicht gestartet werden."
    }

    $monitoringGestartet = $true

    Wait-ForHttp -Name "Prometheus" -Url "http://localhost:9090/-/ready" -TimeoutSeconds 90
    Wait-ForHttp -Name "Grafana" -Url "http://localhost:3000/api/health" -TimeoutSeconds 90

    Write-Host "Prometheus: http://localhost:9090"
    Write-Host "Grafana:    http://localhost:3000"
    Write-Host "Grafana Login: admin / schulapp"

    # REST API starten
    $restProcess = Start-Process powershell -PassThru -ArgumentList `
        "-NoExit", `
        "-Command", "Set-Location '$PSScriptRoot\schulAppREST'; dotnet run"

    # SOAP API starten
    $soapProcess = Start-Process powershell -PassThru -ArgumentList `
        "-NoExit", `
        "-Command", "Set-Location '$PSScriptRoot\SchulappSOAP'; dotnet run"

    # Auf REST API warten
    Wait-ForHttp `
        -Name "REST API" `
        -Url "http://localhost:63636/api/PingPong/bestenliste" `
        -TimeoutSeconds 90

    # Auf SOAP API warten
    Wait-ForHttp `
        -Name "SOAP API" `
        -Url "http://localhost:5210/SchuelerService.svc?wsdl" `
        -TimeoutSeconds 90

    # Metrik-Endpunkte prüfen
    Wait-ForHttp `
        -Name "REST Prometheus Metriken" `
        -Url "http://localhost:63636/metrics" `
        -TimeoutSeconds 30

    Wait-ForHttp `
        -Name "SOAP Prometheus Metriken" `
        -Url "http://localhost:5210/metrics" `
        -TimeoutSeconds 30

    # SchulApp starten
    Set-Location "$PSScriptRoot\SchulApp"
    dotnet run --project SchulApp.csproj
}
finally {
    # REST API beenden
    if ($restProcess -and !$restProcess.HasExited) {
        taskkill /PID $restProcess.Id /T /F | Out-Null
        Write-Host "REST API wurde beendet."
    }

    # SOAP API beenden
    if ($soapProcess -and !$soapProcess.HasExited) {
        taskkill /PID $soapProcess.Id /T /F | Out-Null
        Write-Host "SOAP API wurde beendet."
    }

    # Monitoring beenden. Die Docker-Volumes bleiben bestehen, damit die Historie erhalten bleibt.
    if ($monitoringGestartet) {
        Write-Host "Beende Prometheus und Grafana..."
        docker compose -f $monitoringCompose down | Out-Null
        Write-Host "Monitoring wurde beendet."
    }

    Set-Location $startPfad
    Write-Host "Zurück zu: $startPfad"
    
}
