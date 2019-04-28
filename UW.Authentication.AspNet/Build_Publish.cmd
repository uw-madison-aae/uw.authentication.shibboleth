@if exist "%ProgramFiles(x86)%\Microsoft Visual Studio\2019\Professional\MSBuild\Current\Bin" set PATH=%ProgramFiles(x86)%\Microsoft Visual Studio\2019\Professional\MSBuild\Current\Bin;%PATH%
@if exist "%ProgramFiles(x86)%\NuGet" set PATH=%ProgramFiles(x86)%\NuGet;%PATH%
msbuild UW.Authentication.AspNet.csproj /t:Build;Package;Publish /p:Configuration="Release"
@echo off
for %%A in (nuget\*.nupkg) do (
	nuget push %%A -source https://nuget.aae.wisc.edu/nuget 1191E7324CAB1662
)
pause
