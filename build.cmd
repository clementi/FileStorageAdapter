@echo off

set PATH=%PATH%;C:\Windows\Microsoft.NET\Framework\v4.0.30319\;bin

if not exist output ( mkdir output ) else ( del /q output\*.* )

if "%1" == "clean" ( goto clean )

SET EnableNuGetPackageRestore=true

echo Compiling...
msbuild /nologo /verbosity:quiet src/FileStorageAdapter.sln /p:Configuration=Release /t:Clean
msbuild /nologo /verbosity:quiet src/FileStorageAdapter.sln /p:Configuration=Release

echo Copying...
copy src\proj\FileStorageAdapter\bin\Release\*.* output\
copy src\proj\FileStorageAdapter.AmazonS3\bin\Release\*.* output\
copy src\proj\FileStorageAdapter.LocalFileSystem\bin\Release\*.* output\

:clean
echo Cleaning...
msbuild /nologo /verbosity:quiet src/FileStorageAdapter.sln /p:Configuration=Release /t:Clean

echo Done.
