# Ideas Backlog

This document contains all proposed ideas for improving the KMyMoney.Net development process, organized by category and priority.

## Legend

- **Priority**: 🔥 High | ⚡ Medium | 📌 Low
- **Status**: ⬜ Not Started | 🟡 In Progress | ✅ Done | ❌ Rejected
- **Effort**: ⭐ Easy | ⭐⭐ Medium | ⭐⭐⭐ Hard

---

## Skills (✅ Implemented - See `.vibe/skills/`)

| Idea | Priority | Status | Effort | Owner |
|------|----------|--------|--------|-------|
| `kmy-test` - Test execution and management | 🔥 | ✅ | ⭐ | - |
| `kmy-tdd` - TDD workflow orchestration | 🔥 | ✅ | ⭐⭐ | - |
| `kmy-deps` - Dependency management | ⚡ | ✅ | ⭐ | - |
| `kmy-build` - Build and packaging | ⚡ | ✅ | ⭐⭐ | - |
| `kmy-analyze` - Static analysis | ⚡ | ✅ | ⭐⭐ | - |
| `kmy-domain` - Domain knowledge | 🔥 | ✅ | ⭐⭐⭐ | - |

---

## TDD Infrastructure

| Idea | Priority | Status | Effort | Owner | Notes |
|------|----------|--------|--------|-------|-------|
| Automated test-writer subagent | 🔥 | ⬜ | ⭐⭐⭐ | - | AI that generates tests from spec |
| Automated developer subagent | 🔥 | ⬜ | ⭐⭐⭐ | - | AI that implements from contract |
| TDD workflow coordinator | 🔥 | ⬜ | ⭐⭐⭐ | - | Orchestrates subagents |
| Test file isolation system | ⚡ | ⬜ | ⭐⭐ | - | Prevents dev from seeing tests |
| Contract validation tool | ⚡ | ⬜ | ⭐⭐ | - | Validates contracts before implementation |
| TDD metrics dashboard | 📌 | ⬜ | ⭐⭐⭐ | - | Tracks TDD adoption, coverage |

### Details: Automated Test-Writer Subagent

**Description**: AI subagent that generates comprehensive test suites from feature specifications.

**Capabilities**:
- Parse feature description and acceptance criteria
- Generate test cases for all scenarios
- Use Shouldly assertions
- Use NSubstitute for mocking
- Follow Arrange-Act-Assert pattern
- Generate tests that compile

**Input**:
```
Feature: LocalFileAccessor
Description: Implement file accessor for local filesystem
Acceptance Criteria:
- Implement IFileAccessor
- Read files asynchronously
- Throw FileNotFoundException if not found
- Create directories if they don't exist
Edge Cases:
- File doesn't exist
- Directory doesn't exist
- Empty file
- Large file
```

**Output**:
```csharp
// Complete test class with all test cases
public class LocalFileAccessorTests
{
    [Fact] public void ReadAsync_ExistingFile_ReturnsContent() { ... }
    [Fact] public void ReadAsync_NotFound_ThrowsFileNotFoundException() { ... }
    // ... all edge cases covered
}
```

### Details: TDD Workflow Coordinator

**Description**: Master controller that manages the TDD process with subagents.

**Workflow**:
```
1. Receive user request
2. Spawn test-writer subagent
3. Wait for tests to be created
4. Spawn developer subagent
5. Provide contract to developer
6. Wait for implementation
7. Run tests
8. If pass: integrate
9. If fail: provide feedback to developer
10. Iterate until complete
```

**Features**:
- Progress tracking
- Error reporting
- Timeout handling
- Resource cleanup

---

## Code Quality Automation

| Idea | Priority | Status | Effort | Owner | Notes |
|------|----------|--------|--------|-------|-------|
| Pre-commit hooks | ⚡ | ⬜ | ⭐ | - | Auto-run tests on git add |
| Roslyn analyzers | ⚡ | ⬜ | ⭐⭐ | - | Enforce one-class-per-file |
| AGPL-3.0 license checker | ⚡ | ⬜ | ⭐ | - | Verify license headers |
| Dependency update bot | 📌 | ⬜ | ⭐⭐ | - | Auto-PR for updates |
| Code review bot | 📌 | ⬜ | ⭐⭐⭐ | - | Auto-review PRs |

