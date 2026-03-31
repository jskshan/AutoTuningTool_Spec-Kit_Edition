[CmdletBinding()]
param(
    [switch]$SkipBuild,
    [switch]$KeepArtifacts
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function Restart-InX86PowerShell {
    param(
        [string]$ScriptPath,
        [switch]$SkipBuild,
        [switch]$KeepArtifacts
    )

    if ([Environment]::Is64BitProcess -eq $false) {
        return
    }

    $x86PowerShellPath = Join-Path $env:WINDIR 'SysWOW64\WindowsPowerShell\v1.0\powershell.exe'

    if ((Test-Path $x86PowerShellPath) -eq $false) {
        throw '32-bit PowerShell was not found. DataSave smoke test requires an x86 host to load FingerAutoTuning.dll.'
    }

    $argumentList = @(
        '-NoProfile',
        '-ExecutionPolicy', 'Bypass',
        '-File', $ScriptPath
    )

    if ($SkipBuild) {
        $argumentList += '-SkipBuild'
    }

    if ($KeepArtifacts) {
        $argumentList += '-KeepArtifacts'
    }

    Write-Host 'Restarting smoke test in 32-bit PowerShell...'
    & $x86PowerShellPath @argumentList
    exit $LASTEXITCODE
}

function Invoke-ReleaseBuild {
    param(
        [string]$RepositoryRoot
    )

    Write-Host 'Building AutoTuning_NewUI.sln (Release|x86)...'
    Push-Location $RepositoryRoot

    try {
        & dotnet msbuild 'AutoTuning_NewUI.sln' '/t:Build' '/p:Configuration=Release' '/p:Platform=x86' '/m'

        if ($LASTEXITCODE -ne 0) {
            throw 'dotnet msbuild failed.'
        }
    }
    finally {
        Pop-Location
    }
}

function New-SaveDataInfo {
    New-Object FingerAutoTuning.SaveDataInfo
}

function Get-NormalReportFilePath {
    param(
        [string]$LogRoot,
        [string]$DataType,
        [int]$PH1,
        [int]$PH2,
        [int]$RepeatIndex,
        [bool]$SetRepeatIndex
    )

    $frequency = [FingerAutoTuning.ElanConvert]::Convert2Frequency($PH1, $PH2).ToString('0.000')

    if ($SetRepeatIndex) {
        return Join-Path $LogRoot ("{0}\Report_{1}_{2}_{3}_{4}.txt" -f $DataType, $frequency, $PH1.ToString('x2').ToUpper(), $PH2.ToString('x2').ToUpper(), $RepeatIndex)
    }

    return Join-Path $LogRoot ("{0}\Report_{1}_{2}_{3}.txt" -f $DataType, $frequency, $PH1.ToString('x2').ToUpper(), $PH2.ToString('x2').ToUpper())
}

function Get-NormalFrameFilePath {
    param(
        [string]$LogRoot,
        [string]$DataType,
        [int]$PH1,
        [int]$PH2
    )

    $frequency = [FingerAutoTuning.ElanConvert]::Convert2Frequency($PH1, $PH2).ToString('0.000')
    return Join-Path $LogRoot ("{0}\{0}_{1}_{2}_{3}.csv" -f $DataType, $frequency, $PH1.ToString('x2').ToUpper(), $PH2.ToString('x2').ToUpper())
}

function Get-SelfPh2Sum {
    param(
        [int]$SelfPH2ELat,
        [int]$SelfPH2ELmt,
        [int]$SelfPH2Lat,
        [int]$SelfPH2
    )

    return [FingerAutoTuning.ElanConvert]::Convert2SelfPH2SumInt($SelfPH2ELat, $SelfPH2ELmt, $SelfPH2Lat, $SelfPH2)
}

function Get-SelfReportFilePath {
    param(
        [string]$LogRoot,
        [string]$DataType,
        [FingerAutoTuning.SaveDataInfo]$Info,
        [int]$RepeatIndex,
        [bool]$SetRepeatIndex,
        [bool]$IncludeKSequence
    )

    $selfPh2Sum = Get-SelfPh2Sum -SelfPH2ELat $Info.m_n_SELF_PH2E_LAT -SelfPH2ELmt $Info.m_n_SELF_PH2E_LMT -SelfPH2Lat $Info.m_n_SELF_PH2_LAT -SelfPH2 $Info.m_n_SELF_PH2
    $frequency = [FingerAutoTuning.ElanConvert]::Convert2Frequency($Info.m_n_SELF_PH1, $selfPh2Sum).ToString('0.000')

    if ($SetRepeatIndex) {
        $fileName = 'Report_{0}_{1}_{2}_{3}_{4}_{5}_{6}' -f $frequency, $Info.m_n_SELF_PH1.ToString('x2').ToUpper(), $Info.m_n_SELF_PH2E_LMT.ToString('x2').ToUpper(), $selfPh2Sum.ToString('x2').ToUpper(), $Info.m_nSelf_DFT_NUM.ToString('x2').ToUpper(), $Info.m_sSelfTraceType, $RepeatIndex
    }
    else {
        $fileName = 'Report_{0}_{1}_{2}_{3}_{4}_{5}' -f $frequency, $Info.m_n_SELF_PH1.ToString('x2').ToUpper(), $Info.m_n_SELF_PH2E_LMT.ToString('x2').ToUpper(), $selfPh2Sum.ToString('x2').ToUpper(), $Info.m_nSelf_DFT_NUM.ToString('x2').ToUpper(), $Info.m_sSelfTraceType
    }

    if ($IncludeKSequence -and $Info.m_bSetSelfKSequence) {
        $fileName = '{0}_P{1:00}N{2:00}' -f $fileName, $Info.m_nSelfNCPValue, $Info.m_nSelfNCNValue
    }

    return Join-Path $LogRoot ("{0}\{1}.txt" -f $DataType, $fileName)
}

function Get-SelfFrameFilePath {
    param(
        [string]$LogRoot,
        [string]$DataType,
        [FingerAutoTuning.SaveDataInfo]$Info
    )

    $selfPh2Sum = Get-SelfPh2Sum -SelfPH2ELat $Info.m_n_SELF_PH2E_LAT -SelfPH2ELmt $Info.m_n_SELF_PH2E_LMT -SelfPH2Lat $Info.m_n_SELF_PH2_LAT -SelfPH2 $Info.m_n_SELF_PH2
    $frequency = [FingerAutoTuning.ElanConvert]::Convert2Frequency($Info.m_n_SELF_PH1, $selfPh2Sum).ToString('0.000')
    $fileName = '{0}_{1}_{2}_{3}_{4}_{5}_{6}' -f $DataType, $frequency, $Info.m_n_SELF_PH1.ToString('x2').ToUpper(), $Info.m_n_SELF_PH2E_LMT.ToString('x2').ToUpper(), $selfPh2Sum.ToString('x2').ToUpper(), $Info.m_sSelfTraceType, $Info.m_nSelf_DFT_NUM.ToString('x4').ToUpper()

    if ($Info.m_bSetSelfKSequence) {
        $fileName = '{0}_P{1:00}N{2:00}' -f $fileName, $Info.m_nSelfNCPValue, $Info.m_nSelfNCNValue
    }

    return Join-Path $LogRoot ("{0}\{1}.csv" -f $DataType, $fileName)
}

function Add-Result {
    param(
        [System.Collections.Generic.List[object]]$Results,
        [string]$CaseName,
        [string]$ExpectedPath,
        [string]$ActualPath
    )

    $Results.Add([pscustomobject]@{
        CaseName = $CaseName
        Passed = ($ExpectedPath -eq $ActualPath)
        ExpectedPath = $ExpectedPath
        ActualPath = $ActualPath
    }) | Out-Null
}

Restart-InX86PowerShell -ScriptPath $MyInvocation.MyCommand.Path -SkipBuild:$SkipBuild -KeepArtifacts:$KeepArtifacts

$repoRoot = Split-Path -Parent $MyInvocation.MyCommand.Path

if (-not $SkipBuild) {
    Invoke-ReleaseBuild -RepositoryRoot $repoRoot
}

$releaseDirectory = Join-Path $repoRoot 'FingerAutoTuning\FingerAutoTuning\bin\Release'
$fingerAssemblyPath = Join-Path $releaseDirectory 'FingerAutoTuning.dll'

if ((Test-Path $fingerAssemblyPath) -eq $false) {
    throw "FingerAutoTuning.dll was not found: $fingerAssemblyPath"
}

[System.Reflection.Assembly]::LoadFrom($fingerAssemblyPath) | Out-Null

$frmMain = [System.Runtime.Serialization.FormatterServices]::GetUninitializedObject([FingerAutoTuning.frmMain])
$frmMain.m_sParentAPVersion = 'SmokeParent'
$frmMain.m_sAPVersion = 'SmokeFinger'

$saveData = New-Object FingerAutoTuning.SaveData($frmMain, 'SmokeProject')
$artifactRoot = Join-Path $repoRoot 'tmp_data_export_smoke'

if (Test-Path $artifactRoot) {
    Remove-Item -Path $artifactRoot -Recurse -Force
}

New-Item -ItemType Directory -Path $artifactRoot | Out-Null

$results = New-Object 'System.Collections.Generic.List[object]'

$normalLogRoot = Join-Path $artifactRoot 'normal'
New-Item -ItemType Directory -Path $normalLogRoot | Out-Null

$normalInfo = New-SaveDataInfo
$normalInfo.m_sLogDirectoryPath = $normalLogRoot
$normalInfo.m_nPH1 = 0x1A
$normalInfo.m_nPH2 = 0x2B
$normalInfo.m_nPH3 = 0x3C
$normalInfo.m_nDFT_NUM = 0x10
$normalInfo.m_nReadPH1 = 0x1A
$normalInfo.m_nReadPH2 = 0x2B
$normalInfo.m_nReadPH3 = 0x3C
$normalInfo.m_nReadDFT_NUM = 0x10
$normalInfo.m_nTXTraceNumber = 1
$normalInfo.m_nRXTraceNumber = 1
$normalInfo.m_nListIndex = 7
$normalInfo.m_nFrameNumber = 1

$null = $saveData.CreateRecordData($normalInfo, $normalLogRoot, 'RawData', 0, $false, $false, $true)
$saveData.CloseRecordData()
Add-Result -Results $results -CaseName 'CreateRecordData_Normal' -ExpectedPath (Get-NormalReportFilePath -LogRoot $normalLogRoot -DataType 'RawData' -PH1 $normalInfo.m_nPH1 -PH2 $normalInfo.m_nPH2 -RepeatIndex 0 -SetRepeatIndex $false) -ActualPath $saveData.DataFilePath

$frame2D = New-Object 'System.Int32[,]' 1,1
$frame2D[0,0] = 123
$null = $saveData.SaveFrameDataByFile($normalInfo, 'NormalFrame', $frame2D, 'Header1,Header2')
$saveFrameDataByFilePath = Get-NormalFrameFilePath -LogRoot $normalLogRoot -DataType 'NormalFrame' -PH1 $normalInfo.m_nPH1 -PH2 $normalInfo.m_nPH2
$saveFrameDataByFileActualPath = $saveData.DataFilePath

if (Test-Path $saveFrameDataByFilePath) {
    $saveFrameDataByFileActualPath = $saveFrameDataByFilePath
}

Add-Result -Results $results -CaseName 'SaveFrameDataByFile_Normal' -ExpectedPath $saveFrameDataByFilePath -ActualPath $saveFrameDataByFileActualPath

$frame3D = New-Object 'System.Int32[,,]' 1,2,2
$frame3D[0,1,1] = 456
$null = $saveData.SaveFrameData($normalInfo, 'FrameData', $frame3D, [FingerAutoTuning.ICGenerationType]::Gen7, [FingerAutoTuning.ICSolutionType]::Solution_8F09, $false)
Add-Result -Results $results -CaseName 'SaveFrameData_Normal' -ExpectedPath (Get-NormalFrameFilePath -LogRoot $normalLogRoot -DataType 'FrameData' -PH1 $normalInfo.m_nPH1 -PH2 $normalInfo.m_nPH2) -ActualPath $saveData.DataFilePath

$selfLogRoot = Join-Path $artifactRoot 'self'
New-Item -ItemType Directory -Path $selfLogRoot | Out-Null

$selfInfo = New-SaveDataInfo
$selfInfo.m_sLogDirectoryPath = $selfLogRoot
$selfInfo.m_bGetSelf = $true
$selfInfo.m_bSetSelfKSequence = $true
$selfInfo.m_sSelfTraceType = 'Odd'
$selfInfo.m_sSelfTracePart = 'Odd'
$selfInfo.m_n_SELF_PH1 = 0x12
$selfInfo.m_n_SELF_PH2E_LAT = 0x01
$selfInfo.m_n_SELF_PH2E_LMT = 0x34
$selfInfo.m_n_SELF_PH2_LAT = 0x02
$selfInfo.m_n_SELF_PH2 = 0x56
$selfInfo.m_nSelf_DFT_NUM = 0x78
$selfInfo.m_nTXTraceNumber = 1
$selfInfo.m_nRXTraceNumber = 1
$selfInfo.m_nRead_SELF_PH1 = 0x12
$selfInfo.m_nRead_SELF_PH2E_LAT = 0x01
$selfInfo.m_nRead_SELF_PH2E_LMT = 0x34
$selfInfo.m_nRead_SELF_PH2_LAT = 0x02
$selfInfo.m_nRead_SELF_PH2 = 0x56
$selfInfo.m_nReadSelf_DFT_NUM = 0x78
$selfInfo.m_nSelf_Gain = 1
$selfInfo.m_nSelf_CAG = 2
$selfInfo.m_nSelf_IQ_BSH = 3
$selfInfo.m_nReadSelf_Gain = 1
$selfInfo.m_nReadSelf_CAG = 2
$selfInfo.m_nReadSelf_IQ_BSH = 3
$selfInfo.m_nSelfNCPValue = 4
$selfInfo.m_nSelfNCNValue = 5
$selfInfo.m_nSelfCALValue = 6
$selfInfo.m_dSelf_SampleTime = 1.5
$selfInfo.m_nFrameNumber = 1

$null = $saveData.CreateRecordData($selfInfo, $selfLogRoot, 'SelfReport', 3, $false, $true, $true)
$saveData.CloseRecordData()
Add-Result -Results $results -CaseName 'CreateRecordData_SelfWithKSequence' -ExpectedPath (Get-SelfReportFilePath -LogRoot $selfLogRoot -DataType 'SelfReport' -Info $selfInfo -RepeatIndex 3 -SetRepeatIndex $true -IncludeKSequence $true) -ActualPath $saveData.DataFilePath

$selfFrame3D = New-Object 'System.Int32[,,]' 1,3,3
$selfFrame3D[0,1,1] = 789
$null = $saveData.SaveFrameData($selfInfo, 'SelfFrame', $selfFrame3D, [FingerAutoTuning.ICGenerationType]::Gen7, [FingerAutoTuning.ICSolutionType]::Solution_8F09, $false)
Add-Result -Results $results -CaseName 'SaveFrameData_SelfWithKSequence' -ExpectedPath (Get-SelfFrameFilePath -LogRoot $selfLogRoot -DataType 'SelfFrame' -Info $selfInfo) -ActualPath $saveData.DataFilePath

$reportData = New-Object 'System.Collections.Generic.List[byte[]]'
$reportData.Add([byte[]](0x01, 0x02, 0x03)) | Out-Null
$null = $saveData.SaveReportData($selfInfo, 'SelfReportData', $reportData, 3, $false, $true, $true)
Add-Result -Results $results -CaseName 'SaveReportData_SelfWithoutKSequence' -ExpectedPath (Get-SelfReportFilePath -LogRoot $selfLogRoot -DataType 'SelfReportData' -Info $selfInfo -RepeatIndex 3 -SetRepeatIndex $true -IncludeKSequence $false) -ActualPath $saveData.DataFilePath

$rawLogRoot = Join-Path $artifactRoot 'rawadcs'
New-Item -ItemType Directory -Path $rawLogRoot | Out-Null

$rawInfo = New-SaveDataInfo
$rawInfo.m_sLogDirectoryPath = $rawLogRoot
$rawInfo.m_bRawADCSweep = $true
$rawInfo.m_nSELC = 7
$rawInfo.m_nVSEL = 8
$rawInfo.m_nLG = 9
$rawInfo.m_nSELGM = 10
$rawInfo.m_nFIR_TAP_NUM = 1
$rawInfo.m_nFIRTB = 2
$rawInfo.m_nReadFIR_TAP_NUM = 1
$rawInfo.m_nReadFIRTB = 2
$rawInfo.m_nReadSELC = 7
$rawInfo.m_nReadVSEL = 8
$rawInfo.m_nReadLG = 9
$rawInfo.m_nReadSELGM = 10
$rawInfo.m_nDFT_NUM = 0x11
$rawInfo.m_nIQ_BSH_0 = 0x22
$rawInfo.m_nTXTraceNumber = 1
$rawInfo.m_nRXTraceNumber = 1
$rawInfo.m_nFrameNumber = 1

$rawFrame3D = New-Object 'System.Int32[,,]' 1,2,2
$rawFrame3D[0,1,1] = 99
$null = $saveData.SaveFrameData($rawInfo, 'RawAdcsFrame', $rawFrame3D, [FingerAutoTuning.ICGenerationType]::Gen7, [FingerAutoTuning.ICSolutionType]::Solution_8F09, $false)
Add-Result -Results $results -CaseName 'SaveFrameData_RawAdcsSweep' -ExpectedPath (Join-Path $rawLogRoot 'RawAdcsFrame\RawAdcsFrame_7_8_9_10.csv') -ActualPath $saveData.DataFilePath

Write-Host ''
Write-Host 'DataSave smoke test results:'
$results | Format-Table CaseName, Passed -AutoSize

$failedResults = @($results | Where-Object { $_.Passed -eq $false })

if ($failedResults.Count -gt 0) {
    Write-Host ''
    Write-Host 'Failed cases:' -ForegroundColor Red
    $failedResults | Format-List CaseName, ExpectedPath, ActualPath

    if (-not $KeepArtifacts -and (Test-Path $artifactRoot)) {
        Remove-Item -Path $artifactRoot -Recurse -Force
    }

    throw 'DataSave smoke test failed.'
}

Write-Host ''
Write-Host 'All DataSave smoke test cases passed.' -ForegroundColor Green

if ($KeepArtifacts) {
    Write-Host "Artifacts kept at: $artifactRoot"
}
elseif (Test-Path $artifactRoot) {
    Remove-Item -Path $artifactRoot -Recurse -Force
}