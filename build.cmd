@echo off

if not exist output ( mkdir output )

echo Compiling
msbuild /nologo /verbosity:quiet src/FileStorageAdapter.sln /p:Configuration=Release /t:Clean
msbuild /nologo /verbosity:quiet src/FileStorageAdapter.sln /p:Configuration=Release

echo Copying
copy src\proj\FileStorageAdapter\bin\Release\*.* output\

echo Cleaning
msbuild /nologo /verbosity:quiet src/FileStorageAdapter.sln /p:Configuration=Release /t:Clean

echo Done
