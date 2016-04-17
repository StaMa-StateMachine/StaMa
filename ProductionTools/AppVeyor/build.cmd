@setlocal

@set

@echo Info: Platform=%PLATFORM%
@echo Info: Configuration=%CONFIGURATION%

msbuild StaMa.sln /t:Clean,Build /p:Configuration=Debug /p:Platform="Any CPU"
msbuild StaMaNETMF.sln /t:Clean,Build /p:Configuration=Debug /p:Platform="Any CPU"
msbuild DevelopersGuide\StaMaDevelopersGuide.shfbproj /t:Clean,Build /p:Configuration=Debug /p:Platform="AnyCPU"


if "%APPVEYOR_REPO_COMMIT%" EQU "" set APPVEYOR_REPO_COMMIT=999.999.999.999
set ArtifactVersionTag=%APPVEYOR_REPO_COMMIT%
if "%APPVEYOR_REPO_TAG%"=="true" set ProductVersion=%APPVEYOR_REPO_TAG_NAME%
@echo Info: ArtifactVersionTag=%ArtifactVersionTag%


:: Generating nuget package
nuget pack StaMa.StateMachine.nuspec -Version %ArtifactVersionTag% -Symbols -Verbosity detailed -OutputDirectory bin\
appveyor PushArtifact "bin\StaMa.StateMachine.%ArtifactVersionTag%.nupkg" -DeploymentName NuGetPackage
appveyor PushArtifact "bin\StaMa.StateMachine.%ArtifactVersionTag%.symbols.nupkg" -DeploymentName NuGetSymbolsPackage


:: Generating github release package
robocopy /S . bin\SourceCodeAndBinariesZip\StaMa_State_Machine_Controller_Library ChangeLog.txt LICENSE StaMa.sln StaMaNETMF.sln /xd bin /fp /ndl
robocopy /S StaMa bin\SourceCodeAndBinariesZip\StaMa_State_Machine_Controller_Library\StaMa /xd obj /xf *.snk *.enc /fp /ndl
robocopy /S Tests bin\SourceCodeAndBinariesZip\StaMa_State_Machine_Controller_Library\Tests /xd obj DOTNETMF_FS_EMULATION /xf OnBoardFlash.dat* /fp /ndl
robocopy /S Samples bin\SourceCodeAndBinariesZip\StaMa_State_Machine_Controller_Library\Samples /xd obj DOTNETMF_FS_EMULATION /xf OnBoardFlash.dat* /fp /ndl
robocopy /S bin\netmf\AnyCPU bin\SourceCodeAndBinariesZip\StaMa_State_Machine_Controller_Library\bin\netmf\AnyCPU /fp /ndl
robocopy /S bin\net-4.0\AnyCPU bin\SourceCodeAndBinariesZip\StaMa_State_Machine_Controller_Library\bin\net-4.0\AnyCPU /fp /ndl
robocopy /S bin\netmf\DevelopersGuide bin\SourceCodeAndBinariesZip\StaMa_State_Machine_Controller_Library\bin\netmf\DevelopersGuide StaMaDevelopersGuide.chm /fp /ndl
robocopy /S ProductionTools\ExtractVisioPages\Artifacts bin\SourceCodeAndBinariesZip\StaMa_State_Machine_Controller_Library /fp /ndl

set StaMaZip=bin\StaMa_State_Machine_Controller_Library.%ArtifactVersionTag%.zip
@pushd bin\SourceCodeAndBinariesZip
7z a ..\..\%StaMaZip% StaMa_State_Machine_Controller_Library
@popd
appveyor PushArtifact "%StaMaZip%" -DeploymentName SourceCodeAndBinariesZip

@endlocal
