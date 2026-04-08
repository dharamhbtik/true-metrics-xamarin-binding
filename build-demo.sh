#!/bin/bash
# Build script for TrueMetrics Demo Application

set -e

echo "=================================="
echo "TrueMetrics Demo Build Script"
echo "=================================="
echo ""

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Function to print colored output
print_status() {
    echo -e "${GREEN}[INFO]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARN]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Check if running on macOS or Linux
if [[ "$OSTYPE" == "darwin"* ]]; then
    print_status "Detected macOS"
    PLATFORM="macOS"
elif [[ "$OSTYPE" == "linux-gnu"* ]]; then
    print_status "Detected Linux"
    PLATFORM="Linux"
else
    print_warning "Unknown platform: $OSTYPE"
    PLATFORM="Unknown"
fi

# Check for required tools
print_status "Checking prerequisites..."

if ! command -v dotnet &> /dev/null; then
    print_error ".NET SDK not found. Please install .NET 8.0 or later."
    exit 1
fi

if ! command -v msbuild &> /dev/null; then
    if ! command -v "dotnet" msbuild &> /dev/null; then
        print_warning "MSBuild not found in PATH. Will use dotnet build."
        USE_MSBUILD=false
    else
        USE_MSBUILD=true
    fi
else
    USE_MSBUILD=true
fi

# Display .NET version
print_status ".NET SDK version:"
dotnet --version

echo ""
print_status "Step 1: Cleaning previous builds..."

# Clean function
clean_project() {
    local project_path=$1
    if [ -d "$project_path/bin" ]; then
        rm -rf "$project_path/bin"
        print_status "  Cleaned $project_path/bin"
    fi
    if [ -d "$project_path/obj" ]; then
        rm -rf "$project_path/obj"
        print_status "  Cleaned $project_path/obj"
    fi
}

clean_project "TrueMetricsDemo"
clean_project "TrueMetricsDemo.Droid"

echo ""
print_status "Step 2: Restoring NuGet packages..."

# Restore packages
if [ -f "NuGet.config" ]; then
    print_status "  Using NuGet.config"
fi

dotnet restore TrueMetricsDemo/TrueMetricsDemo.csproj --verbosity quiet
dotnet restore TrueMetricsDemo.Droid/TrueMetricsDemo.Droid.csproj --verbosity quiet

print_status "  Packages restored successfully"

echo ""
print_status "Step 3: Building TrueMetricsDemo (Shared Library)..."

dotnet build TrueMetricsDemo/TrueMetricsDemo.csproj \
    --configuration Debug \
    --no-restore \
    --verbosity minimal

print_status "  Shared library build completed"

echo ""
print_status "Step 4: Building TrueMetricsDemo.Droid (Android)..."

if [ "$USE_MSBUILD" = true ] && [ "$PLATFORM" = "macOS" ]; then
    print_status "  Using MSBuild for Android project..."
    
    # Try to find MSBuild
    if [ -f "/Applications/Visual Studio.app/Contents/MacOS/vstool" ]; then
        print_status "  Found Visual Studio for Mac"
        # Use Visual Studio's MSBuild
        MSBUILD_PATH="/Applications/Visual Studio.app/Contents/MonoBundle/MSBuild/Current/bin/MSBuild"
        if [ -f "$MSBUILD_PATH" ]; then
            "$MSBUILD_PATH" TrueMetricsDemo.Droid/TrueMetricsDemo.Droid.csproj \
                /p:Configuration=Debug \
                /p:Platform=AnyCPU \
                /restore \
                /verbosity:minimal
        else
            print_warning "  MSBuild not found at expected path, using dotnet"
            dotnet build TrueMetricsDemo.Droid/TrueMetricsDemo.Droid.csproj \
                --configuration Debug \
                --no-restore \
                --verbosity minimal
        fi
    else
        msbuild TrueMetricsDemo.Droid/TrueMetricsDemo.Droid.csproj \
            /p:Configuration=Debug \
            /p:Platform=AnyCPU \
            /restore \
            /verbosity:minimal || {
            print_warning "  MSBuild failed, falling back to dotnet build..."
            dotnet build TrueMetricsDemo.Droid/TrueMetricsDemo.Droid.csproj \
                --configuration Debug \
                --no-restore \
                --verbosity minimal
        }
    fi
else
    dotnet build TrueMetricsDemo.Droid/TrueMetricsDemo.Droid.csproj \
        --configuration Debug \
        --no-restore \
        --verbosity minimal
fi

print_status "  Android project build completed"

echo ""
print_status "Step 5: Build summary"

# Check for build outputs
SHARED_DLL="TrueMetricsDemo/bin/Debug/netstandard2.1/TrueMetricsDemo.dll"
ANDROID_DLL="TrueMetricsDemo.Droid/bin/Debug/TrueMetricsDemo.Droid.dll"

if [ -f "$SHARED_DLL" ]; then
    print_status "  ✓ Shared library: $SHARED_DLL"
else
    print_error "  ✗ Shared library not found"
fi

if [ -f "$ANDROID_DLL" ]; then
    print_status "  ✓ Android library: $ANDROID_DLL"
else
    print_error "  ✗ Android library not found"
fi

# List APK if built
APK_PATH=$(find TrueMetricsDemo.Droid/bin/Debug -name "*.apk" -type f 2>/dev/null | head -1)
if [ -n "$APK_PATH" ]; then
    print_status "  ✓ APK: $APK_PATH"
else
    print_warning "  - No APK found (normal for library builds)"
fi

echo ""
print_status "=================================="
print_status "Build completed successfully!"
print_status "=================================="
echo ""
print_status "Next steps:"
echo "  1. Open MyPackSmall.sln in Visual Studio"
echo "  2. Set 'TrueMetricsDemo.Droid' as startup project"
echo "  3. Connect Android device or start emulator"
echo "  4. Press F5 to debug and deploy"
echo ""
print_status "Or deploy from command line:"
echo "  msbuild TrueMetricsDemo.Droid/TrueMetricsDemo.Droid.csproj /t:Install"
echo ""
