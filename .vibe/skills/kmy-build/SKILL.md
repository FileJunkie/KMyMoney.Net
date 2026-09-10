---
name: kmy-build
description: Build, pack, and package KMyMoney.Net projects
allowed_models: all
---

# KMyMoney.Net Build Skill

## Purpose
Build, pack, publish, and create Debian packages for KMyMoney.Net.

## Activation
Load this skill when you need to:
- Build the solution
- Create NuGet packages
- Build Debian packages
- Publish releases
- Manage build configurations

## Commands

### Build Solution
```bash
# Build all projects (Debug)
dotnet build KMyMoney.Net.sln

# Build Release configuration
dotnet build KMyMoney.Net.sln --configuration Release

# Build without restoring (faster for iterative builds)
dotnet build --no-restore

# Build with verbosity
dotnet build --verbosity detailed
```

### Build Specific Project
```bash
dotnet build KMyMoney.Net.Core/KMyMoney.Net.Core.csproj
dotnet build KMyMoney.Net.TelegramBot/KMyMoney.Net.TelegramBot.csproj
```

### Restore Packages
```bash
# Restore all packages
dotnet restore KMyMoney.Net.sln

# Restore specific project
dotnet restore KMyMoney.Net.Core/KMyMoney.Net.Core.csproj
```

## Packaging Commands

### Create NuGet Packages
```bash
# Pack a single project
dotnet pack KMyMoney.Net.Core/KMyMoney.Net.Core.csproj --configuration Release

# Pack all projects
dotnet pack KMyMoney.Net.sln --configuration Release

# Pack with specific version
dotnet pack KMyMoney.Net.Core/KMyMoney.Net.Core.csproj --configuration Release /p:Version=1.0.0

# Pack with output directory
dotnet pack --output nupkgs --configuration Release
```

### Pack Command Options
```bash
# Include symbols
dotnet pack --include-symbols

# Include source
dotnet pack --include-source

# Serviceable (patch updates)
dotnet pack /p:PackageVersion=1.0.1 --version-suffix ""

# Preview (CI builds)
dotnet pack /p:VersionSuffix="ci-$(date +%Y%m%d)-$(git rev-parse --short HEAD)"
```

## Debian Packaging

### Prerequisites
```bash
# Install dpkg
sudo apt-get update && sudo apt-get install -y dpkg dpkg-dev debhelper

# Install dotnet-deb (for .NET Debian packages)
dotnet tool install --global dotnet-deb
```

### Build Debian Package
```bash
# From packaging directory
cd packaging

# Build with dpkg-buildpackage
dpkg-buildpackage -us -uc -b

# Or use dotnet-deb
dotnet deb KMyMoney.Net.sln --configuration Release
```

### Package Structure
The `packaging/` directory contains:
```
packaging/
├── debian/
│   ├── changelog       # Debian changelog
│   ├── control         # Package metadata
│   ├── rules           # Build rules
│   ├── compat          # Compatibility level
│   └── source/
│       └── format      # Source format
├── KMyMoney.Net.service  # Systemd service file
└── README.md           # Packaging instructions
```

### Update Debian Version
```bash
# Edit debian/changelog
# Format: package (version) distribution; urgency=level
#
# Example:
# KMyMoney.Net (1.0.0-1) unstable; urgency=medium
#
#   * Initial release
#
#  -- Developer Name <email>  Date

# Increment version
dch --increment

# New version
dch --newversion 1.0.1-1 "Update to new version"
```

### Build and Install Debian Package
```bash
# Build the package
cd packaging
dpkg-buildpackage -us -uc -b

# Install the generated .deb file
sudo dpkg -i ../KMyMoney.Net_1.0.0-1_amd64.deb

# Fix dependencies
sudo apt-get install -f
```

## Publish Commands

### Publish to NuGet
```bash
# Publish a single package
dotnet nuget push KMyMoney.Net.Core.1.0.0.nupkg --source https://api.nuget.org/v3/index.json --api-key YOUR_KEY

# Publish all packages from directory
for pkg in *.nupkg; do
    dotnet nuget push $pkg --source https://api.nuget.org/v3/index.json --api-key YOUR_KEY
    # Add --skip-duplicate to avoid errors on existing versions
done
```

