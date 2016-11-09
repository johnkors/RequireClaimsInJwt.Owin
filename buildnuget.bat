@echo off
set csproj=source\RequireClaimsInJwt.Owin\RequireClaimsInJwt.Owin.csproj
set version=1.2.0
set msbuildexe=
for /D %%D in (%SYSTEMROOT%\Microsoft.NET\Framework\v4*) do set msbuildexe=%%D\MSBuild.exe
set xunitrunnerpath=source\packages\xunit.runner.console.2.1.0\tools\xunit.console.exe

nuget restore %csproj% -SolutionDirectory source\


if %ERRORLEVEL% EQU 0 (
   %msbuildexe% source\Solution.sln /p:Configuration=Release
) else (
   echo **************** Nuget restore failed ****************
   exit /b %errorlevel%
)

if %ERRORLEVEL% EQU 0 (
   %xunitrunnerpath% source\RequireClaimsInJwt.Owin.Tests\bin\Release\RequireClaimsInJwt.Owin.Tests.dll
) else (
   echo **************** MSBuild failed *****************
   exit /b %errorlevel%
)

if %ERRORLEVEL% EQU 0 (
   nuget pack %csproj% -Version %version% -OutputDirectory dist\ -Properties Configuration=Release
) else (
   echo **************** XUnit tests failed! ****************
   exit /b %errorlevel%
)

echo \n
echo *************************************************
echo **************** BUILD SUCCESS! *****************
echo *************************************************