### Details: Pre-commit Hooks

**Implementation**: Git hook that runs on `git add` or before commit

**Hook content** (`.git/hooks/pre-commit`):
```bash
#!/bin/bash

echo "Running pre-commit checks..."

# Check for AGENTS.md violations
.vibe/skills/kmy-analyze/analyze.sh

# Run tests for changed files
git diff --cached --name-only | grep -E '\.cs$' | while read file; do
    # Determine which test project to run
    # Run relevant tests
    dotnet test --filter "..."
done

echo "Pre-commit checks complete"
```

### Details: Roslyn Analyzer for One-Class-Per-File

**Implementation**: Custom Roslyn analyzer that enforces AGENTS.md rule

**Analyzer code**:
```csharp
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class OneClassPerFileAnalyzer : DiagnosticAnalyzer
{
    public static DiagnosticDescriptor Rule = new DiagnosticDescriptor(
        "KMM001",
        "One class per file",
        "File {0} contains multiple top-level classes",
        "Design",
        DiagnosticSeverity.Error,
        true);
    
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => 
        ImmutableArray.Create(Rule);
    
    public override void Initialize(AnalysisContext context)
    {
        context.RegisterSyntaxTreeAction(ctx =>
        {
            var classes = ctx.Tree.Root.DescendantNodes().OfType<ClassDeclarationSyntax>();
            var interfaces = ctx.Tree.Root.DescendantNodes().OfType<InterfaceDeclarationSyntax>();
            
            if (classes.Count() + interfaces.Count() > 1)
            {
                ctx.ReportDiagnostic(Diagnostic.Create(
                    Rule,
                    Location.None,
                    ctx.Tree.FilePath));
            }
        });
    }
}
```

---

## Domain-Specific Tooling

| Idea | Priority | Status | Effort | Owner | Notes |
|------|----------|--------|--------|-------|-------|
| .kmy file validator CLI | ⚡ | ⬜ | ⭐⭐ | - | Validate .kmy files |
| Synthetic .kmy test data generator | ⚡ | ⬜ | ⭐⭐ | - | Generate test files |
| Dropbox API mock server | 📌 | ⬜ | ⭐⭐⭐ | - | Local testing |
| KMyMoney XML schema validator | 📌 | ⬜ | ⭐⭐ | - | Schema validation |
| Transaction import/export tool | 📌 | ⬜ | ⭐⭐ | - | CSV, OFX import |

### Details: .kmy File Validator CLI

**Command**: `kmymoney validate [file]`

**Features**:
- Check gzip header
- Validate XML structure
- Check for required elements
- Report validation errors
- Suggest fixes

**Implementation**:
```csharp
public class KMyMoneyValidator
{
    public ValidationResult Validate(Stream stream)
    {
        // Check gzip
        // Parse XML
        // Validate structure
        // Return result
    }
}
```

### Details: Synthetic .kmy Test Data Generator

**Command**: `kmymoney generate [options]`

**Options**:
```
--accounts N       Number of accounts (default: 5)
--transactions N  Number of transactions (default: 100)
--date-range      Date range for transactions
--seed           Random seed for reproducibility
--output         Output file path
```

**Implementation**:
```csharp
public class TestDataGenerator
{
    public KMyMoneyFile Generate(GeneratorOptions options)
    {
        var random = new Random(options.Seed);
        
        var accounts = GenerateAccounts(random, options.AccountCount);
        var transactions = GenerateTransactions(random, options.TransactionCount, accounts);
        
        return new KMyMoneyFile
        {
            Information = new FileInformation { Name = "test.kmy", Version = "1.0" },
            Accounts = accounts,
            Transactions = transactions
        };
    }
}
```

---

## Development Workflow Enhancements

| Idea | Priority | Status | Effort | Owner | Notes |
|------|----------|--------|--------|-------|-------|
| Feature branch automation | 📌 | ⬜ | ⭐⭐ | - | Auto-create branches |
| Standardized commit templates | 📌 | ⬜ | ⭐ | - | Commit message guides |
| Impact analysis tool | ⚡ | ⬜ | ⭐⭐ | - | Show affected files |
| Session-based work tracking | ⚡ | ⬜ | ⭐⭐ | - | Track changes per session |
| Change request templates | 📌 | ⬜ | ⭐ | - | Standardized PR format |

