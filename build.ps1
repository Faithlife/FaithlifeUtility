<#
.SYNOPSIS
This is a Powershell script to bootstrap a Cake build.

.PARAMETER Target
The build script target to run.
#>

[CmdletBinding()]
Param(
    [string]$Target,
    [Parameter(Position=0,Mandatory=$false,ValueFromRemainingArguments=$true)]
    [string[]]$ScriptArgs
)

$CakeVersion = "0.30.0"

# create cake directory
$CakeDirPath = Join-Path $PSScriptRoot "cake"
New-Item -Path $CakeDirPath -Type Directory -ErrorAction SilentlyContinue | Out-Null

# download nuget.exe if not in path and not already downloaded
$NuGetExe = Get-Command "nuget.exe" -ErrorAction SilentlyContinue
If ($NuGetExe -ne $null) {
    $NuGetExePath = $NuGetExe.Path
}
Else {
    $NuGetExePath = Join-Path $CakeDirPath "nuget.exe"
    If (!(Test-Path $NuGetExePath)) {
        Invoke-WebRequest -Uri http://dist.nuget.org/win-x86-commandline/latest/nuget.exe -OutFile $NuGetExePath
    }
}

# download Cake if necessary
$CakeExePath = Join-Path $CakeDirPath "Cake.$CakeVersion/Cake.exe"
If (!(Test-Path $CakeExePath)) {
    # create packages.config
    $PackagesConfigPath = Join-Path $CakeDirPath "packages.config"
    [System.IO.File]::WriteAllLines($PackagesConfigPath, @(
        "<?xml version=`"1.0`" encoding=`"utf-8`"?>",
        "<packages>",
        "`t<package id=`"Cake`" version=`"$CakeVersion`" />",
        "</packages>"))

    # use NuGet to download Cake
    Push-Location $CakeDirPath
    Invoke-Expression "&`"$NuGetExePath`" install -OutputDirectory ."
    If ($LASTEXITCODE -ne 0) {
        Throw "An error occurred while restoring NuGet tools."
    }
    Pop-Location
}

# run Cake with specified arguments
$ExtraArgs = ""
if ($Target) { $ExtraArgs += "--target=$Target" }
Invoke-Expression "& `"$CakeExePath`" --paths_tools=cake $ExtraArgs $ScriptArgs"
Exit $LASTEXITCODE
