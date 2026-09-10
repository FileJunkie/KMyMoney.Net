# Quick Start Guide for AI Agents

This guide helps AI agents (like Mistral Vibe) get started quickly with KMyMoney.Net development.

---

## Step 1: Understand the Project

**Read these files first**:
1. `AGENTS.md` - **CRITICAL**: Project rules and restrictions
2. `CLAUDE.md` - Additional context (note: Claude is US-based and prohibited)
3. `README.md` - Project overview
4. `LICENSE` - AGPL-3.0 license

**Key facts**:
- **Language**: C# with latest stable .NET
- **Type**: Telegram bot for editing KMyMoney `.kmy` files
- **File format**: `.kmy` = gzip-compressed XML
- **Architecture**: Layered (TelegramBot → Core → FileAccessors → External)
- **License**: AGPL-3.0 (all contributions must use this license)

---

## Step 2: Load Vibe Skills

Load the project-specific skills for context:

```
/load kmy-domain    # Domain knowledge (.kmy format, architecture)
/load kmy-test      # Test execution commands
/load kmy-tdd       # TDD workflow (if doing test-first)
/load kmy-deps      # Dependency management
/load kmy-build     # Build and packaging
/load kmy-analyze   # Static analysis and compliance
```

Or use explicit skill command:
```
skill kmy-domain
skill kmy-test
```

---

## Step 3: Verify Environment

Check that you have the prerequisites:

```bash
# .NET SDK
which dotnet
dotnet --version

# Git
git --version

# Project builds
dotnet restore KMyMoney.Net.sln
dotnet build KMyMoney.Net.sln

# Tests run
dotnet test KMyMoney.Net.sln
```

---

## Step 4: Common Tasks

### Run Tests
```
# All tests
dotnet test

# Specific project
dotnet test KMyMoney.Net.Core.Tests

# Filter by name
dotnet test --filter "Name~Parser"

# With coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Build Project
```
# Build all (Debug)
dotnet build

# Build Release
dotnet build --configuration Release

# Build single project
dotnet build KMyMoney.Net.Core
```

### Check Dependencies
```
# List all
dotnet list package --include-transitive

# Check for updates
dotnet outdated KMyMoney.Net.sln

# Update a package
dotnet add KMyMoney.Net.Core package Shouldly --version latest
```

---

## Step 5: Development Workflow

### For Bug Fixes
1. Find the failing test or create a reproduction
2. Identify the root cause
3. Implement minimal fix
4. Verify tests pass
5. Check for AGENTS.md violations

### For New Features (TDD)
1. Write tests first (load `kmy-tdd` skill)
2. Save tests to scratchpad
3. Implement code
4. Run tests
5. Iterate until pass
6. Merge tests to repo

### For Dependency Updates
1. Check current versions (`kmy-deps` skill)
2. Review changelogs
3. Update in test project first
4. Run tests
5. Update in all projects
6. Verify full build

---

## Step 6: Important Rules (from AGENTS.md)

### ❌ NEVER DO
- Use US-based LLMs (Claude, GPT, etc.)
- Execute git commands without permission
- Add dependencies without permission
- Violate AGPL-3.0 license
- Ignore AGENTS.md rules

### ✅ ALWAYS DO
- Use TDD approach
- One class per file
- Minimal changes
- Match existing patterns
- Run tests before committing
- Update existing dependencies to latest stable

### ⚠️ ASK FIRST
- Before major refactoring
- Before adding new dependencies
- Before pushing to remote
- Before force-pushing

---

## Common Patterns

### File Structure
```
KMyMoney.Net/
├── Models/           # Data models
├── Core/            # Business logic
├── FileAccessors/   # File I/O interfaces
├── TelegramBot/     # Bot logic
├── Persistence/     # Storage interfaces
└── Tests/           # Unit tests
```

### Class Naming
- PascalCase for classes/interfaces
- Interfaces start with `I` (e.g., `IFileAccessor`)
- Test classes end with `Tests` (e.g., `FileAccessorTests`)
- One class per file

### Test Pattern
```csharp
[Fact]
public void Method_State_ExpectedBehavior()
{
    // Arrange
    var input = ...;
    var service = new Service();

    // Act
    var result = service.Method(input);

    // Assert
    result.ShouldBe(expected);
}
```

---

## Troubleshooting

### Tests Failing
```bash
# Run with verbose output
dotnet test --verbosity detailed

# Stop on first failure
dotnet test --stoponerror

# List all tests
dotnet test --list-tests
```

### Build Errors
```bash
# Check dependencies
dotnet restore --force

# Clear cache
dotnet nuget locals all -c

# Detailed build output
dotnet build --verbosity detailed
```

### Missing Files
```bash
# Check project references
dotnet list KMyMoney.Net.TelegramBot reference

# Check file exists
ls -la KMyMoney.Net.Core/Services/
```

---

## Useful Queries

### Find Files
```bash
# Find all C# files
find . -name "*.cs" -type f

# Find test files
find . -path "*/Tests/*" -name "*.cs"

# Find by class name
grep -r "class KMyMoneyParser" . --include="*.cs"
```

### Check AGENTS.md Compliance
```bash
# One class per file check
.vibe/skills/kmy-analyze/analyze.sh

# Find multiple classes in one file
grep -r "public class" . --include="*.cs" | cut -d: -f1 | sort | uniq -c | grep -v "1 "
```

---

## Resources

### Documentation
- `.vibe/TDD_WORKFLOW.md` - Detailed TDD process
- `.vibe/IDEAS_BACKLOG.md` - All proposed improvements
- `.vibe/PROJECT_ANALYSIS.md` - Full project analysis
- `.vibe/skills/kmy-domain/SKILL.md` - Domain knowledge

### External
- [AGPL-3.0 License](https://www.gnu.org/licenses/agpl-3.0.html)
- [.NET Documentation](https://learn.microsoft.com/en-us/dotnet/)
- [xUnit Documentation](https://xunit.net/)
- [Shouldly Documentation](https://github.com/shouldly/shouldly)
- [NSubstitute Documentation](https://nsubstitute.github.io/)

---

## Quick Commands Reference

| Task | Command |
|------|---------|
| Build all | `dotnet build KMyMoney.Net.sln` |
| Run all tests | `dotnet test` |
| Test specific project | `dotnet test KMyMoney.Net.Core.Tests` |
| Build Release | `dotnet build --configuration Release` |
| Clean | `dotnet clean` |
| Restore packages | `dotnet restore` |
| List dependencies | `dotnet list package` |
| Check updates | `dotnet outdated KMyMoney.Net.sln` |

---

## Getting Help

If unsure:
1. **Check AGENTS.md** - Most questions are answered there
2. **Check existing code** - Match existing patterns
3. **Check skills** - Load relevant Vibe skill
4. **Ask user** - Clarify requirements before acting

---

*This quick start guide is designed for AI agents working on KMyMoney.Net. Last updated: 2026-09-10*
