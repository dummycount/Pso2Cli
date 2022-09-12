# Build
$vs = ConvertFrom-Json $(&'C:\Program Files (x86)\Microsoft Visual Studio\Installer\vswhere.exe' -latest -format json | Out-String)
$msBuild = Join-Path $vs.installationPath 'Msbuild/Current/Bin/MSBuild.exe'

& "$PSScriptRoot/LinkDependencies.ps1"
& $msBuild "$PSScriptRoot/Pso2Cli.sln" -p:RestorePackagesConfig=true -p:Configuration=Release -verbosity:minimal -restore

if (!$?) {
    exit
}

# Copy to release folder
$out = "$PSScriptRoot/Release/Pso2Cli"
New-Item -ItemType Directory -Force -Path $out | Out-Null

Remove-Item "$out/*"

Copy-Item -Path "$PSScriptRoot/Aqp2Fbx/bin/Release/*" -Destination $out -Recurse
Copy-Item -Path "$PSScriptRoot/Dds2Png/bin/Release/*" -Destination $out -Recurse
Copy-Item -Path "$PSScriptRoot/Fbx2Aqp/bin/Release/*" -Destination $out -Recurse
Copy-Item -Path "$PSScriptRoot/IceCli/bin/Release/*" -Destination $out -Recurse

# Remove empty directories
foreach ($subdir in Get-ChildItem $out -Directory) {
    if ($null -eq (Get-ChildItem $subdir)) {
        Remove-Item $subdir
    }
}

# Archive the release
Compress-Archive -Path $out -DestinationPath "$PSScriptRoot/Release/Pso2Cli.zip" -Force
