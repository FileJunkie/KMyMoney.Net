---
name: kmy-tdd
description: Orchestrate Test-Driven Development workflow for KMyMoney.Net
allowed_models: all
---

# KMyMoney.Net TDD Skill

## Purpose
Orchestrate a two-subagent TDD workflow where one agent writes tests from specifications, and another implements code without seeing the tests.

## Activation
Load this skill when you need to:
- Implement a new feature using TDD
- Fix a bug with test-first approach
- Add tests for existing untested code
- Validate user workflows through tests

## Workflow

### Phase 1: Test Specification
**User provides:**
- Feature/bug description
- Acceptance criteria
- Expected behavior
- Edge cases to consider

### Phase 2: Test Writer Subagent
**Input:** Feature description + acceptance criteria
**Output:** Test file(s) in appropriate `.Tests` project

**Process:**
1. Analyze requirements and identify test cases
2. Create test class in correct project (e.g., `KMyMoney.Net.Core.Tests`)
3. Write tests using Shouldly + NSubstitute
4. Use Arrange-Act-Assert pattern
5. Ensure tests compile (but may fail - no implementation yet)
6. Save tests to scratchpad: `/tmp/tdd-tests/{task-id}/`

**Test file template:**
```csharp
// File: KMyMoney.Net.Core.Tests/Services/NewServiceTests.cs
using KMyMoney.Net.Core.Services;
using NSubstitute;
using Shouldly;
using Xunit;

namespace KMyMoney.Net.Core.Tests.Services;

public class NewServiceTests
{
    [Fact]
    public void MethodName_InputCondition_ExpectedResult()
    {
        // Arrange
        var dependency = Substitute.For<IDependency>();
        dependency.Method().Returns(expectedValue);
        var service = new NewService(dependency);

        // Act
        var result = service.MethodUnderTest(input);

        // Assert
        result.ShouldBe(expected);
    }
}
```

### Phase 3: Developer Subagent
**Input:**
- Feature description
- Interface/contract requirements
- File location to implement
- **CANNOT read test source code**

**Output:** Production code that satisfies the contract

**Process:**
1. Read feature description and contract
2. Implement minimal code to satisfy requirements
3. Run `dotnet test` to verify implementation
4. Iterate until all tests pass
5. Do NOT look at test files

### Phase 4: Integration
1. Test subagent's files are reviewed
2. All tests pass with developer's implementation
3. Files merged into main repo
4. Manual verification if needed

## TDD Commands

### For test writer
```bash
# Check what projects exist
ls KMyMoney.Net.*Tests*

# Create new test file
# (Write to scratchpad first)

# Verify test compiles (may fail at runtime)
dotnet build KMyMoney.Net.Core.Tests
```

### For developer
```bash
# Build all projects
dotnet build

# Run all tests
dotnet test

# Run tests for specific project
dotnet test KMyMoney.Net.Core.Tests

# Run with detailed output
dotnet test --verbosity detailed
```

## Contract Definition

When specifying requirements for the developer subagent, provide:

```
## Contract: IFileParser

### Purpose
Parse .kmy files (gzip-compressed XML) into KMyMoney model objects

### Location
File: KMyMoney.Net.Core/Services/KMyMoneyFileParser.cs
Namespace: KMyMoney.Net.Core.Services

### Interface
```csharp
public interface IFileParser
{
    Task<KMyMoneyFile> ParseAsync(Stream fileStream);
}
```

### Requirements
1. Handle gzip decompression
2. Parse XML into model objects
3. Throw specific exceptions for invalid formats
4. Support async operation

### Dependencies
- Uses: KMyMoney.Net.Models
- Uses: System.IO.Compression
- Uses: System.Xml.Serialization
```

## TDD Cycle Commands

### Red Phase (Test Writer)
```bash
# Create test file
# Tests should FAIL (no implementation)
dotnet test --filter "NewFeature" --stoponerror
```

### Green Phase (Developer)
```bash
# Implement minimal code
dotnet build

# Run tests
dotnet test --filter "NewFeature"

# Iterate until PASS
```

### Refactor Phase
```bash
# Both agents collaborate (or single agent)
# Improve code while keeping tests green
dotnet test
```

## Isolation Mechanism

