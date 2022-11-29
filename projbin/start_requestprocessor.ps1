#!/usr/bin/pwsh

[CmdletBinding()]
param (
)

. $PSScriptRoot/Library.ps1

$binDir = $PSScriptRoot
$baseDir = Split-Path -Path $binDir -Parent

Push-Location $baseDir

Use-PsEnv

try {
    $env:DEFAULT_CONFIG = "$PWD/config-cpp-client.xml"
    Set-Location src/RequestProcessor
    dotnet run
}
finally {
    Pop-Location
}