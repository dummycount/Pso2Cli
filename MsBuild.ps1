
function Get-MsBuild {
	$vs = ConvertFrom-Json $(&'C:\Program Files (x86)\Microsoft Visual Studio\Installer\vswhere.exe' -latest -format json | Out-String)
	return Join-Path $vs.installationPath 'Msbuild/Current/Bin/MSBuild.exe'
}