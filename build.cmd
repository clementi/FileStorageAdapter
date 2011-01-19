@echo off

set PATH=%PATH%;C:\Windows\Microsoft.NET\Framework\v4.0.30319\

if not exist output ( mkdir output )

echo Compiling
msbuild /nologo /verbosity:quiet src/FileStorageAdapter.sln /p:Configuration=Release /t:Clean
msbuild /nologo /verbosity:quiet src/FileStorageAdapter.sln /p:Configuration=Release

echo Copying
copy src\proj\FileStorageAdapter\bin\Release\*.* output\
copy src\proj\FileStorageAdapter.AmazonS3\bin\Release\*.* output\

echo Cleaning
msbuild /nologo /verbosity:quiet src/FileStorageAdapter.sln /p:Configuration=Release /t:Clean

echo Done
