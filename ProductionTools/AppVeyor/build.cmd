@setlocal
msbuild StaMa.sln /t:Clean,Build /p:Configuration=Debug /p:Platform="Any CPU"
msbuild StaMaNETMF.sln /t:Clean,Build /p:Configuration=Debug /p:Platform="Any CPU"
msbuild DevelopersGuide\StaMaDevelopersGuide.shfbproj /t:Clean,Build /p:Configuration=Debug /p:Platform="AnyCPU"

xcopy /S /F /Y ProductionTools\ExtractVisioPages\Artifacts\*.vs* .
del StaMa\StaMa.snk
for /f %%i in ('dir /b /s /a:d obj') do rd /s /q %%i
for /f %%i in ('dir /b /s /a:d DOTNETMF_FS_EMULATION') do rd /s /q %%i
for /f %%i in ('dir /b /s OnBoardFlash.*') do del %%i

@pushd ..
set StaMaZip=StaMa\bin\StaMa_State_Machine_Controller_Library.zip
7z a %StaMaZip% StaMa\StaMa
7z a %StaMaZip% StaMa\Tests
7z a %StaMaZip% StaMa\Samples
7z a %StaMaZip% StaMa\bin\netmf\AnyCPU
7z a %StaMaZip% StaMa\bin\net-4.0\AnyCPU
7z a %StaMaZip% StaMa\bin\netmf\DevelopersGuide\StaMaDevelopersGuide.chm
7z a %StaMaZip% StaMa\ChangeLog.txt
7z a %StaMaZip% StaMa\LICENSE
7z a %StaMaZip% StaMa\StaMa.sln
7z a %StaMaZip% StaMa\StaMaNETMF.sln
7z a %StaMaZip% StaMa\StaMaShapes.vst
@popd

nuget pack StaMa.StateMachine.nuspec -Symbols -OutputDirectory bin\

@endlocal
