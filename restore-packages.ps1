[CmdletBinding()]
param()

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function Get-NuGetExecutable {
    $nugetCommand = Get-Command nuget.exe -ErrorAction SilentlyContinue
    if ($nugetCommand) {
        Write-Host "Using nuget.exe from PATH: $($nugetCommand.Source)"
        return $nugetCommand.Source
    }

    $toolRoot = Join-Path $env:LOCALAPPDATA 'AutoTuningTool\tools'
    $nugetPath = Join-Path $toolRoot 'nuget.exe'

    if (-not (Test-Path $nugetPath)) {
        Write-Host 'nuget.exe was not found. Downloading a local copy...'
        New-Item -ItemType Directory -Path $toolRoot -Force | Out-Null
        Invoke-WebRequest -Uri 'https://dist.nuget.org/win-x86-commandline/latest/nuget.exe' -OutFile $nugetPath
    }
    else {
        Write-Host "Using cached nuget.exe: $nugetPath"
    }

    return $nugetPath
}

function Invoke-NuGetRestore {
    param(
        [Parameter(Mandatory = $true)]
        [string]$NuGetPath,

        [Parameter(Mandatory = $true)]
        [string]$SolutionPath
    )

    Write-Host "Restoring packages for: $SolutionPath"
    & $NuGetPath restore $SolutionPath -NonInteractive

    if ($LASTEXITCODE -ne 0) {
        throw "NuGet restore failed: $SolutionPath"
    }
}

$repoRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$solutionPaths = @(@(
    'AutoTuning_NewUI.sln',
    'FingerAutoTuning\FingerAutoTuning.sln',
    'MPPPenAutoTuning\MPPPenAutoTuning.sln'
) | ForEach-Object { Join-Path $repoRoot $_ } | Where-Object { Test-Path $_ })

if ($solutionPaths.Count -eq 0) {
    throw 'No solution files were found for package restore.'
}

$nugetPath = Get-NuGetExecutable

Write-Host ''
Write-Host 'Starting NuGet package restore...'
Write-Host ''

foreach ($solutionPath in $solutionPaths) {
    Invoke-NuGetRestore -NuGetPath $nugetPath -SolutionPath $solutionPath
}

Write-Host ''
Write-Host 'NuGet package restore completed.'