### Details: Feature Branch Automation

**Workflow**:
```
User: /new-feature 59-add-local-accessor

Bot:
1. Check if issue #59 exists
2. Create branch: feature/59-add-local-accessor
3. Checkout branch
4. Create directory for tracking
5. Open editor with template
```

**Template** (`.vibe/templates/feature.md`):
```markdown
# Feature: {feature-name}

## Issue
{issue-link}

## Description
{description}

## Acceptance Criteria
- [ ] {criteria-1}
- [ ] {criteria-2}

## Tasks
- [ ] Implement {task-1}
- [ ] Add tests for {task-1}
- [ ] Document {task-1}

## Notes
{notes}
```

### Details: Impact Analysis Tool

**Command**: `kmymoney impact [file]`

**Output**:
```
Analyzing changes to: KMyMoney.Net.Core.Services.KMyMoneyParser

Affected files:
  ✓ KMyMoney.Net.Core.Tests/Services/KMyMoneyParserTests.cs (test file)
  ✓ KMyMoney.Net.TelegramBot/Services/BotService.cs (uses parser)
  ✓ KMyMoney.Net.Cli/Program.cs (uses parser)

Affected tests:
  ✓ KMyMoneyParserTests.ParseKmyFile_ValidInput_ReturnsModel
  ✓ KMyMoneyParserTests.ParseKmyFile_InvalidInput_ThrowsException
  ✓ BotServiceTests.GetAccounts_ValidFile_ReturnsAccounts

Dependencies:
  ✓ KMyMoney.Net.Models (project reference)
  ✓ System.IO.Compression (NuGet package)
  ✓ System.Xml.Serialization (NuGet package)
```

---

## Testing Strategy

| Idea | Priority | Status | Effort | Owner | Notes |
|------|----------|--------|--------|-------|-------|
| Integration test suite | ⚡ | ⬜ | ⭐⭐⭐ | - | End-to-end Telegram flows |
| Contract tests | ⚡ | ⬜ | ⭐⭐ | - | Verify implementations |
| Property-based tests | 📌 | ⬜ | ⭐⭐⭐ | - | For parsing, gzip logic |
| Performance benchmarks | 📌 | ⬜ | ⭐⭐ | - | For large .kmy files |
| Mutation testing | 📌 | ⬜ | ⭐⭐⭐ | - | Test quality measurement |
| Test coverage visualization | ⚡ | ⬜ | ⭐⭐ | - | HTML/Markdown reports |

### Details: Integration Test Suite

**Project**: `KMyMoney.Net.TelegramBot.IntegrationTests`

**Test scenarios**:
```
1. User authentication flow
   - /start → /login → OAuth → /list
   
2. File selection flow
   - /list → /select → /accounts
   
3. Transaction management flow
   - /accounts → /add_transaction → input → confirmation
   
4. Error handling flow
   - Invalid command → Error message
   - Missing file → Prompt to select
   - Parse error → Show error details
```

**Test infrastructure**:
- Mock Telegram API
- Mock Dropbox API
- In-memory persistence for testing
- Test data generator

### Details: Contract Tests

**Approach**: Verify that implementations satisfy their contracts

**Example**:
```csharp
// Contract: IFileAccessor
public abstract class FileAccessorContractTests
{
    protected abstract IFileAccessor CreateAccessor();
    
    [Fact]
    public async Task ReadAsync_ExistingFile_ReturnsContent()
    {
        var accessor = CreateAccessor();
        // ... test implementation
    }
    
    [Fact]
    public async Task ReadAsync_NonExistingFile_ThrowsException()
    {
        var accessor = CreateAccessor();
        // ... test implementation
    }
}

// Implementation test
public class DropboxFileAccessorContractTests : FileAccessorContractTests
{
    protected override IFileAccessor CreateAccessor() => new DropboxFileAccessor(...);
}

public class LocalFileAccessorContractTests : FileAccessorContractTests
{
    protected override IFileAccessor CreateAccessor() => new LocalFileAccessor();
}
```

---

## Documentation

