# Test-Driven Development Workflow

This document describes the TDD workflow for KMyMoney.Net, including subagent coordination, isolation mechanisms, and verification procedures.

## Overview

KMyMoney.Net uses a **two-subagent TDD workflow** where:
1. **Test Writer Subagent**: Creates tests from specifications
2. **Developer Subagent**: Implements code without seeing tests

This ensures clean separation and prevents implementation bias from test knowledge.

## Workflow Diagram

```
┌─────────────────────────────────────────────────────────────────┐
│                        TDD Workflow                                  │
├─────────────────────────────────────────────────────────────────┤
│                                                                     │
│  ┌──────────────┐     ┌──────────────┐     ┌─────────────────┐ │
│  │    User      │     │  Test Writer │     │   Developer     │ │
│  │              │────▶│   Subagent   │────▶│    Subagent     │ │
│  │ - Feature    │     │              │     │                 │ │
│  │   spec       │     │ - Receives   │     │ - Receives      │ │
│  │ - Acceptance │     │   spec       │     │   contract      │ │
│  │   criteria   │     │ - Writes     │     │ - Implements    │ │
│  │ - Edge cases │     │   tests      │     │   code          │ │
│  └──────────────┘     │ - Saves to   │     │ - Runs tests    │ │
│                        │   scratchpad │     │ - Iterates      │ │
│                        └──────────────┘     └─────────────────┘ │
│                                    │                            │
│                                    └────────────┬─────────────────┘
│                                                 │
│                                    ┌────────────▼─────────────────┐
│                                    │     Integration & Verification   │
│                                    │  - Review tests                 │
│                                    │  - Merge to repo                │
│                                    │  - Final verification           │
│                                    └──────────────────────────────┘
└─────────────────────────────────────────────────────────────────┘
```

## Step-by-Step Process

### Step 1: User Request

User provides:
- **Feature description** - What needs to be implemented
- **Acceptance criteria** - Conditions that define "done"
- **Expected behavior** - How it should work
- **Edge cases** - Special scenarios to handle
- **Contract** - Interface/method signatures (if applicable)

**Example Request:**
```
Add a new file accessor for local filesystem access.

Acceptance Criteria:
- Implement IFileAccessor interface
- Read files from local filesystem
- Throw FileNotFoundException if file doesn't exist
- Create parent directories if they don't exist on write
- Support async operations

Edge Cases:
- File doesn't exist
- Directory doesn't exist
- Empty file
- Large files (>100MB)
- Concurrent access

Contract:
public interface IFileAccessor
{
    Task<byte[]> ReadAsync(string filePath);
    Task WriteAsync(string filePath, byte[] content);
    Task<bool> ExistsAsync(string filePath);
    Task DeleteAsync(string filePath);
}
```

### Step 2: Test Writer Subagent

**Input:** User request
**Output:** Test file(s) in scratchpad

**Process:**

1. **Analyze requirements**
   - Identify all test cases from acceptance criteria
   - Map edge cases to test scenarios
   - Determine test boundaries

2. **Identify test cases**
   ```
   Test Cases for LocalFileAccessor:
   ✓ ReadAsync_ExistingFile_ReturnsFileContent
   ✓ ReadAsync_NonExistingFile_ThrowsFileNotFoundException
   ✓ WriteAsync_NewFile_CreatesFile
   ✓ WriteAsync_ExistingFile_Overwrites
   ✓ WriteAsync_NonExistingDirectory_CreatesDirectory
   ✓ ExistsAsync_ExistingFile_ReturnsTrue
   ✓ ExistsAsync_NonExistingFile_ReturnsFalse
   ✓ DeleteAsync_ExistingFile_DeletesFile
   ✓ DeleteAsync_NonExistingFile_ThrowsException
   ```

3. **Create test file**
   - Use correct namespace (matching production)
   - Use Shouldly for assertions
   - Use NSubstitute for mocking (if needed)
   - Follow Arrange-Act-Assert pattern
   - Include all identified test cases

4. **Verify test compilation**
   - Tests should compile (but will fail at runtime - no implementation)
   - Use temporary project for validation

5. **Save to scratchpad**
   - Location: `/tmp/tdd-tests/{task-id}/`
   - Structure: Mirror production project structure
   - Do NOT commit to repo yet

