$startPfad = Get-Location

Set-Location $PSScriptRoot

# REST API starten
$restProcess = Start-Process powershell -PassThru -ArgumentList `
    "-NoExit", `
    "-Command", "Set-Location '$PSScriptRoot\schulAppREST'; dotnet run"

# SOAP API starten
$soapProcess = Start-Process powershell -PassThru -ArgumentList `
    "-NoExit", `
    "-Command", "Set-Location '$PSScriptRoot\SchulappSOAP'; dotnet run"

# Auf REST API warten
Write-Host "Warte auf REST API..."

do {
    Start-Sleep -Seconds 1

    try {
        $response = Invoke-WebRequest `
            "http://localhost:63636/api/PingPong/bestenliste" `
            -UseBasicParsing

        $restBereit = $response.StatusCode -eq 200
    }
    catch {
        $restBereit = $false
    }

} until ($restBereit)

Write-Host "REST API ist bereit."


# Auf SOAP API warten
Write-Host "Warte auf SOAP API..."

do {
    Start-Sleep -Seconds 1

    try {
        $response = Invoke-WebRequest `
            "http://localhost:5210/SchuelerService.svc?wsdl" `
            -UseBasicParsing

        $soapBereit = $response.StatusCode -eq 200
    }
    catch {
        $soapBereit = $false
    }

} until ($soapBereit)

Write-Host "SOAP API ist bereit."


# SchulApp starten
Set-Location "$PSScriptRoot\SchulApp"

try {
    dotnet run --project SchulApp.csproj
}
finally {

    # REST API beenden
    if (!$restProcess.HasExited) {
        taskkill /PID $restProcess.Id /T /F | Out-Null
    }

    # SOAP API beenden
    if (!$soapProcess.HasExited) {
        taskkill /PID $soapProcess.Id /T /F | Out-Null
    }

    Set-Location $startPfad

    Write-Host "REST API wurde beendet."
    Write-Host "SOAP API wurde beendet."
    Write-Host "Zurueck zu: $startPfad"
}