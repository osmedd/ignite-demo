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

try {
    $env:DEFAULT_CONFIG = "$PWD/config-cpp-client.xml"
    if ($CPP) {
        $platform = Get-Platform
        $workerCppBuildDir = "$baseDir/build/$platform/Worker/Release"
        $env:DEFAULT_CONFIG = "$PWD/config-cpp-client.xml"
        $env:IGNITE_HOME = "$PWD/toolbox/apache-ignite-$env:IGNITE_VERSION-bin"
        Write-Verbose "start_worker: c++: IGNITE_HOME: $env:IGNITE_HOME"
        if (!(Test-Path $workerCppBuildDir)) {
            Write-Error "start_worker: no C++ worker found!" -ErrorAction Stop
        }
        else {
            Set-Location $workerCppBuildDir
            ./Worker
        }
    }
    else {
        Set-Location src/Worker.NET
        dotnet run
    }
}
finally {
    Pop-Location
}