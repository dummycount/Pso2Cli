function New-Junction {
    param (
        [string] $Path,
        [string] $Target
    )
    if (!(Test-Path -Path $Path)) {
        New-Item -ItemType Junction -Path $Path -Target $Target
    }
}

# Ugly hack to make nuget packages in submodules work by linking our
# packages into the submodules
$aquaPackages = Join-Path $PSScriptRoot 'AquaLibrary\packages'
$rootPackages = Join-Path $PSScriptRoot 'packages'

New-Item -ItemType Directory -Force -Path $rootPackages | Out-Null
New-Junction -Path $aquaPackages -Target $rootPackages

# Link FBX dependencies into AquaModelLibrary.Native
$fbxTarget = 'C:\Program Files\Autodesk\FBX\FBX SDK\2020.3.2'
$fbxPath = Join-Path $PSScriptRoot 'AquaLibrary\AquaModelLibrary.Native\Dependencies\FBX'

New-Junction -Path "$fbxPath/lib" -Target "$fbxTarget/lib"
New-Junction -Path "$fbxPath/include" -Target "$fbxTarget/include"
