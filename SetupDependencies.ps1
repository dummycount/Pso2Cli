param (
    [switch] $ReleaseOnly
)

function New-Junction {
    param (
        [string] $Path,
        [string] $Target
    )
    if (!(Test-Path -Path $Path)) {
        New-Item -ItemType Junction -Path $Path -Target $Target
    }
}

# Link FBX dependencies into AquaModelLibrary.Native
$fbxTarget = 'C:\Program Files\Autodesk\FBX\FBX SDK\2020.1'
$fbxPath = Join-Path $PSScriptRoot 'PSO2-Aqua-Library\AquaModelLibrary.Native\Dependencies\FBX'

if (!(Test-Path $fbxTarget)) {
    Write-Output 'Could not find FBX SDK. Install from:'
    Write-Output 'https://www.autodesk.com/content/dam/autodesk/www/adn/fbx/2020-1/fbx20201_fbxsdk_vs2017_win.exe'
    exit 1
}

New-Junction -Path "$fbxPath/lib" -Target "$fbxTarget/lib"
New-Junction -Path "$fbxPath/include" -Target "$fbxTarget/include"
