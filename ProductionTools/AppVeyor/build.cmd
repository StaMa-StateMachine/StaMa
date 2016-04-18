@setlocal

@set APPVEYOR_

@echo Info: Platform=%PLATFORM%
@echo Info: Configuration=%CONFIGURATION%

msbuild StaMa.sln /t:Clean,Build /p:Configuration=Debug /p:Platform="Any CPU"
msbuild StaMaNETMF.sln /t:Clean,Build /p:Configuration=Debug /p:Platform="Any CPU"
msbuild DevelopersGuide\StaMaDevelopersGuide.shfbproj /t:Clean,Build /p:Configuration=Debug /p:Platform="AnyCPU"


if "%APPVEYOR_BUILD_VERSION%"=="" set APPVEYOR_BUILD_VERSION=0.0.0.0


if not "%APPVEYOR_REPO_COMMIT%"=="" @echo Built from Git commit: https://github.com/StaMa-StateMachine/StaMa/commit/%APPVEYOR_REPO_COMMIT%>VersionInfo.txt
if "%APPVEYOR_REPO_TAG%"=="true" @echo Release: https://github.com/StaMa-StateMachine/StaMa/releases/tag/%APPVEYOR_REPO_TAG_NAME%>VersionInfo.txt


:: Generating nuget package
nuget pack StaMa.StateMachine.nuspec -Version %APPVEYOR_BUILD_VERSION% -Symbols -Verbosity detailed -OutputDirectory bin\
appveyor PushArtifact "bin\StaMa.StateMachine.%APPVEYOR_BUILD_VERSION%.nupkg" -DeploymentName NuGetPackage -Type NuGetPackage
appveyor PushArtifact "bin\StaMa.StateMachine.%APPVEYOR_BUILD_VERSION%.symbols.nupkg" -DeploymentName NuGetSymbolsPackage -Type NuGetPackage


:: Generating github release package
robocopy /S . bin\SourceCodeAndBinariesZip\StaMa_State_Machine_Controller_Library ChangeLog.txt LICENSE StaMa.sln StaMaNETMF.sln VersionInfo.txt /xd bin /fp /ndl
robocopy /S StaMa bin\SourceCodeAndBinariesZip\StaMa_State_Machine_Controller_Library\StaMa /xd obj /xf *.snk *.enc /fp /ndl
robocopy /S Tests bin\SourceCodeAndBinariesZip\StaMa_State_Machine_Controller_Library\Tests /xd obj DOTNETMF_FS_EMULATION /xf OnBoardFlash.dat* /fp /ndl
robocopy /S Samples bin\SourceCodeAndBinariesZip\StaMa_State_Machine_Controller_Library\Samples /xd obj DOTNETMF_FS_EMULATION /xf OnBoardFlash.dat* /fp /ndl
robocopy /S bin\netmf\AnyCPU bin\SourceCodeAndBinariesZip\StaMa_State_Machine_Controller_Library\bin\netmf\AnyCPU /fp /ndl
robocopy /S bin\net-4.0\AnyCPU bin\SourceCodeAndBinariesZip\StaMa_State_Machine_Controller_Library\bin\net-4.0\AnyCPU /fp /ndl
robocopy /S bin\netmf\DevelopersGuide bin\SourceCodeAndBinariesZip\StaMa_State_Machine_Controller_Library\bin\netmf\DevelopersGuide StaMaDevelopersGuide.chm /fp /ndl
robocopy /S ProductionTools\ExtractVisioPages\Artifacts bin\SourceCodeAndBinariesZip\StaMa_State_Machine_Controller_Library /fp /ndl

set StaMaZip=bin\StaMa_State_Machine_Controller_Library.%APPVEYOR_BUILD_VERSION%.zip
@pushd bin\SourceCodeAndBinariesZip
7z a ..\..\%StaMaZip% StaMa_State_Machine_Controller_Library
@popd
appveyor PushArtifact "%StaMaZip%" -DeploymentName SourceCodeAndBinariesZip

:: Save developers guide web content
set DevelopersGuideZip=bin\StaMa_DevelopersGuidePages.%APPVEYOR_BUILD_VERSION%.zip
@pushd bin\netmf\DevelopersGuide
del *.chm
7z a ..\..\..\%DevelopersGuideZip% .
@popd
appveyor PushArtifact "%DevelopersGuideZip%" -DeploymentName DevelopersGuideZip

@endlocal
