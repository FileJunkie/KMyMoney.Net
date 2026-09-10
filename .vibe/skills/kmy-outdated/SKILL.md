---
name: kmy-outdated
description: Use dotnet-outdated to check and update NuGet dependencies in KMyMoney.Net projects
allowed_models: all
---

# KMyMoney.Net dotnet-outdated Skill

## Purpose
Use the `dotnet-outdated` global tool to efficiently check, analyze, and update NuGet package dependencies across all KMyMoney.Net projects. This skill provides comprehensive workflows for dependency maintenance using the tool.

## Activation
Load this skill when you need to:
- Check which packages have newer versions available
- Analyze dependency updates across the entire solution
- Update packages interactively or automatically
- Review outdated dependencies before planning updates
- Generate reports of dependency status

## Prerequisites

### Install/Upgrade dotnet-outdated
The tool must support .NET 10.0 (requires v5.0.0+):

```bash
# Check current version
dotnet outdated --version

# Install latest version (recommended)
dotnet tool uninstall --global dotnet-outdated-tool 2>/dev/null || true
dotnet tool install --global dotnet-outdated-tool

# Alternative: Update existing installation
dotnet tool update --global dotnet-outdated-tool

# Verify version (should be 5.0.0 or higher for .NET 10.0 support)
dotnet outdated --version
```

**Note:** Version 4.x does NOT support .NET 10.0 projects and will fail with "Unable to process the project" errors.

## Basic Commands

### Check Outdated Packages

```bash
# Check all projects in solution
cd /home/filejunkie/git/KMyMoney.Net
dotnet outdated KMyMoney.Net.sln

# Check specific project
 dotnet outdated KMyMoney.Net.Core/KMyMoney.Net.Core.csproj

# Check with more details
 dotnet outdated KMyMoney.Net.sln --verbose
```

### Interactive Update Mode

```bash
# Interactive mode - prompts for each update
 dotnet outdated KMyMoney.Net.sln -u:prompt

# Auto-update all to latest stable versions
 dotnet outdated KMyMoney.Net.sln -u

# Update only patch versions (bug fixes)
 dotnet outdated KMyMoney.Net.sln -u --version-lock Minor

# Update only minor versions (features, no breaking changes)
 dotnet outdated KMyMoney.Net.sln -u --version-lock Major
```

### Version Lock Options

| Option | Description | Use Case |
|--------|-------------|----------|
| `--version-lock None` | Update to any newer version | Full updates |
| `--version-lock Minor` | Stay within minor version (x.Y.z) | Patch updates only |
| `--version-lock Major` | Stay within major version (X.y.z) | Minor updates only |

Examples:
```bash
# Update to any newer version (default)
dotnet outdated KMyMoney.Net.sln -u

# Only patch updates (x.x.PATCH)
dotnet outdated KMyMoney.Net.sln -u --version-lock Minor

# Only minor updates (x.MINOR.x)
dotnet outdated KMyMoney.Net.sln -u --version-lock Major
```

## Solution-Wide Workflows

### Step 1: Check All Outdated Packages

```bash
cd /home/filejunkie/git/KMyMoney.Net
dotnet outdated KMyMoney.Net.sln
```

This shows a table with:
- **Project**: The project file
- **Id**: Package name
- **Current**: Currently installed version
- **Latest**: Latest stable version available
- **Update**: The version range for update

### Step 2: Dry Run (Preview Updates)

```bash
# Preview what would be updated without making changes
dotnet outdated KMyMoney.Net.sln -u:prompt --dry-run
```

### Step 3: Interactive Update

```bash
# Prompts for each update - recommended for production
dotnet outdated KMyMoney.Net.sln -u:prompt
```

Response options when prompted:
- `y` - Update this package
- `n` - Skip this package
- `a` - Update all remaining packages
- `q` - Quit/skip all remaining

### Step 4: Verify Updates

```bash
# After updating, restore and build
dotnet restore KMyMoney.Net.sln
dotnet build KMyMoney.Net.sln

# Run all tests
dotnet test KMyMoney.Net.sln
```

## Project-Specific Commands

### Check Single Project

```bash
# Core project
dotnet outdated KMyMoney.Net.Core/KMyMoney.Net.Core.csproj

# TelegramBot project
dotnet outdated KMyMoney.Net.TelegramBot/KMyMoney.Net.TelegramBot.csproj

# Test projects
dotnet outdated KMyMoney.Net.Core.Tests/KMyMoney.Net.Core.Tests.csproj
```

### Update Single Project

```bash
# Update only Core project interactively
dotnet outdated KMyMoney.Net.Core/KMyMoney.Net.Core.csproj -u:prompt

# Update only test projects
dotnet outdated KMyMoney.Net.Core.Tests/KMyMoney.Net.Core.Tests.csproj -u
```

## Advanced Features

### Pre-release Versions

```bash
# Include pre-release versions in check
dotnet outdated KMyMoney.Net.sln --pre-release Always

# Options: Auto (default), Always, Never
dotnet outdated KMyMoney.Net.sln --pre-release Auto
```

### Include Auto-References

