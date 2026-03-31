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
        throw '32-bit PowerShell was not found. MPP Excel smoke test requires an x86 host to load MPPPenAutoTuning.dll.'
    }

    $argumentList = @(
        '-NoProfile',
        '-ExecutionPolicy', 'Bypass',
        '-STA',
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

    Write-Host 'Building MPPPenAutoTuning.sln (Release|x86)...'
    Push-Location $RepositoryRoot

    try {
        & dotnet msbuild 'MPPPenAutoTuning\MPPPenAutoTuning.sln' '/t:Build' '/p:Configuration=Release' '/p:Platform=x86' '/m'

        if ($LASTEXITCODE -ne 0) {
            throw 'dotnet msbuild failed.'
        }
    }
    finally {
        Pop-Location
    }
}

function Set-PrivateField {
    param(
        [object]$Instance,
        [Type]$Type,
        [string]$FieldName,
        [object]$Value,
        [System.Reflection.BindingFlags]$Flags
    )

    $fieldInfo = $Type.GetField($FieldName, $Flags)

    if ($null -eq $fieldInfo) {
        throw "Field was not found: $FieldName"
    }

    $fieldInfo.SetValue($Instance, $Value)
}

function Get-PrivateField {
    param(
        [object]$Instance,
        [Type]$Type,
        [string]$FieldName,
        [System.Reflection.BindingFlags]$Flags
    )

    $fieldInfo = $Type.GetField($FieldName, $Flags)

    if ($null -eq $fieldInfo) {
        throw "Field was not found: $FieldName"
    }

    return $fieldInfo.GetValue($Instance)
}

function Invoke-PrivateMethod {
    param(
        [object]$Instance,
        [Type]$Type,
        [string]$MethodName,
        [object[]]$Arguments,
        [System.Reflection.BindingFlags]$Flags
    )

    $methodInfo = $Type.GetMethod($MethodName, $Flags)

    if ($null -eq $methodInfo) {
        throw "Method was not found: $MethodName"
    }

    return $methodInfo.Invoke($Instance, $Arguments)
}

function Add-Result {
    param(
        [System.Collections.Generic.List[object]]$Results,
        [string]$CaseName,
        [bool]$Passed,
        [string]$Expected,
        [string]$Actual
    )

    $Results.Add([pscustomobject]@{
        CaseName = $CaseName
        Passed = $Passed
        Expected = $Expected
        Actual = $Actual
    }) | Out-Null
}

function Get-WorksheetName {
    param(
        [string]$FilePath
    )

    [OfficeOpenXml.ExcelPackage]::LicenseContext = [OfficeOpenXml.LicenseContext]::NonCommercial
    $package = New-Object OfficeOpenXml.ExcelPackage([System.IO.FileInfo]::new($FilePath))

    try {
        return $package.Workbook.Worksheets[0].Name
    }
    finally {
        $package.Dispose()
    }
}

Restart-InX86PowerShell -ScriptPath $MyInvocation.MyCommand.Path -SkipBuild:$SkipBuild -KeepArtifacts:$KeepArtifacts

$repoRoot = Split-Path -Parent $MyInvocation.MyCommand.Path

if (-not $SkipBuild) {
    Invoke-ReleaseBuild -RepositoryRoot $repoRoot
}

$releaseDirectory = Join-Path $repoRoot 'MPPPenAutoTuning\MPPPenAutoTuning\bin\Release'
$mppAssemblyPath = Join-Path $releaseDirectory 'MPPPenAutoTuning.dll'

if ((Test-Path $mppAssemblyPath) -eq $false) {
    throw "MPPPenAutoTuning.dll was not found: $mppAssemblyPath"
}

$assembly = [System.Reflection.Assembly]::LoadFrom($mppAssemblyPath)
$formType = $assembly.GetType('MPPPenAutoTuning.frmSummarizeLogData')

if ($null -eq $formType) {
    throw 'Type was not found: MPPPenAutoTuning.frmSummarizeLogData'
}

$instanceFlags = [System.Reflection.BindingFlags]::Instance -bor [System.Reflection.BindingFlags]::NonPublic
$staticFlags = [System.Reflection.BindingFlags]::Static -bor [System.Reflection.BindingFlags]::NonPublic
$nestedFlags = [System.Reflection.BindingFlags]::NonPublic

$artifactRoot = Join-Path $repoRoot 'tmp_mpp_excel_smoke'

if (Test-Path $artifactRoot) {
    Remove-Item -Path $artifactRoot -Recurse -Force
}

New-Item -ItemType Directory -Path $artifactRoot | Out-Null

$form = [Activator]::CreateInstance($formType)
$checkBox = Get-PrivateField -Instance $form -Type $formType -FieldName 'ckbxOutputFolderName' -Flags $instanceFlags
$textBox = Get-PrivateField -Instance $form -Type $formType -FieldName 'tbxOutputFolderName' -Flags $instanceFlags
$results = New-Object 'System.Collections.Generic.List[object]'

Set-PrivateField -Instance $form -Type $formType -FieldName 'm_sDefaultOutputFolderPath' -Value $artifactRoot -Flags $instanceFlags
Set-PrivateField -Instance $form -Type $formType -FieldName 'm_sDirectoryName' -Value 'ELAN_SKU_A1_extra_tag' -Flags $instanceFlags

$checkBox.Checked = $false
$expectedAutoFolder = Join-Path $artifactRoot 'output_ELAN_SKU_A1'
$actualAutoFolder = [string](Invoke-PrivateMethod -Instance $form -Type $formType -MethodName 'EnsureSummaryOutputFolderPath' -Arguments @() -Flags $instanceFlags)
Add-Result -Results $results -CaseName 'AutoOutputFolderName' -Passed ($expectedAutoFolder -eq $actualAutoFolder) -Expected $expectedAutoFolder -Actual $actualAutoFolder

