Set-Location $PSScriptRoot

Start-Process powershell -ArgumentList `
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

dotnet run --project SchulApp.csproj