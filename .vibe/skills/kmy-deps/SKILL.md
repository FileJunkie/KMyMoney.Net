---
name: kmy-deps
description: Manage NuGet dependencies for KMyMoney.Net projects
allowed_models: all
---

# KMyMoney.Net Dependencies Skill

## Purpose
Check, update, and manage NuGet package dependencies across all KMyMoney.Net projects.

## Activation
Load this skill when you need to:
- Check for outdated dependencies
- Update package versions
- Add new dependencies (with permission)
- Resolve dependency conflicts
- Verify dependency compatibility

## Commands

### List all dependencies
```bash
# For all projects in solution
dotnet list package --include-transitive

# For specific project
dotnet list KMyMoney.Net.Core package

dotnet list KMyMoney.Net.TelegramBot package
```

### Check for updates
```bash
# Install dotnet-outdated tool
dotnet tool install --global dotnet-outdated

# Check all projects for outdated packages
dotnet outdated KMyMoney.Net.sln

# Check specific project
dotnet outdated KMyMoney.Net.Core/KMyMoney.Net.Core.csproj
```

### Update a dependency
```bash
# Update specific package in a project
dotnet add KMyMoney.Net.Core package Newtonsoft.Json --version 13.0.3

# Update all projects using a package
dotnet add KMyMoney.Net.sln package Shouldly --version 4.0.3

# Update to latest version
dotnet add KMyMoney.Net.Core package NSubstitute --version latest
```

### Remove a dependency
```bash
dotnet remove KMyMoney.Net.Core package PackageName
```

### Show dependency graph
```bash
dotnet list package --format json --include-transitive > deps.json
```

## Project-Specific Dependencies

### Core Projects

**KMyMoney.Net.Models**
- Minimal dependencies (data models only)
- May use: System.Xml.Serialization

**KMyMoney.Net.Core**
- System.IO.Compression (for gzip)
- System.Xml.Serialization (for XML parsing)
- KMyMoney.Net.Models (project reference)
- KMyMoney.Net.Core.FileAccessors (project reference)

**KMyMoney.Net.Core.FileAccessors**
- No external NuGet dependencies (interfaces only)

**KMyMoney.Net.Core.FileAccessors.Dropbox**
- Dropbox.Api (for Dropbox integration)
- Microsoft.Extensions.Http (for HTTP client)

### TelegramBot Projects

**KMyMoney.Net.TelegramBot**
- Telegram.Bot (for Telegram Bot API)
- Microsoft.Extensions.DependencyInjection
- Microsoft.Extensions.Logging
- KMyMoney.Net.Core (project reference)
- KMyMoney.Net.TelegramBot.Persistence (project reference)

**KMyMoney.Net.TelegramBot.Persistence**
- No external NuGet dependencies (interfaces only)

**KMyMoney.Net.TelegramBot.Persistence.Etcd**
- dotnet-etcd (for etcd client)
- Microsoft.Extensions.Options

**KMyMoney.Net.TelegramBot.Persistence.InMemory**
- No external NuGet dependencies (in-memory implementation)

### Test Projects

**All .Tests projects**
- xunit (test framework)
- xunit.runner.visualstudio (test runner)
- Shouldly (assertions)
- NSubstitute (mocking)
- Microsoft.NET.Test.Sdk

**KMyMoney.Net.Tests.Common**
- xunit
- Shouldly
- NSubstitute

### CLI Project

**KMyMoney.Net.Cli**
- Microsoft.Extensions.CommandLineUtils (or System.CommandLine)
- KMyMoney.Net.Core (project reference)

## Dependency Update Workflow

### Step 1: Check current versions
```bash
dotnet outdated KMyMoney.Net.sln
```

### Step 2: Review changelogs
- Check NuGet package page for breaking changes
- Review GitHub releases for major updates

### Step 3: Update in test project first
```bash
# Update Shouldly in one test project
dotnet add KMyMoney.Net.Core.Tests package Shouldly --version 4.0.3

# Run tests to verify compatibility
dotnet test KMyMoney.Net.Core.Tests
```

### Step 4: Update across all projects
```bash
# Update Shouldly in all test projects
for proj in $(find . -name "*Tests.csproj"); do
    dotnet add $proj package Shouldly --version 4.0.3
done
```

### Step 5: Verify solution builds
```bash
dotnet build KMyMoney.Net.sln
dotnet test KMyMoney.Net.sln
```

## Common Dependencies & Versions

| Package | Current Version | Latest Stable | Used In |
|---------|-----------------|---------------|---------|
| xunit | ~2.4.x | Check | All .Tests |
| Shouldly | ~4.0.x | Check | All .Tests |
| NSubstitute | ~5.x | Check | All .Tests |
| Telegram.Bot | ~19.x | Check | TelegramBot |
| Dropbox.Api | ~5.x | Check | FileAccessors.Dropbox |
| dotnet-etcd | ~1.x | Check | Persistence.Etcd |

## Dependency Rules

### From AGENTS.md
1. **No new dependencies without permission**
2. **Can update existing dependencies** to latest stable versions
3. **Match existing patterns** when adding new dependencies
4. **Verify compatibility** before updating

### Compatibility Matrix

| Project | .NET Version | Notes |
|---------|--------------|-------|
| All | Latest stable | Use same version across solution |

## Version Update Strategy

### Patch updates (x.x.PATCH)
- Apply immediately if no breaking changes
- Low risk, bug fixes only

### Minor updates (x.MINOR.x)
- Review changelog for new features
- Check for deprecations
- Run full test suite

### Major updates (MAJOR.x.x)
- **Require explicit permission**
- Review breaking changes
- May require code changes
- Test thoroughly

## Dependency Analysis Commands

### Show all packages with versions
```bash
for proj in $(find . -name "*.csproj" -not -path "*/obj/*"); do
    echo "=== $proj ==="
    dotnet list $proj package --format json | jq -r '.[] | "  \(.Name) \(.Version)"'
done
```

### Find projects using a specific package
```bash
grep -r "PackageReference Include=\"Shouldly\"" . --include="*.csproj"
```

### Check for transitive dependencies
```bash
dotnet list KMyMoney.Net.TelegramBot package --include-transitive
```

### Verify no duplicate versions
```bash
# List all package versions across all projects
grep -rh "PackageReference" . --include="*.csproj" | grep -oP 'Include="\K[^"]+' | sort | uniq -c
```

## Dependency Cleanup

### Remove unused packages
```bash
# Check for unused packages
dotnet list package --unused
```

### Consolidate versions
```bash
# Ensure all projects use same version of a package
# Example: Update all projects to use Shouldly 4.0.3
find . -name "*.csproj" -exec grep -l "Shouldly" {} \; | \
  xargs -I {} dotnet add {} package Shouldly --version 4.0.3
```

## Package Sources

### NuGet.org (primary)
```xml
<packageSources>
  <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
</packageSources>
```

### Check for vulnerabilities
```bash
# Install dotnet-dependency-check
dotnet tool install --global dotnet-dependency-check

# Scan for vulnerabilities
dotnet-dependency-check KMyMoney.Net.sln
```

## Best Practices

1. **Always check changelogs** before major updates
2. **Update one project at a time** for critical dependencies
3. **Run full test suite** after dependency updates
4. **Verify build succeeds** before committing
5. **Document breaking changes** in commit message
6. **Use version ranges sparingly** (prefer exact versions)
7. **Check transitive dependencies** for conflicts
