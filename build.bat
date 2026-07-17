@echo off
REM ============================================================
REM   Build CustomWarp.dll from Source\CustomWarp.cs
REM
REM   Needs a Kerbal Space Program 1.12.x install (for the stock
REM   Managed assemblies). No Visual Studio required — uses the
REM   csc.exe that ships with the .NET Framework on every Windows box.
REM   No third-party dependencies (no Harmony).
REM
REM   Set KSP= below if it isn't found automatically.
REM ============================================================

set CSC=%WINDIR%\Microsoft.NET\Framework64\v4.0.30319\csc.exe

REM --- Locate the KSP install --------------------------------------
set STEAM=C:\Program Files (x86)\Steam\steamapps\common
set KSP=
if exist "%STEAM%\Kerbal Space Program\KSP_x64_Data\Managed\Assembly-CSharp.dll" set KSP=%STEAM%\Kerbal Space Program
if "%KSP%"=="" if exist "%STEAM%\Kerbal Space Program-Current\KSP_x64_Data\Managed\Assembly-CSharp.dll" set KSP=%STEAM%\Kerbal Space Program-Current
if "%KSP%"=="" (
    echo ERROR: Could not find a KSP install under "%STEAM%".
    echo Edit build.bat and set KSP= to your Kerbal Space Program folder.
    pause
    exit /b 1
)
set MANAGED=%KSP%\KSP_x64_Data\Managed
echo Using KSP install: %KSP%

REM --- Stock references (UnityEngine.dll is split into modules on 1.8+) ---
set REFS=/reference:"%MANAGED%\Assembly-CSharp.dll"
set REFS=%REFS% /reference:"%MANAGED%\Assembly-CSharp-firstpass.dll"
if exist "%MANAGED%\UnityEngine.dll"                   set REFS=%REFS% /reference:"%MANAGED%\UnityEngine.dll"
if exist "%MANAGED%\UnityEngine.CoreModule.dll"        set REFS=%REFS% /reference:"%MANAGED%\UnityEngine.CoreModule.dll"
if exist "%MANAGED%\UnityEngine.IMGUIModule.dll"       set REFS=%REFS% /reference:"%MANAGED%\UnityEngine.IMGUIModule.dll"
if exist "%MANAGED%\UnityEngine.InputLegacyModule.dll" set REFS=%REFS% /reference:"%MANAGED%\UnityEngine.InputLegacyModule.dll"

REM --- Compile -----------------------------------------------------
if not exist "GameData\CustomWarp\Plugins" mkdir "GameData\CustomWarp\Plugins"
"%CSC%" /nologo /target:library /optimize+ ^
    /out:"GameData\CustomWarp\Plugins\CustomWarp.dll" ^
    %REFS% "Source\CustomWarp.cs"
if errorlevel 1 (
    echo BUILD FAILED.
    pause
    exit /b 1
)

echo.
echo ============================================================
echo Build OK: GameData\CustomWarp\Plugins\CustomWarp.dll
echo Copy the GameData\CustomWarp folder into your KSP GameData.
echo ============================================================
