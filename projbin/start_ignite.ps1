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
    $env:IGNITE_HOME = "$baseDir/toolbox/apache-ignite-$env:IGNITE_VERSION-bin"
    $env:DEFAULT_CONFIG = "$PWD/config-cpp.xml"

    Write-Output "start_ignite: Clearing out old persistence data..."
    Remove-Item -Recurse "$env:IGNITE_HOME/work"

    Write-Output "start_ignite: Start ignite instance..."
    & $env:IGNITE_HOME/bin/ignite
}
finally {
    Pop-Location
}