$expectedLogSummaryPath = Join-Path $expectedAutoFolder 'Log Summary.xlsx'
$actualLogSummaryPath = [string](Invoke-PrivateMethod -Instance $form -Type $formType -MethodName 'GetSummaryOutputFilePath' -Arguments @('Log Summary.xlsx') -Flags $instanceFlags)
Add-Result -Results $results -CaseName 'LogSummaryPath' -Passed ($expectedLogSummaryPath -eq $actualLogSummaryPath) -Expected $expectedLogSummaryPath -Actual $actualLogSummaryPath

$summaryDataTable = New-Object System.Data.DataTable 'Summary'
[void]$summaryDataTable.Columns.Add('Metric', [string])
[void]$summaryDataTable.Columns.Add('Value', [int])
$summaryRow = $summaryDataTable.NewRow()
$summaryRow['Metric'] = 'Beacon'
$summaryRow['Value'] = 10
[void]$summaryDataTable.Rows.Add($summaryRow)

$null = Invoke-PrivateMethod -Instance $form -Type $formType -MethodName 'ExportToExcel' -Arguments @([System.Data.DataTable]$summaryDataTable, [string]$expectedLogSummaryPath, [int]0, [int]0, [bool]$false, $null) -Flags $instanceFlags
$null = Invoke-PrivateMethod -Instance $form -Type $formType -MethodName 'SetTitleAndColumnFormatToExcel' -Arguments @([string]$expectedLogSummaryPath, [string]'SKU-1', [int]0, [int]0) -Flags $instanceFlags
$logSummarySheetName = Get-WorksheetName -FilePath $expectedLogSummaryPath
Add-Result -Results $results -CaseName 'LogSummaryExport' -Passed ((Test-Path $expectedLogSummaryPath) -and ($logSummarySheetName -eq 'summary')) -Expected "$expectedLogSummaryPath | summary" -Actual "$expectedLogSummaryPath | $logSummarySheetName"

$dataType = $formType.GetNestedType('DataType', $nestedFlags)

if ($null -eq $dataType) {
    throw 'Nested enum was not found: DataType'
}

$allDataEnum = [Enum]::Parse($dataType, 'AllData')
$allModifyDataEnum = [Enum]::Parse($dataType, 'AllModifyData')

$allDataPath = [string](Invoke-PrivateMethod -Instance $form -Type $formType -MethodName 'GetSummaryOutputFilePath' -Arguments @('df_all.xlsx') -Flags $instanceFlags)
$allDataTable = New-Object System.Data.DataTable 'All Table'
[void]$allDataTable.Columns.Add('Name', [string])
[void]$allDataTable.Columns.Add('Value', [double])
$allDataRow = $allDataTable.NewRow()
$allDataRow['Name'] = 'CaseA'
$allDataRow['Value'] = 12.5
[void]$allDataTable.Rows.Add($allDataRow)

$null = Invoke-PrivateMethod -Instance $form -Type $formType -MethodName 'ExportAllDataToExcel' -Arguments @([System.Data.DataTable]$allDataTable, [string]$allDataPath, $allDataEnum) -Flags $instanceFlags
$allDataSheetName = Get-WorksheetName -FilePath $allDataPath
Add-Result -Results $results -CaseName 'AllDataExport' -Passed ((Test-Path $allDataPath) -and ($allDataSheetName -eq 'Sheet1')) -Expected "$allDataPath | Sheet1" -Actual "$allDataPath | $allDataSheetName"

$allModifyPath = [string](Invoke-PrivateMethod -Instance $form -Type $formType -MethodName 'GetSummaryOutputFilePath' -Arguments @('df_all_M.xlsx') -Flags $instanceFlags)
$null = Invoke-PrivateMethod -Instance $form -Type $formType -MethodName 'ExportAllDataToExcel' -Arguments @([System.Data.DataTable]$allDataTable, [string]$allModifyPath, $allModifyDataEnum) -Flags $instanceFlags
$allModifySheetName = Get-WorksheetName -FilePath $allModifyPath
Add-Result -Results $results -CaseName 'AllModifyExport' -Passed ((Test-Path $allModifyPath) -and ($allModifySheetName -eq 'Sheet1')) -Expected "$allModifyPath | Sheet1" -Actual "$allModifyPath | $allModifySheetName"

$checkBox.Checked = $true
$textBox.Text = 'custom_summary_output'
$expectedCustomFolder = Join-Path $artifactRoot 'custom_summary_output'
$actualCustomFolder = [string](Invoke-PrivateMethod -Instance $form -Type $formType -MethodName 'EnsureSummaryOutputFolderPath' -Arguments @() -Flags $instanceFlags)
Add-Result -Results $results -CaseName 'CustomOutputFolderName' -Passed ($expectedCustomFolder -eq $actualCustomFolder) -Expected $expectedCustomFolder -Actual $actualCustomFolder

$results | Format-Table -AutoSize

$failedResults = @($results | Where-Object { $_.Passed -eq $false })

if ($failedResults.Count -gt 0) {
    throw ('MPP Excel smoke test failed for: ' + (($failedResults | Select-Object -ExpandProperty CaseName) -join ', '))
}

Write-Host 'All MPP Excel smoke test cases passed.' -ForegroundColor Green

if (-not $KeepArtifacts) {
    Remove-Item -Path $artifactRoot -Recurse -Force
}