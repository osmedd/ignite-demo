#! /usr/bin/pwsh

# Add/update toolbox contents

[CmdletBinding()]
param (
)

. "$PSScriptRoot/Library.ps1"

$binDir = $PSScriptRoot
$baseDir = Split-Path -Path $binDir -Parent

Push-Location $baseDir

Use-PsEnv

$platform = Get-Platform
Write-Output "Platform: $platform"

$toolboxDir = "$PWD/$env:TOOLBOX_DIR"
if (!(Test-Path $toolboxDir)) {
    Write-Verbose "build_toolbox: creating toolbox directory: $toolboxDir"
    mkdir -p $toolboxDir | Out-Null
}
$toolboxPlatformDir = "$toolboxDir/$platform"
if (!(Test-Path "$toolboxPlatformDir")) {
    Write-Verbose "build_toolbox: creating toolbox platform directory: $toolboxPlatformDir"
    mkdir -p $toolboxPlatformDir
}

$downloadsDir = "$PWD/$env:DOWNLOADS_DIR"
if (!(Test-Path $downloadsDir)) {
    Write-Verbose "build_toolbox: creating downloads directory: $downloadsDir"
    mkdir -p $downloadsDir | Out-Null
}
$downloadsPlatformDir = "$downloadsDir/$platform"
if (!(Test-Path $downloadsPlatformDir)) {
    Write-Verbose "build_toolbox: creating downloads platform directory: $downloadsPlatformDir"
    mkdir -p $downloadsPlatformDir | Out-Null
}

$igniteVersion = $env:IGNITE_VERSION
$igniteZip = "apache-ignite-$igniteVersion-bin.zip"
$igniteDownloadZip = "$downloadsDir/$igniteZip"
$igniteDownloadUrl = "https://dlcdn.apache.org/ignite/$igniteVersion/apache-ignite-$igniteVersion-bin.zip"
$igniteToolboxDir = "$toolboxDir/apache-ignite-$igniteVersion-bin"

if (!(Test-Path $igniteDownloadZip)) {
    Write-Verbose "build_toolbox: downloading apache ignite from $igniteDownloadUrl"
    Invoke-WebRequest -Uri $igniteDownloadUrl -OutFile $igniteDownloadZip
}

if (!(Test-Path $igniteToolboxDir)) {
    Write-Verbose "build_toolbox: unpacking apache ignite to $toolboxDir"
    Expand-Archive -Path $igniteDownloadZip -DestinationPath $toolboxDir
}

$opensslVersion = $env:OPENSSL_VERSION
if ($platform -eq "wntx64") {
    $opensslZip = "OpenSSL_${opensslVersion}-$platform-vc16.zip"
    $opensslDownloadZip = "$PWD/artifacts/$opensslZip"
    $opensslToolboxRootDir = "$toolboxPlatformDir/openssl"
    $opensslToolboxDir = "$opensslToolboxRootDir/OpenSSL_${opensslVersion}-vc16"

    if (!(Test-Path $opensslDownloadZip)) {
        Write-Error "build_toolbox: openssl version ${opensslVersion} artifact missing: $opensslDownloadZip" -ErrorAction Stop
    }

    if (!(Test-Path $opensslToolboxDir)) {
        Write-Verbose "build_toolbox: unpacking openssl to $opensslToolboxDir"
        Expand-Archive -Path $opensslDownloadZip -DestinationPath $toolboxPlatformDir
    }
}
else {
    Write-Verbose "build_toolkit: No OpenSSL kit required for non-Windows hosts"
}