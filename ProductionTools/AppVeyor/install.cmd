set APPVEYOR_

set GIT_VERSION=
for /f %%i in ('git tag') do set GIT_VERSION=%%i
@echo Current GitVersion=%GIT_VERSION%
if %APPVEYOR_REPO_TAG%==true set GIT_VERSION=%APPVEYOR_REPO_TAG_NAME%

set V1=
set V2=
set V3=
for /f "tokens=1,2,3 delims=." %%i in ('echo %GIT_VERSION%') do set V1=%%i&set V2=%%j&set V3=%%k
if "%V1%"=="" set V1=v0
if "%V2%"=="" set V2=0
if "%V3%"=="" set V3=0

set APPVEYOR_BUILD_VERSION=%V1:~1%.%V2%.%V3%.%APPVEYOR_BUILD_NUMBER%

@echo Updated APPVEYOR_BUILD_VERSION=%APPVEYOR_BUILD_VERSION%

nuget install secure-file -ExcludeVersion
secure-file\tools\secure-file -decrypt StaMa\StaMa.snk.enc -secret %STAMA_SNK%
nuget install EWSoftware.SHFB -Version 2015.10.10 -OutputDirectory DevelopersGuide
nuget install EWSoftware.SHFB.NETMicroFramework -Version 4.3.0 -OutputDirectory DevelopersGuide
nuget restore StaMa.sln
powershell .\ProductionTools\AppVeyor\install-netmf.ps1