### Publish to Local Feed
```bash
# Create local feed directory
mkdir ~/nuget-local

# Add as NuGet source
nuget sources Add -Name "Local" -Path ~/nuget-local

# Publish to local feed
dotnet nuget push KMyMoney.Net.Core.1.0.0.nupkg --source ~/nuget-local
```

## Build Configurations

### Debug Configuration
- Symbols included
- Optimizations disabled
- Debug information included
- Used for development

### Release Configuration
- Optimizations enabled
- Debug information optional
- Used for production

### Custom Configuration
Create or modify in `.csproj`:
```xml
<PropertyGroup Condition=" '$(Configuration)' == 'Custom' ">
    <Optimize>true</Optimize>
    <DebugType>pdbonly</DebugType>
    <DebugSymbols>true</DebugSymbols>
</PropertyGroup>
```

Build with:
```bash
dotnet build --configuration Custom
```

## Multi-Target Build

### Target Multiple .NET Versions
```xml
<!-- In .csproj -->
<TargetFrameworks>net8.0;net9.0</TargetFrameworks>
```

### Target Multiple Platforms
```bash
# Build for Linux x64
dotnet publish --runtime linux-x64 --self-contained true

# Build for Windows x64
dotnet publish --runtime win-x64 --self-contained true

# Build for macOS arm64
dotnet publish --runtime osx-arm64 --self-contained true
```

### Framework-Dependent Deployment (FDD)
```bash
# Smallest package, requires .NET runtime on target
dotnet publish --runtime linux-x64 --self-contained false
```

### Self-Contained Deployment (SCD)
```bash
# Includes .NET runtime, larger package
dotnet publish --runtime linux-x64 --self-contained true
```

## Clean Commands

### Clean Solution
```bash
dotnet clean KMyMoney.Net.sln
```

### Clean Specific Project
```bash
dotnet clean KMyMoney.Net.Core/KMyMoney.Net.Core.csproj
```

### Clean All (including user profile cache)
```bash
dotnet clean KMyMoney.Net.sln --configuration Release
dotnet nuget locals all -c
```

### Remove bin/obj directories
```bash
find . -type d \( -name bin -o -name obj \) -exec rm -rf {} + 2>/dev/null
```

## Build Analysis

### Check Build Time
```bash
time dotnet build KMyMoney.Net.sln
```

### Check Build Output Size
```bash
# After build, check sizes
du -sh KMyMoney.Net.Core/bin/Debug/net*/
```

### Check Dependencies in Output
```bash
ldd KMyMoney.Net.TelegramBot/bin/Debug/net*/KMyMoney.Net.TelegramBot
```

## CI/CD Build Commands

### GitHub Actions Build
```yaml
- name: Build
  run: dotnet build KMyMoney.Net.sln --configuration Release

- name: Test
  run: dotnet test KMyMoney.Net.sln --configuration Release --no-build

- name: Pack
  run: dotnet pack KMyMoney.Net.sln --configuration Release --output nupkgs
```

### Local CI Simulation
```bash
# Clean build
rm -rf bin obj

# Restore
dotnet restore KMyMoney.Net.sln

# Build
dotnet build KMyMoney.Net.sln --configuration Release --no-restore

# Test
dotnet test KMyMoney.Net.sln --configuration Release --no-build

# Pack
dotnet pack KMyMoney.Net.sln --configuration Release --no-build --output nupkgs
```

## Best Practices

1. **Always build Release before commit**
2. **Clean before build** when switching branches
3. **Use --no-restore** for faster iterative builds
4. **Build --configuration Release** for final verification
5. **Check all projects build** before packaging
6. **Verify tests pass** before creating packages
7. **Use deterministic builds** for reproducible results

## Troubleshooting

### Build Errors
```bash
# Check for specific errors
dotnet build --verbosity detailed

# Check project references
dotnet list KMyMoney.Net.TelegramBot reference
```

### Missing SDK
```bash
# List installed SDKs
dotnet --list-sdks

# Install missing SDK
dotnet install --sdk-version 8.0.100
```

### Package Restore Issues
```bash
# Clear cache
dotnet nuget locals all -c

# Restore with force
dotnet restore --force
```

### Version Conflicts
```bash
# Check for version conflicts
dotnet list package --include-transitive
```