```bash
# Include auto-referenced packages (like Microsoft.* from SDK)
dotnet outdated KMyMoney.Net.sln -i

# Or with long option
dotnet outdated KMyMoney.Net.sln --include-auto-references
```

### JSON Output

```bash
# Output as JSON for scripting
dotnet outdated KMyMoney.Net.sln --format json > outdated-report.json

# Pretty print JSON
 dotnet outdated KMyMoney.Net.sln --format json | jq .
```

### Filter by Project

```bash
# Only check specific project types
dotnet outdated KMyMoney.Net.sln | grep -E "(Tests|TelegramBot)"
```

## Scripted Workflows

### Update All Test Projects

```bash
#!/bin/bash
cd /home/filejunkie/git/KMyMoney.Net

# Find all test projects
for proj in $(find . -path "*/Tests/*.csproj" -o -name "*Tests.csproj"); do
    echo "Checking $proj..."
    dotnet outdated "$proj" -u:prompt
done
```

### Batch Update All Projects

```bash
#!/bin/bash
cd /home/filejunkie/git/KMyMoney.Net

# Update all projects with patch updates only
dotnet outdated KMyMoney.Net.sln -u --version-lock Minor

# Or for full updates
dotnet outdated KMyMoney.Net.sln -u
```

### Generate Update Report

```bash
#!/bin/bash
cd /home/filejunkie/git/KMyMoney.Net

# Create report directory
mkdir -p reports
date=$(date +%Y%m%d_%H%M%S)
report="reports/outdated_${date}.txt"

# Run check and save report
dotnet outdated KMyMoney.Net.sln > "$report"
echo "Report saved to: $report"

# Count outdated packages
total=$(grep -c "\|" "$report" 2>/dev/null || echo "0")
echo "Total outdated entries: $total"
```

### Automated CI Check (Non-Destructive)

```bash
#!/bin/bash
cd /home/filejunkie/git/KMyMoney.Net

# Check for outdated packages - fail if any found
echo "Checking for outdated packages..."
outdated_count=$(dotnet outdated KMyMoney.Net.sln --format json | jq '.[] | length' 2>/dev/null | paste -sd+ | bc 2>/dev/null || echo "0")

if [ "$outdated_count" -gt "0" ]; then
    echo "::warning::Found $outdated_count outdated package(s)"
    dotnet outdated KMyMoney.Net.sln
    exit 0  # Warning, not error
fi
echo "All packages are up to date"
```

## KMyMoney.Net Specific Patterns

### Common Dependencies to Watch

Based on project structure:

| Package | Projects | Update Frequency |
|---------|----------|-----------------|
| Fractions | Core | Check regularly |
| xunit | All .Tests | Major versions with care |
| Shouldly | All .Tests | Minor/patch OK |
| NSubstitute | All .Tests | Minor/patch OK |
| Telegram.Bot | TelegramBot | Check for breaking changes |
| Dropbox.Api | FileAccessors.Dropbox | Check API compatibility |
| dotnet-etcd | Persistence.Etcd | Check breaking changes |
| Microsoft.NET.Test.Sdk | All .Tests | Patch updates OK |

### Update Strategy by Project Type

**Test Projects (xunit, Shouldly, NSubstitute, Microsoft.NET.Test.Sdk):**
```bash
# Update all test dependencies (minor/patch safe)
dotnet outdated KMyMoney.Net.sln -u --version-lock Minor --include KMyMoney.Net.*Tests*
```

**Core Projects (Fractions, System.*):**
```bash
# Be conservative with core dependencies
dotnet outdated KMyMoney.Net.Core -u:prompt --version-lock Major
```

**TelegramBot Projects:**
```bash
# Check for breaking changes in Telegram.Bot before updating
dotnet outdated KMyMoney.Net.TelegramBot -u:prompt
```

**External Service Projects (Dropbox, Etcd):**
```bash
# Verify API compatibility before updating
dotnet outdated KMyMoney.Net.Core.FileAccessors.Dropbox -u:prompt
dotnet outdated KMyMoney.Net.TelegramBot.Persistence.Etcd -u:prompt
```

## Version Lock Examples

### Conservative Approach (Patch Only)

```bash
# Only apply bug fix updates (x.x.PATCH)
dotnet outdated KMyMoney.Net.sln -u --version-lock Minor

# This ensures no new features or breaking changes
```

### Moderate Approach (Minor Updates)

```bash
# Allow feature updates but not breaking changes (x.MINOR.x)
dotnet outdated KMyMoney.Net.sln -u --version-lock Major

# Good for test packages like Shouldly, NSubstitute
```

### Aggressive Approach (All Updates)

```bash
# Update to latest stable versions
dotnet outdated KMyMoney.Net.sln -u

# Use with caution - may introduce breaking changes
```

## Pre-Update Checklist

Before running updates:

1. **Check current versions**
   ```bash
   dotnet outdated KMyMoney.Net.sln
   ```

2. **Review changelogs** for major updates
   - Visit NuGet package page
   - Check GitHub releases
   - Look for breaking changes

