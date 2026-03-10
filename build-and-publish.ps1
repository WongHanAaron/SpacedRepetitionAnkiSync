param(
    [string]$Configuration = 'Release',
    [string]$Runtime = 'win-x64'
)

# paths
$project = 'src\AnkiSync.Presentation.Console\AnkiSync.Presentation.Console.csproj'
$outSingle = 'publish\single'
$outSingleFile = 'publish\AnkiSyncSingleFile'

function Clean-Dir($dir) {
    if (Test-Path $dir) {
        Write-Host "Cleaning directory $dir";
        Remove-Item "$dir\*" -Force -Recurse -ErrorAction SilentlyContinue
    } else {
        New-Item -ItemType Directory -Path $dir | Out-Null
    }
}

# killer: stop any running instance of the executable
Get-Process -Name 'AnkiSync.Presentation.Console' -ErrorAction SilentlyContinue | Stop-Process -Force

# if a previous exe was locked, try to rename it so publish can overwrite
$locked = Join-Path $outSingleFile 'AnkiSync.Presentation.Console.exe'
if (Test-Path $locked) {
    try {
        Rename-Item -Path $locked -NewName 'AnkiSync.Presentation.Console.locked.exe' -Force -ErrorAction Stop
        Write-Host "Renamed locked executable in $outSingleFile"
    } catch {
        Write-Warning "Could not rename locked exe: $_"
    }
}

Clean-Dir $outSingle
Clean-Dir $outSingleFile

Write-Host "Publishing to $outSingle..."
& dotnet publish $project -c $Configuration -r $Runtime /p:PublishSingleFile=true /p:IncludeAllContentForSelfExtract=true /p:PublishTrimmed=false -o $outSingle

Write-Host "Publishing to $outSingleFile..."
& dotnet publish $project -c $Configuration -r $Runtime /p:PublishSingleFile=true /p:IncludeAllContentForSelfExtract=true /p:PublishTrimmed=false -o $outSingleFile

Write-Host "Publish complete. Single file outputs at:`n - $outSingle`n - $outSingleFile"