$ErrorActionPreference = 'Stop'
if ($env:OS -ne 'Windows_NT' -or [Environment]::Is64BitOperatingSystem -eq $false) { throw 'FarmHelm desktop sidecar publishing requires Windows x64.' }
$hostTriple = rustc --print host-tuple
if ($hostTriple -ne 'x86_64-pc-windows-msvc') { throw "Unsupported Rust host tuple: $hostTriple" }
$frontendRoot = Split-Path -Parent $PSScriptRoot
$repositoryRoot = Split-Path -Parent $frontendRoot
$publishDirectory = Join-Path $env:TEMP 'farmhelm-tauri-sidecar-publish'
$binariesDirectory = Join-Path $frontendRoot 'src-tauri\binaries'
Remove-Item -LiteralPath $publishDirectory -Recurse -Force -ErrorAction SilentlyContinue
New-Item -ItemType Directory -Force -Path $publishDirectory, $binariesDirectory | Out-Null
dotnet publish (Join-Path $repositoryRoot 'backend\FarmHelm.Api\FarmHelm.Api.csproj') -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o $publishDirectory
$publishedExecutable = Join-Path $publishDirectory 'FarmHelm.Api.exe'
if (-not (Test-Path -LiteralPath $publishedExecutable)) { throw 'Expected published FarmHelm.Api.exe was not produced.' }
Copy-Item -LiteralPath $publishedExecutable -Destination (Join-Path $binariesDirectory 'farmhelm-api-x86_64-pc-windows-msvc.exe') -Force
