# Set Working Directory
Split-Path $MyInvocation.MyCommand.Path | Push-Location
[Environment]::CurrentDirectory = $PWD

Remove-Item "$env:RELOADEDIIMODS/reloaded.template.aobscan/*" -Force -Recurse
dotnet publish "./reloaded.template.aobscan.csproj" -c Release -o "$env:RELOADEDIIMODS/reloaded.template.aobscan" /p:OutputPath="./bin/Release" /p:ReloadedILLink="true"

# Restore Working Directory
Pop-Location