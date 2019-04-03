@echo off
rem goto upload

if not exist "..\Misc-Beta-Test\packages\OpenCover.4.6.519\tools\OpenCover.Console.exe" goto error_console1
if not exist "..\Misc-Beta-Test\packages\NUnit.ConsoleRunner.3.9.0\tools\nunit3-console.exe" goto error_console2
if not exist "..\Misc-Beta-Test\Test-Easly-Compiler\bin\x64\Debug\Test-Easly-Compiler.dll" goto error_largelist
if not exist "..\Misc-Beta-Test\Test-Easly-Compiler\bin\x64\Release\Test-Easly-Compiler.dll" goto error_largelist
if exist *.log del *.log
if exist ..\Misc-Beta-Test\Test-Easly-Compiler\obj\x64\Debug\Coverage-Easly-Compiler-Debug_coverage.xml del ..\Misc-Beta-Test\Test-Easly-Compiler\obj\x64\Debug\Coverage-Easly-Compiler-Debug_coverage.xml
if exist ..\Misc-Beta-Test\Test-Easly-Compiler\obj\x64\Release\Coverage-Easly-Compiler-Release_coverage.xml del ..\Misc-Beta-Test\Test-Easly-Compiler\obj\x64\Release\Coverage-Easly-Compiler-Release_coverage.xml

:runtests
"..\Misc-Beta-Test\packages\OpenCover.4.6.519\tools\OpenCover.Console.exe" -register:user -target:"..\Misc-Beta-Test\packages\NUnit.ConsoleRunner.3.9.0\tools\nunit3-console.exe" -targetargs:"..\Misc-Beta-Test\Test-Easly-Compiler\bin\x64\Debug\Test-Easly-Compiler.dll --trace=Debug --labels=All --where=cat==Coverage" -filter:"+[Easly-Compiler*]* -[Test-Easly-Compiler*]*" -output:"..\Misc-Beta-Test\Test-Easly-Compiler\obj\x64\Debug\Coverage-Easly-Compiler-Debug_coverage.xml"
"..\Misc-Beta-Test\packages\OpenCover.4.6.519\tools\OpenCover.Console.exe" -register:user -target:"..\Misc-Beta-Test\packages\NUnit.ConsoleRunner.3.9.0\tools\nunit3-console.exe" -targetargs:"..\Misc-Beta-Test\Test-Easly-Compiler\bin\x64\Release\Test-Easly-Compiler.dll --trace=Debug --labels=All --where=cat==Coverage" -filter:"+[Easly-Compiler*]* -[Test-Easly-Compiler*]*" -output:"..\Misc-Beta-Test\Test-Easly-Compiler\obj\x64\Release\Coverage-Easly-Compiler-Release_coverage.xml"

:upload
if exist ..\Misc-Beta-Test\Test-Easly-Compiler\obj\x64\Debug\Coverage-Easly-Compiler-Debug_coverage.xml ..\Misc-Beta-Test\packages\Codecov.1.1.1\tools\codecov -f "..\Misc-Beta-Test\Test-Easly-Compiler\obj\x64\Debug\Coverage-Easly-Compiler-Debug_coverage.xml" -t "e37c5eff-2e09-45d3-94bf-ace6e34fca12"
if exist ..\Misc-Beta-Test\Test-Easly-Compiler\obj\x64\Release\Coverage-Easly-Compiler-Release_coverage.xml ..\Misc-Beta-Test\packages\Codecov.1.1.1\tools\codecov -f "..\Misc-Beta-Test\Test-Easly-Compiler\obj\x64\Release\Coverage-Easly-Compiler-Release_coverage.xml" -t "e37c5eff-2e09-45d3-94bf-ace6e34fca12"
goto end

:error_console1
echo ERROR: OpenCover.Console not found.
goto end

:error_console2
echo ERROR: nunit3-console not found.
goto end

:error_largelist
echo ERROR: Test-Easly-Compiler.dll not built (both Debug and Release are required).
goto end

:end
