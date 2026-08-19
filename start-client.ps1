$startPfad = Get-Location

function Test-SchulAppEndpoint {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Url
    )

    try {
        $response = Invoke-WebRequest `
            $Url `
            -UseBasicParsing `
            -TimeoutSec 5

        return $response.StatusCode -ge 200 -and $response.StatusCode -lt 400
    }
    catch {
        return $false
    }
}

try {
    Set-Location $PSScriptRoot

    $restUrl = "http://localhost:63636/metrics"
    $soapUrl = "http://localhost:5210/SchuelerService.svc?wsdl"

    if (-not (Test-SchulAppEndpoint -Url $restUrl)) {
        throw "Die SchulApp REST API ist nicht erreichbar. Starte zuerst .\start.ps1 und lasse das erste SchulApp-Fenster geöffnet."
    }

    if (-not (Test-SchulAppEndpoint -Url $soapUrl)) {
        throw "Die SchulApp SOAP API ist nicht erreichbar. Starte zuerst .\start.ps1 und lasse das erste SchulApp-Fenster geöffnet."
    }

    $clientDirectory = Join-Path $PSScriptRoot "SchulApp"

    $clientProcess = Start-Process `
        -FilePath "dotnet" `
        -WorkingDirectory $clientDirectory `
        -ArgumentList "run", "--no-build", "--project", "SchulApp.csproj" `
        -PassThru

    Write-Host "Zusätzlicher SchulApp-Client wurde gestartet."
    Write-Host "Client PID: $($clientProcess.Id)"
    Write-Host "REST, SOAP, Prometheus und Grafana werden nicht erneut gestartet."
}
finally {
    Set-Location $startPfad
}
