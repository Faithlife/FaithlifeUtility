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

# delete cake directory if script changed
$CakeDirPath = Join-Path $PSScriptRoot "cake"
$PackagesConfigPath = Join-Path $CakeDirPath "packages.config"
If ((Test-Path $PackagesConfigPath) -and ((Get-Item(Join-Path $PSScriptRoot "build.cake")).LastWriteTime -gt (Get-Item($PackagesConfigPath)).LastWriteTime)) {
    Write-Host "Cake script changed; rebuilding cake directory."
    Remove-Item $CakeDirPath -Force -Recurse
}

# create cake directory
New-Item -Path $CakeDirPath -Type Directory -ErrorAction SilentlyContinue | Out-Null

# create packages.config
If (!(Test-Path $PackagesConfigPath)) {
    [System.IO.File]::WriteAllLines($PackagesConfigPath, @(
        "<?xml version=`"1.0`" encoding=`"utf-8`"?>",
        "<packages>",
        "`t<package id=`"Cake`" version=`"0.22.2`" />",
        "</packages>"))
}

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

# use NuGet to download Cake
Push-Location $CakeDirPath
Invoke-Expression "&`"$NuGetExePath`" install -ExcludeVersion -OutputDirectory ."
If ($LASTEXITCODE -ne 0) {
    Throw "An error occured while restoring NuGet tools."
}
Pop-Location

# run Cake with specified arguments
$CakeExePath = Join-Path $CakeDirPath "Cake/Cake.exe"
$ExtraArgs = ""
if ($Target) { $ExtraArgs += "--target=$Target" }
Invoke-Expression "& `"$CakeExePath`" --paths_tools=cake --experimental $ExtraArgs $ScriptArgs"
Exit $LASTEXITCODE
