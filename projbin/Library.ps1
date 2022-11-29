# Central library of PS functions
# TODO This should be in a real Module

function Build-Environment {
    [CmdletBinding()]
    param (
        [Parameter(Mandatory)]
        [string[]]$Environment
    )

    foreach ($line in $Environment) {
        if ([string]::IsNullOrWhiteSpace($line)) {
            continue
        }

        # ignore comments
        if ($line.StartsWith("#")) {
            Write-Debug "Build-Environment: Skipping comment: $line"
            continue
        }

        # strip leading "export " statement
        $line = $line -replace "^export +", ""

        $kvp = $line -split "=", 2
        $key = $kvp[0].Trim()
        $value = $kvp[1].Trim()
        if (!($value -like "`"*`"")) {
            $value = "`"$value`""
        }
        Write-Debug "Key: '${key}', Value: '${value}'"
        Invoke-Expression "`$newValue = $value"
        Write-Debug "Set newvalue: '$newValue'"
        Invoke-Expression "`$${key} = `"${newValue}`""
        Write-Debug "Set ${key}: '$newValue'"
        [Environment]::SetEnvironmentVariable($key, $newValue, "Process") | Out-Null
    }
}

function Use-PsEnv {
    [CmdletBinding()]
    param (
        [string]$EnvFile = "./.env"
    )
    if (!(Test-Path $EnvFile)) {
        Write-Error "Use-PsEnv: No .env file!" -ErrorAction Stop
    }

    $content = Get-Content $EnvFile -ErrorAction Stop
    Build-Environment -Environment $content

    $platform = Get-Platform
    $platformEnvFile = "${EnvFile}.$platform"
    if (Test-Path $platformEnvFile) {
        $content = Get-Content $platformEnvFile -ErrorAction Stop
        Build-Environment -Environment $content
    }

    if ($platform -eq "lnx64") {
        Write-Debug "Set TEMP: '/tmp'"
        [Environment]::SetEnvironmentVariable("TEMP", "/tmp", "Process") | Out-Null
    }
}

function Get-IgniteHome {
    [CmdletBinding()]
    [OutputType([string])]
    param (
    )

    $igniteHome = "$PWD/$env:TOOLBOX_DIR/apache-ignite-$env:IGNITE_VERSION-bin"
    return $igniteHome
}

function Get-Platform {
    [CmdletBinding()]
    [OutputType([String])]
    param (
    )
    if ($PSVersionTable.Platform -eq "Win32NT") {
        return "wntx64"
    }
    elseif ($PSVersionTable.Platform -eq "Unix") {
        return "lnx64"
    }
    else {
        Write-Error "Get-Platform: Unknown platform: ${PSVersionTable.Platform}" -ErrorAction Stop
    }
}
