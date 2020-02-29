SET version=0.3

FOR /F "tokens=*" %%i IN ('"%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe" -latest -products * -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe') do (SET msbuild=%%i)

Call "%msbuild%" /p:Configuration=Release CoverFetcher.Net\CoverFetcher.Net.csproj /t:Clean;Rebuild

MKDIR zip
COPY CoverFetcher.Net\bin\Release\*.exe zip\
COPY CoverFetcher.Net\bin\Release\*.dll zip\
COPY CoverFetcher.Net\bin\Release\*.config zip\

COPY LICENSE.md zip\
COPY README.md zip\

Tools\7-Zip\7za.exe a -tzip CoverFetcher_v%version%.zip .\zip\*

RMDIR zip /S /Q