**Example Test File:**
```csharp
// File: /tmp/tdd-tests/local-accessor/KMyMoney.Net.Core.FileAccessors.Tests/LocalFileAccessorTests.cs

using KMyMoney.Net.Core.FileAccessors;
using Shouldly;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace KMyMoney.Net.Core.FileAccessors.Tests;

public class LocalFileAccessorTests : IDisposable
{
    private readonly string _testDir;
    
    public LocalFileAccessorTests()
    {
        _testDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_testDir);
    }
    
    public void Dispose()
    {
        try { Directory.Delete(_testDir, true); }
        catch { /* Ignore */ }
    }
    
    [Fact]
    public async Task ReadAsync_ExistingFile_ReturnsFileContent()
    {
        // Arrange
        var filePath = Path.Combine(_testDir, "test.kmy");
        var expectedContent = new byte[] { 0x1F, 0x8B, 0x08, 0x00 };
        await File.WriteAllBytesAsync(filePath, expectedContent);
        
        var accessor = new LocalFileAccessor();

        // Act
        var result = await accessor.ReadAsync(filePath);

        // Assert
        result.ShouldBe(expectedContent);
    }

    [Fact]
    public async Task ReadAsync_NonExistingFile_ThrowsFileNotFoundException()
    {
        // Arrange
        var filePath = Path.Combine(_testDir, "nonexistent.kmy");
        var accessor = new LocalFileAccessor();

        // Act & Assert
        await Should.ThrowAsync<FileNotFoundException>(() => accessor.ReadAsync(filePath));
    }

    [Fact]
    public async Task WriteAsync_NonExistingDirectory_CreatesDirectory()
    {
        // Arrange
        var subDir = Path.Combine(_testDir, "sub", "nested");
        var filePath = Path.Combine(subDir, "newfile.kmy");
        var content = new byte[] { 0x01, 0x02, 0x03 };
        var accessor = new LocalFileAccessor();

        // Act
        await accessor.WriteAsync(filePath, content);

        // Assert
        File.Exists(filePath).ShouldBeTrue();
        Directory.Exists(subDir).ShouldBeTrue();
    }

    [Fact]
    public async Task WriteAsync_ExistingFile_Overwrites()
    {
        // Arrange
        var filePath = Path.Combine(_testDir, "overwrite.kmy");
        var originalContent = new byte[] { 0x01 };
        var newContent = new byte[] { 0x02 };
        await File.WriteAllBytesAsync(filePath, originalContent);
        
        var accessor = new LocalFileAccessor();

        // Act
        await accessor.WriteAsync(filePath, newContent);

        // Assert
        (await File.ReadAllBytesAsync(filePath)).ShouldBe(newContent);
    }

    [Fact]
    public async Task ExistsAsync_ExistingFile_ReturnsTrue()
    {
        // Arrange
        var filePath = Path.Combine(_testDir, "exists.kmy");
        await File.WriteAllBytesAsync(filePath, Array.Empty<byte>());
        var accessor = new LocalFileAccessor();

        // Act
        var result = await accessor.ExistsAsync(filePath);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public async Task ExistsAsync_NonExistingFile_ReturnsFalse()
    {
        // Arrange
        var filePath = Path.Combine(_testDir, "doesnt-exist.kmy");
        var accessor = new LocalFileAccessor();

        // Act
        var result = await accessor.ExistsAsync(filePath);

        // Assert
        result.ShouldBeFalse();
    }
}
```

### Step 3: Developer Subagent

**Input:**
- Feature description (same as user request)
- Contract/interface definition
- File location to implement
- **RESTRICTION: Cannot read test source code**

**Output:** Production code

**Process:**

1. **Understand contract**
   - Read interface definition
   - Understand requirements
   - Identify dependencies

2. **Implement minimal code**
   - Start with simplest possible implementation
   - Handle one requirement at a time
   - Use existing patterns from project

3. **Run tests**
   - Execute `dotnet test` or filtered tests
   - Tests are in separate test projects
   - Cannot see test implementation details

4. **Iterate**
   - If tests fail: Fix implementation
   - If tests pass: Verify all acceptance criteria
   - Refactor if needed