3. **Commit current state**
   ```bash
   git add .
   git commit -m "Before dependency updates"
   ```

4. **Backup project files**
   ```bash
   cp -r /home/filejunkie/git/KMyMoney.Net /tmp/KMyMoney.Net-backup-$(date +%Y%m%d)
   ```

## Post-Update Verification

### After Any Update

```bash
# Restore packages
dotnet restore KMyMoney.Net.sln

# Build all projects
dotnet build KMyMoney.Net.sln --configuration Release

# Run all tests
dotnet test KMyMoney.Net.sln --configuration Release --no-build

# Check for warnings
dotnet build KMyMoney.Net.sln --verbosity normal 2>&1 | grep -i warning
```

### After Major Updates

```bash
# Full clean and rebuild
rm -rf bin obj
dotnet restore KMyMoney.Net.sln
dotnet build KMyMoney.Net.sln --configuration Release

# Run tests multiple times (some tests may be flaky)
for i in 1 2 3; do
    echo "Test run $i..."
    dotnet test KMyMoney.Net.sln --configuration Release --no-build
done

# Check for binding redirect issues
dotnet list KMyMoney.Net.TelegramBot package --include-transitive
```

## Troubleshooting

### Error: "Unable to process the project"

**Cause:** Using `dotnet-outdated` v4.x with .NET 10.0 projects.

**Solution:**
```bash
dotnet tool uninstall --global dotnet-outdated-tool
dotnet tool install --global dotnet-outdated-tool
```

### Error: "No projects found"

**Cause:** Running from wrong directory or solution file not specified.

**Solution:**
```bash
cd /home/filejunkie/git/KMyMoney.Net
dotnet outdated KMyMoney.Net.sln
```

### Error: "exit code: -1" during restore

**Cause:** MSBuild failure, often due to incompatible SDK or TFM.

**Solution:**
```bash
# Check SDK version
dotnet --version

# Ensure all projects target supported .NET version
# Update projects if needed
dotnet outdated --version
```

### Tool Not Found

**Cause:** Tool not installed or not in PATH.

**Solution:**
```bash
# Check if installed
dotnet tool list --global

# Install if missing
dotnet tool install --global dotnet-outdated-tool

# Ensure PATH includes .NET tools
export PATH="$PATH:$HOME/.dotnet/tools"
```

### Slow Performance

**Solution:**
```bash
# Check specific project instead of entire solution
dotnet outdated KMyMoney.Net.Core/KMyMoney.Net.Core.csproj

# Use --verbose to see progress
dotnet outdated KMyMoney.Net.sln --verbose
```

## Best Practices

1. **Always use `-u:prompt` for production updates** - Review each change before applying
2. **Start with test projects** - Less risk of breaking production code
3. **Update one project at a time for critical dependencies** - Easier to rollback if issues arise
4. **Use `--version-lock Minor` for conservative updates** - Only bug fixes
5. **Check GitHub Actions after updates** - Ensure CI passes
6. **Document breaking changes** - Update README or CHANGELOG
7. **Run full test suite after updates** - Even for patch updates
8. **Commit dependency updates separately** - Makes it easier to track what changed

## Command Reference

### Full Option List

```bash
# Show all available options
dotnet outdated --help
```

Key options:
- `-u, --update`: Update packages (add `:prompt` for interactive)
- `--version-lock <value>`: Lock to Major/Minor/None
- `--pre-release <value>`: Auto/Always/Never
- `-i, --include-auto-references`: Include auto-referenced packages
- `--format <value>`: Output format (table/json)
- `--verbose`: Show detailed output
- `--dry-run`: Preview changes without applying

### Examples of All Combinations

```bash
# Check only, table format (default)
dotnet outdated KMyMoney.Net.sln

# Check only, JSON format
dotnet outdated KMyMoney.Net.sln --format json

# Interactive update, prompt for each
dotnet outdated KMyMoney.Net.sln -u:prompt

# Auto-update, patch only
dotnet outdated KMyMoney.Net.sln -u --version-lock Minor

# Auto-update, minor only
dotnet outdated KMyMoney.Net.sln -u --version-lock Major

# Auto-update, all versions
dotnet outdated KMyMoney.Net.sln -u

# Auto-update with pre-release
dotnet outdated KMyMoney.Net.sln -u --pre-release Always

# Auto-update including auto-references
dotnet outdated KMyMoney.Net.sln -u -i
```

## Integration with AGENTS.md Rules

From AGENTS.md:
1. **Do not add any dependencies on your own** - Always ask for permission
2. **You are free to update dependencies' versions** - This tool helps with that
3. **Use test-driven-development approach** - Verify updates with tests

This skill aligns with rule #2 by providing safe, controlled dependency updates.

## Quick Start

```bash
# 1. Ensure tool is up to date
 dotnet tool update --global dotnet-outdated-tool

# 2. Check what's outdated
 dotnet outdated KMyMoney.Net.sln

# 3. Update interactively (recommended)
 dotnet outdated KMyMoney.Net.sln -u:prompt

# 4. Verify
 dotnet build KMyMoney.Net.sln
dotnet test KMyMoney.Net.sln
```
