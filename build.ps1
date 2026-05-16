$ErrorActionPreference = "Stop"

$csc = Join-Path $env:WINDIR "Microsoft.NET\Framework64\v4.0.30319\csc.exe"
if (!(Test-Path $csc)) {
    throw "C# compiler not found at $csc"
}

$outDir = Join-Path $PSScriptRoot "dist"
New-Item -ItemType Directory -Force -Path $outDir | Out-Null

& $csc `
    /nologo `
    /target:winexe `
    /platform:x64 `
    /optimize+ `
    /win32manifest:"$PSScriptRoot\app.manifest" `
    /reference:System.dll `
    /reference:System.Core.dll `
    /reference:System.Drawing.dll `
    /reference:System.Windows.Forms.dll `
    /out:"$outDir\FortniteCinematicSettings.exe" `
    "$PSScriptRoot\FortniteCinematicSettings.cs"

if ($LASTEXITCODE -ne 0) {
    throw "Build failed with exit code $LASTEXITCODE"
}

Write-Host "Built $outDir\FortniteCinematicSettings.exe"
