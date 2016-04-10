@setlocal
@set ExtractVisioPagesExe=%~dp0bin\Debug\ExtractVisioPages.exe
@set StaMaRootFolder=%~dp0..\..\

@echo %ExtractVisioPagesExe%

mkdir %~dp0Artifacts 2>nul
%ExtractVisioPagesExe% %StaMaRootFolder%StaMaShapesMaster.vst Template %~dp0Artifacts\StaMaShapes.vst
@if NOT ERRORLEVEL 1 goto :extractVisioPage1Success
@echo Failed to extract visio page, aborted.
@goto :exit
:extractVisioPage1Success

mkdir %~dp0Artifacts\Samples\netfwk\Clock 2>nul
%ExtractVisioPagesExe% %StaMaRootFolder%StaMaShapesMaster.vst "Clock Sample" %~dp0Artifacts\Samples\netfwk\Clock\DlgMain.vsd
@if NOT ERRORLEVEL 1 goto :extractVisioPage2Success
@echo Failed to extract visio page, aborted.
@goto :exit
:extractVisioPage2Success

mkdir %~dp0Artifacts\Samples\netfwk\SampleSimpleStateMachineNETFWK 2>nul
%ExtractVisioPagesExe% %StaMaRootFolder%StaMaShapesMaster.vst SampleSimpleStateMachineNETFWK %~dp0Artifacts\Samples\netfwk\SampleSimpleStateMachineNETFWK\SampleSimpleStateMachineNETFWK.vsd
@if NOT ERRORLEVEL 1 goto :extractVisioPage3Success
@echo Failed to extract visio page, aborted.
@goto :exit
:extractVisioPage3Success

mkdir %~dp0Artifacts\Samples\netmf\TicketVendingNETMF 2>nul
%ExtractVisioPagesExe% %StaMaRootFolder%StaMaShapesMaster.vst TicketVendingNETMF %~dp0Artifacts\Samples\netmf\TicketVendingNETMF\Program.vsd
@if NOT ERRORLEVEL 1 goto :extractVisioPage4Success
@echo Failed to extract visio page, aborted.
@goto :exit
:extractVisioPage4Success

:exit
@endlocal
