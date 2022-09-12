$vs = ConvertFrom-Json $(&'C:\Program Files (x86)\Microsoft Visual Studio\Installer\vswhere.exe' -latest -format json | Out-String)
$msBuild = Join-Path $vs.installationPath 'Msbuild/Current/Bin/MSBuild.exe'

& "$PSScriptRoot/LinkDependencies.ps1"
& $msBuild "$PSScriptRoot/Pso2Cli.sln" -property:Configuration=Release -verbosity:minimal

if (!$?) {
    exit
}

$out = "$PSScriptRoot/Release"
New-Item -ItemType Directory -Force -Path $out | Out-Null

Remove-Item "$out/*"

Copy-Item -Path "$PSScriptRoot/Aqp2Fbx/bin/Release/*" -Destination $out -Recurse
Copy-Item -Path "$PSScriptRoot/Dds2Png/bin/Release/*" -Destination $out -Recurse
Copy-Item -Path "$PSScriptRoot/Fbx2Aqp/bin/Release/*" -Destination $out -Recurse
Copy-Item -Path "$PSScriptRoot/IceCli/bin/Release/*" -Destination $out -Recurse
