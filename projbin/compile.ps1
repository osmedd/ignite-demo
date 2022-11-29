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
    
$igniteCppClientHome = "$PWD/$env:TOOLBOX_DIR/$platform/apache-ignite-client/$env:IGNITE_VERSION"
Write-Verbose "compile: using Apache Ignite C++ Client in $igniteCppClientHome"
$igniteCppSrcHome = "$PWD/$env:TOOLBOX_DIR/apache-ignite-$env:IGNITE_VERSION-bin/platforms/cpp"

$env:OPENSSL_ROOT_DIR = "$PWD/$env:TOOLBOX_DIR/$platform/openssl/OpenSSL_${env:OPENSSL_VERSION}-vc16"

try {
    Write-Output "compile: Building .NET components"
    dotnet build
    if ($LASTEXITCODE -ne 0) {
        Write-Error "compile: .NET build failed!" -ErrorAction Stop
    }
    
    if (!(Test-Path $igniteCppClientHome)) {
        Write-Output "compile: Building Apache Ignite C++ client ($env:IGNITE_VERSION)"
        if (!(Test-Path $igniteCppClientHome)) {
            mkdir -p $igniteCppClientHome | Out-Null
        }
        $igniteCppClientBuildDir = "$baseDir/build/$platform/apache-ignite-client"
        if (Test-Path $igniteCppClientBuildDir) {
            Write-Verbose "compile: cleaning up previous Apache Ignite C++ client build directory ($igniteCppClientBuildDir)"
            Remove-item -Recurse $igniteCppClientBuildDir | Out-Null
        }
        mkdir -p $igniteCppClientBuildDir
        Set-Location $igniteCppClientBuildDir
        cmake $igniteCppSrcHome -DCMAKE_GENERATOR_PLATFORM=x64 "-DCMAKE_INSTALL_PREFIX=$igniteCppClientHome" -DWITH_THIN_CLIENT=ON
        cmake --build . --config Release
        cmake --install .
        if ($LASTEXITCODE -ne 0) {
            Write-Error "compile: Apache Ignite client c++ build failed!" -ErrorAction Stop
        }
    }

    Write-Output "compile: Building C++ components"
    $workerCppSrcHome = "$baseDir/src/Worker"
    $workerCppBuildDir = "$baseDir/build/$platform/Worker"
    if (Test-Path $workerCppBuildDir) { Remove-Item -Recurse $workerCppBuildDir }
    mkdir -p $workerCppBuildDir | Out-Null
    Set-Location $workerCppBuildDir
    cmake $workerCppSrcHome
    if ($LASTEXITCODE -ne 0) {
        Write-Error "compile: c++ configure failed!" -ErrorAction Stop
    }
    cmake --build . --config Release
    if ($LASTEXITCODE -ne 0) {
        Write-Error "compile: c++ build failed!" -ErrorAction Stop
    }
}
finally {
    Pop-Location
}
