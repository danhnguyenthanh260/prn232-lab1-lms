param([int]$Port = 8088)
$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
Push-Location $root
try {
    if (-not (Get-Command SqlLocalDB -ErrorAction SilentlyContinue)) { throw 'SQL Server LocalDB is required for local mode. Use Docker Compose on another environment.' }
    $instances = SqlLocalDB info
    if ($instances -notcontains 'PRN232Lab1') { SqlLocalDB create PRN232Lab1; if ($LASTEXITCODE) { throw 'LocalDB creation failed.' } }
    SqlLocalDB start PRN232Lab1
    if ($LASTEXITCODE) { throw 'LocalDB startup failed.' }
    $databaseDirectory = Join-Path $env:LOCALAPPDATA 'PRN232Lab1'
    New-Item -ItemType Directory -Path $databaseDirectory -Force | Out-Null
    $databaseFile = Join-Path $databaseDirectory 'PRN232Lms.mdf'
    $env:ConnectionStrings__Lms = "Server=(localdb)\PRN232Lab1;Integrated Security=true;Database=PRN232Lms;AttachDbFilename=$databaseFile;TrustServerCertificate=True"
    $env:Logging__LogLevel__Microsoft = 'Warning'
    dotnet run --project PRN232.LMS.API --urls "http://127.0.0.1:$Port"
} finally { Pop-Location }
