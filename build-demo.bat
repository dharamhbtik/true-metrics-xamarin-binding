@echo off
REM Build script for TrueMetrics Demo Application on Windows

echo ==================================
echo TrueMetrics Demo Build Script
echo ==================================
echo.

REM Check for .NET SDK
dotnet --version >nul 2>&1
if errorlevel 1 (
    echo [ERROR] .NET SDK not found. Please install .NET 8.0 or later.
    exit /b 1
)

echo [INFO] .NET SDK found
echo.

echo [INFO] Step 1: Cleaning previous builds...
if exist "TrueMetricsDemo\bin" rmdir /s /q "TrueMetricsDemo\bin"
if exist "TrueMetricsDemo\obj" rmdir /s /q "TrueMetricsDemo\obj"
if exist "TrueMetricsDemo.Droid\bin" rmdir /s /q "TrueMetricsDemo.Droid\bin"
if exist "TrueMetricsDemo.Droid\obj" rmdir /s /q "TrueMetricsDemo.Droid\obj"
echo [INFO]   Cleaned build directories
echo.

echo [INFO] Step 2: Restoring NuGet packages...
dotnet restore TrueMetricsDemo\TrueMetricsDemo.csproj --verbosity quiet
dotnet restore TrueMetricsDemo.Droid\TrueMetricsDemo.Droid.csproj --verbosity quiet
echo [INFO]   Packages restored successfully
echo.

echo [INFO] Step 3: Building TrueMetricsDemo (Shared Library)...
dotnet build TrueMetricsDemo\TrueMetricsDemo.csproj --configuration Debug --no-restore --verbosity minimal
if errorlevel 1 (
    echo [ERROR] Shared library build failed
    exit /b 1
)
echo [INFO]   Shared library build completed
echo.

echo [INFO] Step 4: Building TrueMetricsDemo.Droid (Android)...
dotnet build TrueMetricsDemo.Droid\TrueMetricsDemo.Droid.csproj --configuration Debug --no-restore --verbosity minimal
if errorlevel 1 (
    echo [ERROR] Android project build failed
    echo [INFO] Trying with MSBuild...
    
    REM Try MSBuild if available
    where msbuild >nul 2>&1
    if %errorlevel% == 0 (
        msbuild TrueMetricsDemo.Droid\TrueMetricsDemo.Droid.csproj /p:Configuration=Debug /p:Platform=AnyCPU /restore /verbosity:minimal
        if errorlevel 1 (
            echo [ERROR] MSBuild also failed
            exit /b 1
        )
    ) else (
        exit /b 1
    )
)
echo [INFO]   Android project build completed
echo.

echo [INFO] Step 5: Build summary
echo.

if exist "TrueMetricsDemo\bin\Debug\netstandard2.1\TrueMetricsDemo.dll" (
    echo [INFO]   OK: Shared library built successfully
) else (
    echo [ERROR]   Shared library not found
)

if exist "TrueMetricsDemo.Droid\bin\Debug\TrueMetricsDemo.Droid.dll" (
    echo [INFO]   OK: Android library built successfully
) else (
    echo [ERROR]   Android library not found
)

echo.
echo ==================================
echo Build completed!
echo ==================================
echo.
echo [INFO] Next steps:
echo   1. Open MyPackSmall.sln in Visual Studio
echo   2. Set 'TrueMetricsDemo.Droid' as startup project
echo   3. Connect Android device or start emulator
echo   4. Press F5 to debug and deploy
echo.

pause
