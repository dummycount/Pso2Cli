# Build
$vs = ConvertFrom-Json $(&'C:\Program Files (x86)\Microsoft Visual Studio\Installer\vswhere.exe' -latest -format json | Out-String)
$msBuild = Join-Path $vs.installationPath 'Msbuild/Current/Bin/MSBuild.exe'

& "$PSScriptRoot/LinkDependencies.ps1"
& $msBuild "$PSScriptRoot/Pso2Cli.sln" -p:RestorePackagesConfig=true -p:Configuration=Release -verbosity:minimal -restore -t:Rebuild

if (!$?) {
    exit
}

# Copy to release folder
$out = "$PSScriptRoot/Release/Pso2Cli"
New-Item -ItemType Directory -Force -Path $out | Out-Null

Remove-Item "$out/*" -Recurse -Force

Copy-Item -Path "$PSScriptRoot/Cli/bin/Release/*" -Destination $out -Recurse -Force
Copy-Item -Path "$PSScriptRoot/IceCli/bin/Release/*" -Destination $out -Recurse -Force

# Remove empty directories
foreach ($subdir in Get-ChildItem $out -Directory) {
    if ($null -eq (Get-ChildItem $subdir)) {
        Remove-Item $subdir
    }
}

# Archive the release
Compress-Archive -Path $out -DestinationPath "$PSScriptRoot/Release/Pso2Cli.zip" -Force
