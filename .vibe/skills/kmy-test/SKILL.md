---
name: kmy-test
description: Execute and manage tests for KMyMoney.Net projects using dotnet test
allowed_models: all
---

# KMyMoney.Net Test Skill

## Purpose
Run, filter, and analyze unit tests across the KMyMoney.Net solution using `dotnet test`.

## Activation
Load this skill when you need to:
- Run all or specific test projects
- Filter tests by name, class, or trait
- Check test coverage
- Debug failing tests
- Run tests in watch mode

## Commands

### Run all tests
```bash
dotnet test KMyMoney.Net.sln
```

### Run specific test project
```bash
dotnet test KMyMoney.Net.Core.Tests/KMyMoney.Net.Core.Tests.csproj
dotnet test KMyMoney.Net.TelegramBot.Tests/KMyMoney.Net.TelegramBot.Tests.csproj
```

### Run tests with filter
```bash
# Filter by test name (full or partial)
dotnet test --filter "FullyQualifiedName~KMyMoney.Net.Core.KMyMoneyService"

# Filter by test class
dotnet test --filter "FullyQualifiedName~KMyMoney.Net.Core.Tests.KMyMoneyServiceTests"

# Filter by test method
dotnet test --filter "Name~ParseKmyFile"

# Multiple filters (AND logic)
dotnet test --filter "FullyQualifiedName~KMyMoney.Net.Core & Name~Parse"
```

### Run tests with specific configuration
```bash
# Debug configuration (default)
dotnet test --configuration Debug

# Release configuration
dotnet test --configuration Release

# Verbose output
dotnet test --verbosity normal
dotnet test --verbosity detailed
```

### Test coverage
```bash
# Install coverlet collector if needed
dotnet add package coverlet.collector

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"

# Generate coverage report (requires coverlet.console)
dotnet tool install --global coverlet.console
dotnet coverlet KMyMoney.Net.Core.Tests --target "dotnet" --targetargs "test KMyMoney.Net.Core.Tests --no-build"
```

### Run tests in watch mode
```bash
dotnet watch test
```

### Parallel test execution
```bash
# Disable parallelization
dotnet test --parallel none

# Limit parallel threads
dotnet test --parallel 4
```

### Run tests without building
```bash
dotnet test --no-build
```

### Run tests and stop on first failure
```bash
dotnet test --stoponerror
```

## Common Test Patterns

### Test naming convention
Tests follow the pattern: `MethodUnderTest_StateUnderTest_ExpectedBehavior`

Example: `ParseKmyFile_ValidGzipXml_ReturnsKmyModel`

### Test structure (Arrange-Act-Assert)
```csharp
[Fact]
public void MethodUnderTest_StateUnderTest_ExpectedBehavior()
{
    // Arrange
    var input = CreateTestInput();
    var service = new ServiceUnderTest();

    // Act
    var result = service.MethodUnderTest(input);

    // Assert
    result.ShouldNotBeNull();
    result.Property.ShouldBe(expectedValue);
}
```

## Project-Specific Test Commands

### Core tests
```bash
dotnet test KMyMoney.Net.Core.Tests
```

### File Accessors Dropbox tests
```bash
dotnet test KMyMoney.Net.Core.FileAccessors.Dropbox.Tests
```

### TelegramBot tests
```bash
dotnet test KMyMoney.Net.TelegramBot.Tests
```

### Persistence tests
```bash
dotnet test KMyMoney.Net.TelegramBot.Persistence.Etcd.Tests
dotnet test KMyMoney.Net.TelegramBot.Persistence.InMemory.Tests
```

### All tests
```bash
dotnet test
```

## Debugging Tests

### Run single test
```bash
dotnet test --filter "FullyQualifiedName=KMyMoney.Net.Core.Tests.KMyMoneyServiceTests.ParseKmyFile_ValidInput_ReturnsModel"
```

### Get detailed test output
```bash
dotnet test --verbosity detailed --logger "console;verbosity=detailed"
```

### Run tests with debugger attached
```bash
dotnet test --debug
```

## Environment Setup

Ensure the following NuGet packages are available:
- `xunit` (test framework)
- `xunit.runner.visualstudio` (test runner)
- `Shouldly` (assertions)
- `NSubstitute` (mocking)
- `coverlet.collector` (coverage, optional)

## Best Practices
1. Always run tests before committing
2. Use `--no-build` for faster test runs during development
3. Filter tests when working on specific functionality
4. Use watch mode for iterative development
5. Check test output for warnings, not just failures
