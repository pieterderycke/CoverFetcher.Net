SET version=0.1

msbuild /p:Configuration=Release CoverFetcher.Net\CoverFetcher.Net.csproj
MKDIR zip
COPY CoverFetcher.Net\bin\Release\*.exe zip\
COPY CoverFetcher.Net\bin\Release\*.dll zip\
COPY CoverFetcher.Net\bin\Release\*.config zip\

COPY LICENSE.md zip\
COPY README.md zip\

RM Jace.%version%.zip
Tools\7-Zip\7za.exe a -tzip CoverFetcher_v%version%.zip .\zip\*

RMDIR zip /S /Q