### Test Writer Workspace
- Path: `/tmp/tdd-tests/{task-id}/`
- Contains: Test files only
- Can reference: Production code, Shouldly, NSubstitute, xUnit

### Developer Workspace
- Path: Main repo `/home/filejunkie/git/KMyMoney.Net/`
- Contains: Production code
- CANNOT access: Test files in scratchpad
- Can run: `dotnet test` (tests are in separate projects)

## Example TDD Session

### User Request
> Add a new file accessor for local filesystem access. It should:
> - Implement `IFileAccessor` interface
> - Read files from local filesystem
> - Throw `FileNotFoundException` if file doesn't exist
> - Be in `KMyMoney.Net.Core.FileAccessors` project

### Test Writer Subagent Output
File: `/tmp/tdd-tests/local-accessor/LocalFileAccessorTests.cs`
```csharp
using KMyMoney.Net.Core.FileAccessors;
using Shouldly;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace KMyMoney.Net.Core.FileAccessors.Tests;

public class LocalFileAccessorTests
{
    [Fact]
    public void ReadAsync_ExistingFile_ReturnsFileContent()
    {
        // Arrange
        var filePath = "/tmp/test.kmy";
        var expectedContent = new byte[] { 0x1F, 0x8B }; // gzip magic
        File.WriteAllBytes(filePath, expectedContent);
        
        var accessor = new LocalFileAccessor();

        // Act
        var result = accessor.ReadAsync(filePath).Result;

        // Assert
        result.ShouldBe(expectedContent);
    }

    [Fact]
    public void ReadAsync_NonExistingFile_ThrowsFileNotFoundException()
    {
        // Arrange
        var filePath = "/tmp/nonexistent.kmy";
        var accessor = new LocalFileAccessor();

        // Act & Assert
        Should.Throw<FileNotFoundException>(() => accessor.ReadAsync(filePath).Result);
    }
}
```

### Developer Subagent Input
```
## Contract: LocalFileAccessor

Implement IFileAccessor for local filesystem.

**Interface:**
```csharp
public interface IFileAccessor
{
    Task<byte[]> ReadAsync(string filePath);
    Task WriteAsync(string filePath, byte[] content);
}
```

**Location:**
File: KMyMoney.Net.Core.FileAccessors/LocalFileAccessor.cs
Namespace: KMyMoney.Net.Core.FileAccessors

**Requirements:**
1. ReadAsync: Read file from filesystem, return byte array
2. WriteAsync: Write byte array to filesystem file
3. Throw FileNotFoundException if file doesn't exist on read
4. Create parent directories if they don't exist on write
5. Implement async properly (use File.ReadAllBytesAsync, File.WriteAllBytesAsync)

**Dependencies:**
- .NET 6+ (for async file I/O)
- System.IO
```

### Developer Subagent Output
File: `KMyMoney.Net.Core.FileAccessors/LocalFileAccessor.cs`
```csharp
using System;
using System.IO;
using System.Threading.Tasks;

namespace KMyMoney.Net.Core.FileAccessors;

public class LocalFileAccessor : IFileAccessor
{
    public async Task<byte[]> ReadAsync(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("File not found", filePath);
        }
        
        return await File.ReadAllBytesAsync(filePath);
    }

    public async Task WriteAsync(string filePath, byte[] content)
    {
        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        
        await File.WriteAllBytesAsync(filePath, content);
    }
}
```

## Verification

### Test Writer Verification
```bash
# Verify tests are valid
cd /tmp/tdd-tests/local-accessor
dotnet new classlib -n TempTests
dotnet add package xunit
 dotnet add package Shouldly
# (Copy test file to temp project)
dotnet build
```

### Final Verification
```bash
# In main repo
dotnet build
dotnet test KMyMoney.Net.Core.FileAccessors.Tests
```

## TDD Checklist

- [ ] Feature description is clear
- [ ] Acceptance criteria are defined
- [ ] Test cases cover all requirements
- [ ] Tests cover edge cases
- [ ] Tests use Arrange-Act-Assert
- [ ] Tests use Shouldly assertions
- [ ] Implementation passes all tests
- [ ] No test source code visible to developer
- [ ] Code follows existing patterns
- [ ] All existing tests still pass
