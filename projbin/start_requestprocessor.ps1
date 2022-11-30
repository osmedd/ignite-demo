#!/usr/bin/pwsh

[CmdletBinding()]
param (
)

. $PSScriptRoot/Library.ps1

$binDir = $PSScriptRoot
$baseDir = Split-Path -Path $binDir -Parent

Push-Location $baseDir

Use-PsEnv

$platform = Get-Platform

try {
    $env:ASPNETCORE_ENVIRONMENT = "Development"
    $env:DEFAULT_CONFIG = "$PWD/config-cpp-client.xml"
    $requestprocessorOutDir = "$baseDir/out/$platform/RequestProcessor"
    & $requestprocessorOutDir/RequestProcessor
}
finally {
    Pop-Location
}