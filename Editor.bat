@echo off

set "RegPath=HKEY_LOCAL_MACHINE\SOFTWARE\Unity Technologies\Installer\Unity 6000.0.36f1"
set "RegKey=Location x64"

for /F "usebackq tokens=3,*" %%A IN (`reg query "%RegPath%" /v "%RegKey%" 2^>nul ^| find "%RegKey%"`) do (
    set UnityPath=%%B
)
set UnityPath=%UnityPath%\Editor\Unity.exe

echo %UnityPath%

pause

"%UnityPath%" -projectPath .