5. **Final verification**
   - All tests pass
   - Code follows existing patterns
   - No violations of AGENTS.md rules

**Example Implementation:**
```csharp
// File: KMyMoney.Net.Core.FileAccessors/LocalFileAccessor.cs

using System;
using System.IO;
using System.Threading.Tasks;

namespace KMyMoney.Net.Core.FileAccessors;

/// <summary>
/// File accessor for local filesystem operations.
/// </summary>
public class LocalFileAccessor : IFileAccessor
{
    /// <inheritdoc />
    public async Task<byte[]> ReadAsync(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("File not found", filePath);
        }
        
        return await File.ReadAllBytesAsync(filePath);
    }

    /// <inheritdoc />
    public async Task WriteAsync(string filePath, byte[] content)
    {
        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        
        await File.WriteAllBytesAsync(filePath, content);
    }

    /// <inheritdoc />
    public Task<bool> ExistsAsync(string filePath) => Task.FromResult(File.Exists(filePath));

    /// <inheritdoc />
    public async Task DeleteAsync(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("File not found", filePath);
        }
        
        File.Delete(filePath);
    }
}
```

### Step 4: Integration

**Process:**

1. **Test subagent verification**
   - Review test file for completeness
   - Ensure all acceptance criteria are covered
   - Verify test patterns match project conventions

2. **Developer subagent verification**
   - Review implementation for correctness
   - Check for AGENTS.md violations
   - Verify code style matches project

3. **Merge files**
   - Move test file from scratchpad to repo
   - Implementation is already in repo
   - Ensure proper file locations

4. **Final test run**
   ```bash
   dotnet build KMyMoney.Net.sln
dotnet test KMyMoney.Net.sln
   ```

5. **Manual verification (if needed)**
   - Check edge cases manually
   - Verify integration with existing code

## Isolation Mechanisms

### Physical Isolation

```
Test Writer Workspace:
  Location: /tmp/tdd-tests/{task-id}/
  Contains: Test files only
  Can access: Production code (read-only), Shouldly, NSubstitute, xUnit
  Cannot access: Developer subagent's implementation

Developer Workspace:
  Location: /home/filejunkie/git/KMyMoney.Net/
  Contains: Production code
  Can access: Test projects (for running tests), production code
  Cannot access: Test writer's scratchpad files
```

### Logical Isolation

1. **No test source visibility**
   - Developer subagent cannot read test files
   - Test files are in separate directory tree
   - Access is blocked at the system level

2. **Separate concerns**
   - Test writer focuses only on test cases
   - Developer focuses only on implementation
   - No collaboration between subagents during development

3. **Contract-based communication**
   - Only the interface/contract is shared
   - No implementation details shared
   - No test logic shared

## Verification Checklists

### Test Writer Checklist

- [ ] All acceptance criteria have corresponding tests
- [ ] All edge cases are covered
- [ ] Tests follow Arrange-Act-Assert pattern
- [ ] Tests use Shouldly assertions
- [ ] Tests use NSubstitute for mocking (if applicable)
- [ ] Tests compile successfully
- [ ] Test names follow convention: Method_State_ExpectedBehavior
- [ ] Tests are in correct namespace
- [ ] Tests use correct project references
- [ ] Test file is saved to scratchpad

### Developer Checklist

- [ ] Implementation satisfies all contract requirements
- [ ] All tests pass
- [ ] Code follows existing patterns
- [ ] One class per file
- [ ] Proper error handling
- [ ] Async/await properly implemented
- [ ] No AGENTS.md violations
- [ ] Code compiles without warnings

### Integration Checklist

- [ ] Test file merged to correct location
- [ ] Implementation file in correct location
- [ ] All existing tests still pass
- [ ] New tests pass
- [ ] No build errors
- [ ] No linting warnings
- [ ] Code review completed (if required)

## TDD Cycle Commands

### Red Phase (Test Writer)
```bash
# Navigate to scratchpad
cd /tmp/tdd-tests/{task-id}

# Create test project structure
mkdir -p KMyMoney.Net.Core.FileAccessors.Tests

# Create test file
# ... write tests ...

# Verify compilation (tests will fail - no implementation)
dotnet build KMyMoney.Net.Core.FileAccessors.Tests
```

