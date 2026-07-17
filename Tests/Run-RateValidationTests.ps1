$ErrorActionPreference = 'Stop'

$repo = Split-Path -Parent $PSScriptRoot
$ksp = 'C:\Program Files (x86)\Steam\steamapps\common\Kerbal Space Program'
$managed = Join-Path $ksp 'KSP_x64_Data\Managed'
$plugin = Join-Path $repo 'GameData\CustomWarp\Plugins\CustomWarp.dll'

foreach ($path in @(
    (Join-Path $managed 'UnityEngine.CoreModule.dll'),
    (Join-Path $managed 'UnityEngine.dll'),
    (Join-Path $managed 'UnityEngine.UI.dll'),
    (Join-Path $managed 'Assembly-CSharp.dll'),
    $plugin
)) {
    if (-not (Test-Path -LiteralPath $path)) {
        throw "Required test assembly is missing: $path"
    }
    [void][Reflection.Assembly]::LoadFrom($path)
}

$pluginAssembly = [AppDomain]::CurrentDomain.GetAssemblies() |
    Where-Object { $_.GetName().Name -eq 'CustomWarp' } |
    Select-Object -First 1
$type = $pluginAssembly.GetType('CustomWarp.CustomWarp', $true)
$method = $type.GetMethod(
    'TryParseRates',
    [Reflection.BindingFlags]'Static,NonPublic')
if ($null -eq $method) { throw 'TryParseRates was not found.' }

$script:passed = 0
function Assert-Equal($expected, $actual, [string]$label) {
    if ($expected -ne $actual) {
        throw "$label`: expected [$expected], got [$actual]"
    }
    $script:passed++
}

function Test-RateTable(
    [string[]]$raw,
    [bool]$expectedValid,
    [string]$errorContains,
    [string]$label
) {
    $invokeArgs = [object[]]::new(4)
    $invokeArgs[0] = $raw
    $invokeArgs[1] = 8
    $invokeArgs[2] = $null
    $invokeArgs[3] = $null
    $valid = [bool]$method.Invoke($null, $invokeArgs)

    Assert-Equal $expectedValid $valid $label
    if ($expectedValid) {
        $values = [float[]]$invokeArgs[2]
        Assert-Equal 8 $values.Length "$label value count"
        Assert-Equal 100000 $values[7] "$label final value"
    }
    else {
        $message = [string]$invokeArgs[3]
        Assert-Equal $true $message.Contains($errorContains) "$label error"
        Assert-Equal $null $invokeArgs[2] "$label leaves no partial table"
    }
}

$valid = [string[]]@('1','5','10','50','100','1000','10000','100000')
Test-RateTable $valid $true '' 'valid stock-like table'

Test-RateTable ([string[]]@('1','5','bad','50','100','1000','10000','100000')) $false 'not a valid number' 'invalid number'
Test-RateTable ([string[]]@('1','5','10','10','100','1000','10000','100000')) $false 'must be greater' 'duplicate rate'
Test-RateTable ([string[]]@('1','5','10','9','100','1000','10000','100000')) $false 'must be greater' 'descending rate'
Test-RateTable ([string[]]@('0.5','5','10','50','100','1000','10000','100000')) $false 'between 1x' 'below minimum'
Test-RateTable ([string[]]@('1','5','10','50','100','1000','10000','10000001')) $false 'between 1x' 'above maximum'
Test-RateTable ([string[]]@('1','5','10','50','100','1000','10000','NaN')) $false 'not a valid number' 'NaN rate'
Test-RateTable ([string[]]@('1','5','10','50','100','1000','10000')) $false 'Enter all' 'missing slot'

Write-Output "CustomWarp rate validation tests passed: $script:passed"
