@echo off

set PATH=%PATH%;C:\Windows\Microsoft.NET\Framework\v4.0.30319\;bin

if not exist output ( mkdir output ) else del /q output\*.*
if exist output\merged ( rd /s /q output\merged )

if "%1" == "clean" ( goto clean )

echo Compiling...
msbuild /nologo /verbosity:quiet src/FileStorageAdapter.sln /p:Configuration=Release /t:Clean
msbuild /nologo /verbosity:quiet src/FileStorageAdapter.sln /p:Configuration=Release

echo Copying...
copy src\proj\FileStorageAdapter\bin\Release\*.* output\
copy src\proj\FileStorageAdapter.AmazonS3\bin\Release\*.* output\

if not "%1"=="merge" ( goto clean )
if not exist output\merged ( mkdir output\merged )
echo Merging...
ilmerge /keyfile:src\FileStorageAdapter.snk /internalize:exclude.txt /targetplatform:v4,c:\Windows\Microsoft.NET\Framework\v4.0.30319 /out:output\merged\FileStorageAdapter.dll output\FileStorageAdapter.dll output\FileStorageAdapter.AmazonS3.dll output\AWSSDK.dll

:clean
echo Cleaning...
msbuild /nologo /verbosity:quiet src/FileStorageAdapter.sln /p:Configuration=Release /t:Clean

echo Done.
