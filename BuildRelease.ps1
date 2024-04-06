param (
	[switch] $Clean,
	[switch] $Archive
)

$target = if ($Clean) { '-t:Rebuild' } else { '-t:Build' }

. "$PSScriptRoot/MsBuild.ps1"
$msBuild = Get-MsBuild

& "$PSScriptRoot/SetupDependencies.ps1" -ReleaseOnly
& $msBuild "$PSScriptRoot/Pso2Cli.sln" -p:RestorePackagesConfig=true -p:Configuration=Release -verbosity:minimal -restore $target

if (!$?) {
	exit
}

# Copy to release folder
$out = "$PSScriptRoot/Release/Pso2Cli"

New-Item $out -ItemType Directory -ErrorAction SilentlyContinue
Remove-Item "$out/*" -Recurse -Force

Copy-Item "$PSScriptRoot/Pso2Cli/bin/Release/net8.0-windows/*" $out -Recurse -Force

if ($Archive) {
	Compress-Archive -Path $out -DestinationPath "$PSScriptRoot/Release/Pso2Cli.zip" -Force
}
