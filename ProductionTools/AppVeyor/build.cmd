@setlocal
msbuild StaMa.sln /t:Clean,Build /p:Configuration=Release /p:Platform="Any CPU"
msbuild StaMaNETMF.sln /t:Clean,Build /p:Configuration=Release /p:Platform="Any CPU"
msbuild DevelopersGuide\StaMaDevelopersGuide.shfbproj /t:Clean,Build /p:Configuration=Release /p:Platform="AnyCPU"
@endlocal