### Green Phase (Developer)
```bash
# Navigate to main repo
cd /home/filejunkie/git/KMyMoney.Net

# Build all projects
dotnet build

# Run tests (will fail initially)
dotnet test

# Implement code
# ... write implementation ...

# Run tests again (should pass)
dotnet test

# Iterate until all pass
```

### Refactor Phase (Optional)
```bash
# Both subagents can collaborate here
# Improve code while keeping tests green

# Run full test suite
dotnet test KMyMoney.Net.sln

# Check for improvements
# - Better naming
# - Reduced duplication
# - Improved error messages
# - Performance optimizations
```

## Subagent Coordination

### Test Writer Subagent Instructions

```
YOU ARE THE TEST WRITER SUBAGENT

Your responsibilities:
1. Read the user request carefully
2. Identify all test cases from acceptance criteria
3. Write comprehensive tests for all scenarios
4. Use Shouldly for assertions
5. Use NSubstitute for mocking
6. Follow Arrange-Act-Assert pattern
7. Save tests to /tmp/tdd-tests/{task-id}/
8. DO NOT implement any production code
9. DO NOT read existing implementation (if any)
10. DO NOT communicate with developer subagent

Your output:
- One or more test files
- Tests should compile but may fail at runtime
- All acceptance criteria must be covered
```

### Developer Subagent Instructions

```
YOU ARE THE DEVELOPER SUBAGENT

Your responsibilities:
1. Read the user request and contract
2. Understand the requirements
3. Implement minimal code to satisfy contract
4. Run tests to verify implementation
5. Fix code until all tests pass
6. Follow existing project patterns
7. Respect AGENTS.md rules
8. DO NOT read test source code
9. DO NOT communicate with test writer subagent
10. DO NOT modify tests

Your input:
- User request (feature description)
- Contract (interface/method signatures)
- File location

Your output:
- Production code that satisfies contract
- All tests pass
```

## TDD Configuration

### File Locations

```
Production Code:
  KMyMoney.Net.Core.FileAccessors/LocalFileAccessor.cs

Test Code (during development):
  /tmp/tdd-tests/{task-id}/KMyMoney.Net.Core.FileAccessors.Tests/LocalFileAccessorTests.cs

Test Code (after integration):
  KMyMoney.Net.Core.FileAccessors.Tests/LocalFileAccessorTests.cs
```

### Task ID Generation

Use UUID or timestamp for task identification:
```bash
# Generate task ID
task_id=$(uuidgen)
echo "Task ID: $task_id"

# Or use timestamp
task_id=$(date +%s)
echo "Task ID: $task_id"
```

### Scratchpad Cleanup

```bash
# Clean up old TDD tests
find /tmp/tdd-tests -type d -mtime +7 -exec rm -rf {} + 2>/dev/null

# Or manual cleanup
rm -rf /tmp/tdd-tests/{task-id}
```

## Common TDD Patterns

### Pattern 1: Simple Class Implementation

**Contract:**
```csharp
public interface ICalculator
{
    int Add(int a, int b);
    int Subtract(int a, int b);
}
```

**Tests:**
```csharp
public class CalculatorTests
{
    [Fact] public void Add_TwoPositiveNumbers_ReturnsSum() { ... }
    [Fact] public void Add_NegativeNumbers_ReturnsSum() { ... }
    [Fact] public void Add_Zero_ReturnsOtherNumber() { ... }
    [Fact] public void Subtract_TwoNumbers_ReturnsDifference() { ... }
}
```

**Implementation:**
```csharp
public class Calculator : ICalculator
{
    public int Add(int a, int b) => a + b;
    public int Subtract(int a, int b) => a - b;
}
```

### Pattern 2: Async File Operations

**Contract:**
```csharp
public interface IFileAccessor
{
    Task<byte[]> ReadAsync(string path);
    Task WriteAsync(string path, byte[] content);
}
```

**Tests:**
```csharp
public class FileAccessorTests : IDisposable
{
    private readonly string _tempDir;
    
    public FileAccessorTests() => _tempDir = Path.GetTempPath();
    public void Dispose() => Directory.Delete(_tempDir, true);
    
    [Fact] public async Task ReadAsync_ExistingFile_ReturnsContent() { ... }
    [Fact] public async Task ReadAsync_NotFound_ThrowsException() { ... }
}
```

