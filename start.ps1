$startPfad = Get-Location

Set-Location $PSScriptRoot
$apiProcess = Start-Process powershell -PassThru -ArgumentList `
    "-NoExit", `
    "-Command", "Set-Location '$PSScriptRoot\schulAppREST'; dotnet run"

Write-Host "Warte auf REST API..."

do {
    Start-Sleep -Seconds 1

    try {
        $response = Invoke-WebRequest `
            "http://localhost:63636/api/PingPong/bestenliste" `
            -UseBasicParsing

        $apiBereit = $response.StatusCode -eq 200
    }
    catch {
        $apiBereit = $false
    }

} until ($apiBereit)

Write-Host "REST API ist bereit."

Set-Location "$PSScriptRoot\SchulApp"

try {
    dotnet run --project SchulApp.csproj
}
finally {
    if (!$apiProcess.HasExited) {
        taskkill /PID $apiProcess.Id /T /F | Out-Null
    }
    Set-Location $startPfad

    Write-Host "REST API wurde beendet."
    Write-Host "Zuruek zu: $startPfad"
}