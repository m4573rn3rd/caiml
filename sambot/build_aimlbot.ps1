$ErrorActionPreference = 'Stop'

$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$sourceProject = Join-Path $root 'AIMLbot_source\AIMLbot.csproj'
$builtDll = Join-Path $root 'AIMLbot_source\bin\Release\AIMLbot.dll'
$csc = 'C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe'

if (!(Test-Path $sourceProject)) {
    throw "Could not find AIMLbot source project: $sourceProject"
}

dotnet build $sourceProject -c Release

foreach ($configuration in @('Debug', 'Release')) {
    $outDir = Join-Path $root "bin\$configuration"
    New-Item -ItemType Directory -Force -Path $outDir | Out-Null

    $targetDll = Join-Path $outDir 'AIMLbot.dll'
    $backupDll = Join-Path $outDir 'AIMLbot.original.dll'
    if ((Test-Path $targetDll) -and !(Test-Path $backupDll)) {
        Copy-Item -LiteralPath $targetDll -Destination $backupDll -Force
    }

    Copy-Item -LiteralPath $builtDll -Destination $targetDll -Force
}

foreach ($configuration in @('Debug', 'Release')) {
    $outDir = Join-Path $root "bin\$configuration"
    $outArg = '/out:' + (Join-Path $outDir 'sambot.exe')
    $references = @(
        ('/reference:' + (Join-Path $outDir 'AIMLbot.dll')),
        '/reference:System.dll',
        '/reference:System.Data.dll',
        '/reference:C:\Windows\Microsoft.NET\Framework64\v4.0.30319\WPF\System.Speech.dll',
        '/reference:System.Web.Extensions.dll',
        '/reference:System.Xml.dll'
    )
    $compilerArgs = @('/nologo', '/target:exe', $outArg) + $references + @((Join-Path $root 'Program.cs'))
    & $csc @compilerArgs
    if ($LASTEXITCODE -ne 0) {
        throw "sambot.exe build failed for $configuration with exit code $LASTEXITCODE"
    }
}

Write-Host "AIMLbot.dll and sambot.exe rebuilt successfully."