**Implementation:**
```csharp
public class LocalFileAccessor : IFileAccessor
{
    public async Task<byte[]> ReadAsync(string path) => await File.ReadAllBytesAsync(path);
    public async Task WriteAsync(string path, byte[] content) => await File.WriteAllBytesAsync(path, content);
}
```

### Pattern 3: Service with Dependencies

**Contract:**
```csharp
public interface ITransactionService
{
    Task<Transaction> GetByIdAsync(string id);
    Task<IEnumerable<Transaction>> GetByAccountAsync(string accountId);
}
```

**Tests (with mocking):**
```csharp
public class TransactionServiceTests
{
    private readonly ITransactionRepository _repo = Substitute.For<ITransactionRepository>();
    private readonly TransactionService _service;
    
    public TransactionServiceTests() => _service = new TransactionService(_repo);
    
    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsTransaction()
    {
        // Arrange
        var expected = new Transaction { Id = "TXN-1" };
        _repo.GetByIdAsync("TXN-1").Returns(expected);
        
        // Act
        var result = await _service.GetByIdAsync("TXN-1");
        
        // Assert
        result.ShouldBe(expected);
    }
}
```

**Implementation:**
```csharp
public class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _repo;
    
    public TransactionService(ITransactionRepository repo) => _repo = repo;
    
    public async Task<Transaction> GetByIdAsync(string id) => await _repo.GetByIdAsync(id);
    public async Task<IEnumerable<Transaction>> GetByAccountAsync(string accountId) => 
        await _repo.GetByAccountAsync(accountId);
}
```

## Error Handling in TDD

### When Tests Don't Pass

1. **Test writer phase**
   - Expected: Tests should fail (no implementation)
   - If tests pass: Something is wrong with tests

2. **Developer phase**
   - If tests fail: Fix implementation
   - If tests pass but shouldn't: Tests are incomplete

3. **Common issues**
   - Wrong namespace: Fix import statements
   - Missing dependency: Add NuGet package or project reference
   - Compilation error: Fix syntax
   - Runtime error: Handle edge cases

### Debugging Tips

**For Test Writer:**
```bash
# Check if test compiles
dotnet build TestProject

# Check test discovery
dotnet test --list-tests

# Run single test
dotnet test --filter "FullyQualifiedName=Namespace.ClassName.MethodName"
```

**For Developer:**
```bash
# Run tests with verbose output
dotnet test --verbosity detailed

# Run tests without building
dotnet test --no-build

# Stop on first failure
dotnet test --stoponerror
```

## Performance Considerations

### Test Execution

- Use `--no-build` for faster test runs during development
- Use `--parallel none` if tests are not thread-safe
- Filter tests when working on specific functionality
- Use watch mode for iterative development: `dotnet watch test`

### Test Isolation

- Each test should be independent
- Use `IDisposable` for cleanup
- Avoid static state
- Use fresh instances for each test

## Best Practices

### Test Writer
1. **Be thorough** - Cover all edge cases
2. **Be clear** - Test names should describe behavior
3. **Be specific** - Each test should test one thing
4. **Use patterns** - Follow existing test patterns
5. **Test behavior, not implementation** - Focus on what, not how

### Developer
1. **Start simple** - Implement minimal working code
2. **Test frequently** - Run tests after each change
3. **Follow patterns** - Match existing code style
4. **Keep it clean** - Refactor when needed
5. **Respect contracts** - Implement interface exactly as defined

### Both
1. **Communicate clearly** - Document assumptions
2. **Stay focused** - One feature at a time
3. **Verify completely** - All tests must pass
4. **Follow rules** - Respect AGENTS.md
5. **Document** - Add comments where needed

## Glossary

| Term | Definition |
|------|------------|
| TDD | Test-Driven Development: Test first, then implement |
| Red Phase | Tests are written and failing (no implementation) |
| Green Phase | Implementation makes tests pass |
| Refactor Phase | Improve code while keeping tests green |
| Contract | Interface or method signature to implement |
| Subagent | Specialized AI agent with specific role |
| Scratchpad | Temporary workspace for intermediate files |
| Integration | Merging test and implementation into repo |
