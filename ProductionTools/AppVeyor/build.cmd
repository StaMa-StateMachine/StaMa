@setlocal

@echo Platform=%PLATFORM%
@echo Configuration=%CONFIGURATION%

msbuild StaMa.sln /t:Clean,Build /p:Configuration=Debug /p:Platform="Any CPU"
msbuild StaMaNETMF.sln /t:Clean,Build /p:Configuration=Debug /p:Platform="Any CPU"
msbuild DevelopersGuide\StaMaDevelopersGuide.shfbproj /t:Clean,Build /p:Configuration=Debug /p:Platform="AnyCPU"

xcopy /S /F /Y ProductionTools\ExtractVisioPages\Artifacts\*.vs* .
del StaMa\StaMa.snk
for /f %%i in ('dir /b /s /a:d obj') do rd /s /q %%i
for /f %%i in ('dir /b /s /a:d DOTNETMF_FS_EMULATION') do rd /s /q %%i
for /f %%i in ('dir /b /s OnBoardFlash.*') do del %%i

:: Generating nuget package
nuget pack StaMa.StateMachine.nuspec -Version %APPVEYOR_BUILD_VERSION% -Symbols -OutputDirectory bin\
appveyor PushArtifact "bin\StaMa.StateMachine.%APPVEYOR_BUILD_VERSION%.nupkg"
appveyor PushArtifact "bin\StaMa.StateMachine.%APPVEYOR_BUILD_VERSION%.symbols.nupkg"

:: Generating github release package
set StaMaZip=bin\StaMa_State_Machine_Controller_Library.%APPVEYOR_BUILD_VERSION%.zip

@pushd ..
7z a StaMa\%StaMaZip% StaMa\StaMa
7z a StaMa\%StaMaZip% StaMa\Tests
7z a StaMa\%StaMaZip% StaMa\Samples
7z a StaMa\%StaMaZip% StaMa\bin\netmf\AnyCPU
7z a StaMa\%StaMaZip% StaMa\bin\net-4.0\AnyCPU
7z a StaMa\%StaMaZip% StaMa\bin\netmf\DevelopersGuide\StaMaDevelopersGuide.chm
7z a StaMa\%StaMaZip% StaMa\ChangeLog.txt
7z a StaMa\%StaMaZip% StaMa\LICENSE
7z a StaMa\%StaMaZip% StaMa\StaMa.sln
7z a StaMa\%StaMaZip% StaMa\StaMaNETMF.sln
7z a StaMa\%StaMaZip% StaMa\StaMaShapes.vst
@popd

appveyor PushArtifact "%StaMaZip%"

@endlocal
