@echo off

if not exist "..\packages\NUnit.ConsoleRunner.3.10.0\tools\nunit3-console.exe" goto error_console
if not exist "..\Test-Easly-Compiler\bin\x64\Debug\Test-Easly-Compiler.dll" goto error_EaslyCompiler
del *.log
"..\packages\NUnit.ConsoleRunner.3.10.0\tools\nunit3-console.exe" --trace=Debug --labels=All "./bin/x64/Debug/Test-Easly-Compiler.dll"
goto end

:error_console
echo ERROR: nunit3-console not found.
goto end

:error_EaslyCompiler
echo ERROR: Test-Easly-Compiler.dll not built.
goto end

:end
