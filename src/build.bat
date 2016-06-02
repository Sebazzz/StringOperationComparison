@echo off

set msbuild="C:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe"

echo RESTORING PACKAGES
.nuget\nuget restore || goto :error

echo BUILDING
%msbuild% Benchmark.sln /p:Configuration=Release /t:Rebuild /p:Platform="Any CPU" /nologo /verbosity:normal || goto :error

echo RUNNING BENCHMARK
pushd Benchmark\bin\release
Benchmark.exe
popd

pause
goto :EOF

:error
echo Error while building. exiting.
pause