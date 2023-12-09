
. "$PSScriptRoot/MsBuild.ps1"
$msBuild = Get-MsBuild

& "$PSScriptRoot/SetupDependencies.ps1" -ReleaseOnly
& $msBuild "$PSScriptRoot/Pso2Cli.sln" -p:RestorePackagesConfig=true -p:Configuration=Release -verbosity:minimal -restore -t:Rebuild

if (!$?) {
	exit
}

# Copy to release folder
$out = "$PSScriptRoot/Release/Pso2Cli"

New-Item $out -ItemType Directory -ErrorAction SilentlyContinue
Remove-Item "$out/*" -Recurse -Force

Copy-Item "$PSScriptRoot/Pso2Cli/bin/Release/net8.0-windows/*" $out -Recurse -Force

# Remove empty directories
# foreach ($subdir in Get-ChildItem $out -Directory) {
#     if ($null -eq (Get-ChildItem $subdir)) {
#         Remove-Item $subdir
#     }
# }

# Archive the release
Compress-Archive -Path $out -DestinationPath "$PSScriptRoot/Release/Pso2Cli.zip" -Force