| Idea | Priority | Status | Effort | Owner | Notes |
|------|----------|--------|--------|-------|-------|
| Architecture Decision Records (ADRs) | ⚡ | ⬜ | ⭐ | - | Document major decisions |
| API documentation generator | ⚡ | ⬜ | ⭐⭐ | - | From XML comments |
| Change log automation | ⚡ | ⬜ | ⭐ | - | From commit messages |
| Contributing guide | 📌 | ⬜ | ⭐⭐ | - | For new contributors |
| Developer onboarding | 📌 | ⬜ | ⭐⭐ | - | Quick start guide |

### Details: ADR Template

**Location**: `docs/adr/`

**Template** (`docs/adr/template.md`):
```markdown
# ADR-{number}: {Title}

## Status
{Proposed | Accepted | Rejected | Superseded}

## Context
{The problem being addressed}

## Decision
{The chosen solution}

## Alternatives Considered
{Other options that were rejected}

## Consequences
{Positive and negative outcomes}

## Related
- {links to issues, PRs, discussions}
```

### Details: API Documentation Generator

**Command**: `kmymoney docs`

**Features**:
- Extract XML comments from code
- Generate Markdown documentation
- Include code examples
- Generate type hierarchy
- Generate module index

**Implementation**:
```csharp
public class ApiDocGenerator
{
    public void Generate(string outputDir)
    {
        // Parse all .cs files
        // Extract XML comments
        // Generate Markdown files
        // Copy to output directory
    }
}
```

---

## Additional Ideas

| Idea | Priority | Status | Effort | Owner | Notes |
|------|----------|--------|--------|-------|-------|
| Vibe skill for project analysis | ⚡ | ⬜ | ⭐⭐⭐ | - | Auto-analyze project |
| AI pair programming mode | 📌 | ⬜ | ⭐⭐⭐ | - | Real-time collaboration |
| Code review assistant | ⚡ | ⬜ | ⭐⭐⭐ | - | Suggest improvements |
| Dependency visualization | 📌 | ⬜ | ⭐⭐ | - | Graph of dependencies |
| Project health dashboard | 📌 | ⬜ | ⭐⭐⭐ | - | Metrics and status |

---

## Prioritization Matrix

### Next (High Priority, Low Effort)
1. Pre-commit hooks
2. Roslyn analyzers
3. .kmy file validator CLI
4. Synthetic test data generator

### Soon (High Priority, Medium Effort)
1. Automated test-writer subagent
2. Automated developer subagent
3. TDD workflow coordinator
4. Contract tests

### Later (Medium Priority)
1. Integration test suite
2. Code quality automation
3. Documentation generators
4. Dependency visualization

### Backlog (Low Priority)
1. Property-based tests
2. Performance benchmarks
3. Mutation testing
4. AI pair programming

---

## Decision Log

| Date | Decision | Rationale |
|------|----------|-----------|
| 2026-09-10 | Implemented 6 core skills | Foundation for all other work |
| 2026-09-10 | Prioritize TDD infrastructure | Aligns with AGENTS.md requirements |
| 2026-09-10 | Defer mutation testing | High effort, lower immediate value |

---

## How to Contribute

1. **Review** existing ideas
2. **Vote** on priorities (add emoji reactions)
3. **Add** new ideas to the appropriate section
4. **Claim** an idea by adding your name as owner
5. **Implement** and update status
6. **Document** decisions in the decision log

---

## Templates

### New Idea Template

```markdown
| Idea | Priority | Status | Effort | Owner | Notes |
|------|----------|--------|--------|-------|-------|
| [Idea name] | [🔥/⚡/📌] | ⬜ | [⭐/⭐⭐/⭐⭐⭐] | [name] | [details] |

**Description**: [Detailed description]

**Benefits**: [Why this is valuable]

**Implementation Notes**: [Any specific requirements]

**Dependencies**: [What needs to exist first]
```

### Idea Detail Template

```markdown
### Details: [Idea Name]

**Description**: [Longer description]

**Use Cases**:
- [Use case 1]
- [Use case 2]

**Implementation Approach**:
```[code language]
[example code]
```

**Success Criteria**:
- [ ] [Criteria 1]
- [ ] [Criteria 2]

**Out of Scope**:
- [What this does NOT include]

**Open Questions**:
- [Question 1?]
- [Question 2?]
```
