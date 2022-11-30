#!/usr/bin/pwsh

[CmdletBinding()]
param (
    [switch]$CPP
)

. $PSScriptRoot/Library.ps1

$binDir = $PSScriptRoot
$baseDir = Split-Path -Path $binDir -Parent

Push-Location $baseDir

Use-PsEnv

$platform = Get-Platform

try {
    $env:DEFAULT_CONFIG = "$PWD/config-cpp-client.xml"
    if ($CPP) {
        $workerCppOutDir = "$baseDir/out/$platform/Worker"
        $env:DEFAULT_CONFIG = "$PWD/config-cpp-client.xml"
        $env:IGNITE_HOME = "$PWD/toolbox/apache-ignite-$env:IGNITE_VERSION-bin"
        Write-Verbose "start_worker: c++: IGNITE_HOME: $env:IGNITE_HOME"
        if (!(Test-Path $workerCppOutDir)) {
            Write-Error "start_worker: no C++ worker found!" -ErrorAction Stop
        }
        else {
            & $workerCppOutDir/Worker
        }
    }
    else {
        $workerNetOutDir = "$baseDir/out/$platform/Worker.NET"
        & $workerNetOutDir/Worker.NET
    }
}
finally {
    Pop-Location
}