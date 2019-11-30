@echo off
rem goto upload

if not exist ".\packages\OpenCover.4.7.922\tools\OpenCover.Console.exe" goto error_console1
if not exist ".\packages\NUnit.ConsoleRunner.3.10.0\tools\nunit3-console.exe" goto error_console2
if not exist ".\Test-Easly-Compiler\bin\x64\Debug\Test-Easly-Compiler.dll" goto error_not_built
if not exist ".\Test-Easly-Compiler\bin\x64\Release\Test-Easly-Compiler.dll" goto error_not_built
if exist *.log del *.log
if exist .\Test-Easly-Compiler\obj\x64\Debug\Coverage-Easly-Compiler-Debug_coverage.xml del .\Test-Easly-Compiler\obj\x64\Debug\Coverage-Easly-Compiler-Debug_coverage.xml
if exist .\Test-Easly-Compiler\obj\x64\Release\Coverage-Easly-Compiler-Release_coverage.xml del .\Test-Easly-Compiler\obj\x64\Release\Coverage-Easly-Compiler-Release_coverage.xml

:runtests
".\packages\OpenCover.4.7.922\tools\OpenCover.Console.exe" -register:user -target:".\packages\NUnit.ConsoleRunner.3.10.0\tools\nunit3-console.exe" -targetargs:".\Test-Easly-Compiler\bin\x64\Debug\Test-Easly-Compiler.dll --trace=Debug --labels=All --where=cat==Coverage" -filter:"+[Easly-Compiler*]* -[Test-Easly-Compiler*]*" -output:".\Test-Easly-Compiler\obj\x64\Debug\Coverage-Easly-Compiler-Debug_coverage.xml"
".\packages\OpenCover.4.7.922\tools\OpenCover.Console.exe" -register:user -target:".\packages\NUnit.ConsoleRunner.3.10.0\tools\nunit3-console.exe" -targetargs:".\Test-Easly-Compiler\bin\x64\Release\Test-Easly-Compiler.dll --trace=Debug --labels=All --where=cat==Coverage" -filter:"+[Easly-Compiler*]* -[Test-Easly-Compiler*]*" -output:".\Test-Easly-Compiler\obj\x64\Release\Coverage-Easly-Compiler-Release_coverage.xml"

:upload
if exist .\Test-Easly-Compiler\obj\x64\Debug\Coverage-Easly-Compiler-Debug_coverage.xml .\packages\Codecov.1.9.0\tools\codecov -f ".\Test-Easly-Compiler\obj\x64\Debug\Coverage-Easly-Compiler-Debug_coverage.xml" -t "e37c5eff-2e09-45d3-94bf-ace6e34fca12"
if exist .\Test-Easly-Compiler\obj\x64\Release\Coverage-Easly-Compiler-Release_coverage.xml .\packages\Codecov.1.9.0\tools\codecov -f ".\Test-Easly-Compiler\obj\x64\Release\Coverage-Easly-Compiler-Release_coverage.xml" -t "e37c5eff-2e09-45d3-94bf-ace6e34fca12"
goto end

:error_console1
echo ERROR: OpenCover.Console not found.
goto end

:error_console2
echo ERROR: nunit3-console not found.
goto end

:error_not_built
echo ERROR: Test-Easly-Compiler.dll not built (both Debug and Release are required).
goto end

:end
