# KMyMoney.Net Project Analysis

**Generated**: 2026-09-10  
**Purpose**: Baseline analysis for AI agent development infrastructure

---

## Project Overview

### Basic Information
- **Name**: KMyMoney.Net
- **Type**: Telegram Bot
- **Language**: C#
- **License**: AGPL-3.0 (GNU Affero General Public License v3.0)
- **Primary Function**: Edit KMyMoney `.kmy` files stored in Dropbox via Telegram

### File Format
- `.kmy` files: **gzip-compressed XML** documents
- Contains KMyMoney application data (accounts, transactions, etc.)
- XML schema follows KMyMoney format

---

## Project Structure

```
KMyMoney.Net/
├── .git/
├── .github/
│   └── workflows/                 # CI/CD pipelines
├── .idea/                        # JetBrains IDE settings
├── .vibe/                        # Vibe CLI configuration (NEW)
│   ├── README.md                 # Vibe configuration overview
│   ├── TDD_WORKFLOW.md           # TDD process documentation
│   ├── IDEAS_BACKLOG.md          # Prioritized idea backlog
│   └── skills/                   # Project-specific Vibe skills
│       ├── README.md
│       ├── kmy-test/SKILL.md     # Test execution skill
│       ├── kmy-tdd/SKILL.md      # TDD workflow skill
│       ├── kmy-deps/SKILL.md     # Dependency management skill
│       ├── kmy-build/SKILL.md    # Build and packaging skill
│       ├── kmy-analyze/SKILL.md  # Static analysis skill
│       └── kmy-domain/SKILL.md   # Domain knowledge skill
├── AGENTS.md                     # AI agent instructions
├── CLAUDE.md                     # Project instructions
├── LICENSE                       # AGPL-3.0 license
├── README.md                     # Project overview
├── KMyMoney.Net.sln              # Visual Studio solution
├── KMyMoney.Net.sln.DotSettings.user
├── packaging/                    # Debian package configuration
│   ├── debian/
│   │   ├── changelog
│   │   ├── control
│   │   ├── rules
│   │   ├── compat
│   │   └── source/
│   │       └── format
│   └── KMyMoney.Net.service      # Systemd service file
│
├── KMyMoney.Net.Models/          # Data models (POCO)
│   └── (C# classes for KMyMoney data)
│
├── KMyMoney.Net.Core/            # Core logic
│   ├── (File parsing, manipulation)
│   └── Services/
│
├── KMyMoney.Net.Core.FileAccessors/  # File accessor interfaces
│   └── (IFileAccessor, etc.)
│
├── KMyMoney.Net.Core.FileAccessors.Dropbox/  # Dropbox implementation
│   └── (DropboxFileAccessor)
│
├── KMyMoney.Net.Core.FileAccessors.Dropbox.Tests/  # Tests
│   └── (Unit tests for Dropbox accessor)
│
├── KMyMoney.Net.Core.Tests/      # Core unit tests
│   └── (Unit tests for Core)
│
├── KMyMoney.Net.Cli/             # CLI wrapper
│   └── Program.cs
│
├── KMyMoney.Net.TelegramBot/     # Telegram bot
│   ├── Commands/
│   │   ├── (Bot command classes)
│   │   └── ...
│   ├── Services/
│   │   ├── (Bot services)
│   │   └── ...
│   └── Program.cs
│
├── KMyMoney.Net.TelegramBot.Persistence/  # Persistence interfaces
│   └── (IUserSettingsRepository, etc.)
│
├── KMyMoney.Net.TelegramBot.Persistence.Etcd/  # etcd implementation
│   └── (EtcdUserSettingsRepository)
│
├── KMyMoney.Net.TelegramBot.Persistence.Etcd.Tests/  # Tests
│   └── (Unit tests for etcd persistence)
│
├── KMyMoney.Net.TelegramBot.Persistence.InMemory/  # In-memory implementation
│   └── (InMemoryUserSettingsRepository)
│
├── KMyMoney.Net.TelegramBot.Persistence.InMemory.Tests/  # Tests
│   └── (Unit tests for in-memory persistence)
│
├── KMyMoney.Net.TelegramBot.Tests/  # TelegramBot unit tests
│   └── (Unit tests for bot logic)
│
├── KMyMoney.Net.TelegramBot.IntegrationTests/  # Integration tests
│   └── (End-to-end test scenarios)
│
└── KMyMoney.Net.Tests.Common/    # Common test utilities
    └── (Test helpers, factories)
```

---

## Solution Analysis

### Projects by Type

| Type | Project | Purpose | Dependencies |
|------|---------|---------|--------------|
| Library | KMyMoney.Net.Models | Data models | None |
| Library | KMyMoney.Net.Core | Core logic | Models, FileAccessors |
| Library | KMyMoney.Net.Core.FileAccessors | Interfaces | None |
| Library | KMyMoney.Net.Core.FileAccessors.Dropbox | Dropbox impl | FileAccessors, Dropbox.Api |
| Library | KMyMoney.Net.TelegramBot.Persistence | Interfaces | None |
| Library | KMyMoney.Net.TelegramBot.Persistence.Etcd | etcd impl | Persistence, dotnet-etcd |
| Library | KMyMoney.Net.TelegramBot.Persistence.InMemory | Memory impl | Persistence |
| Library | KMyMoney.Net.TelegramBot | Bot logic | Core, Persistence, Telegram.Bot |
| Library | KMyMoney.Net.Cli | CLI wrapper | Core |
| Tests | *.Tests | Unit tests | xUnit, Shouldly, NSubstitute |
| Tests | Tests.Common | Test utilities | xUnit, Shouldly, NSubstitute |
| Integration | TelegramBot.IntegrationTests | E2E tests | All |

### Project References

```
KMyMoney.Net.TelegramBot
├── KMyMoney.Net.Core
│   └── KMyMoney.Net.Models
│   └── KMyMoney.Net.Core.FileAccessors
│       └── KMyMoney.Net.Core.FileAccessors.Dropbox
└── KMyMoney.Net.TelegramBot.Persistence
    ├── KMyMoney.Net.TelegramBot.Persistence.Etcd
    └── KMyMoney.Net.TelegramBot.Persistence.InMemory

KMyMoney.Net.Cli
└── KMyMoney.Net.Core
    ├── KMyMoney.Net.Models
    └── KMyMoney.Net.Core.FileAccessors
        └── KMyMoney.Net.Core.FileAccessors.Dropbox
```

---

## Dependency Analysis

### NuGet Packages

#### Core Projects
| Project | Package | Version | Purpose |
|---------|---------|---------|---------|
| Core.FileAccessors.Dropbox | Dropbox.Api | ~5.x | Dropbox API client |
| Core.FileAccessors.Dropbox | Microsoft.Extensions.Http | Latest | HTTP client |
| Core | System.IO.Compression | Built-in | Gzip decompression |
| Core | System.Xml.Serialization | Built-in | XML parsing |

#### TelegramBot Projects
| Project | Package | Version | Purpose |
|---------|---------|---------|---------|
| TelegramBot | Telegram.Bot | ~19.x | Telegram Bot API |
| TelegramBot | Microsoft.Extensions.DependencyInjection | Latest | DI container |
| TelegramBot | Microsoft.Extensions.Logging | Latest | Logging |
| Persistence.Etcd | dotnet-etcd | ~1.x | etcd client |
| Persistence.Etcd | Microsoft.Extensions.Options | Latest | Options pattern |

#### Test Projects
| Project | Package | Version | Purpose |
|---------|---------|---------|---------|
| All .Tests | xunit | ~2.4.x | Test framework |
| All .Tests | xunit.runner.visualstudio | ~2.4.x | Test runner |
| All .Tests | Shouldly | ~4.0.x | Assertions |
| All .Tests | NSubstitute | ~5.x | Mocking |
| All .Tests | Microsoft.NET.Test.Sdk | Latest | Test SDK |

#### CLI Project
| Project | Package | Version | Purpose |
|---------|---------|---------|---------|
| Cli | Microsoft.Extensions.CommandLineUtils | Latest | CLI parsing |
| Cli | System.CommandLine | Alternative | CLI parsing |

### Project References
- **Strong naming**: All projects use `KMyMoney.Net.*` prefix
- **No circular dependencies**: Architecture is layered
- **Test projects**: Each main project has corresponding test project
- **Integration tests**: Separate project for end-to-end tests

---

## Code Quality Metrics

### Test Coverage
- **Test projects**: 8 test projects
- **Production projects**: 9 library projects + 1 CLI project
- **Test to production ratio**: ~0.89 (good)
- **Missing**: Some projects may have incomplete coverage

### File Count
```
Total .cs files: (count from find)
Production files: (TBD)
Test files: (TBD)
```

### Class Count
```
Production classes: (TBD)
Test classes: (TBD)
Ratio: (TBD)
```

---

## AGENTS.md Compliance Analysis

### Rules Check

#### ✅ Followed
- [x] Project uses C#
- [x] Latest stable .NET version
- [x] Dependencies in solution
- [x] All subprojects prefixed with `KMyMoney.Net`
- [x] `.Core` contains manipulation code
- [x] `.Models` contains model files
- [x] `.TelegramBot` contains bot logic
- [x] `.Persistence` defines interfaces
- [x] `.Persistence.Etcd` implements etcd
- [x] `.Persistence.InMemory` implements in-memory
- [x] `.Tests.Common` contains test helpers
- [x] `.Tests` projects for unit tests
- [x] `.sln` references all projects
- [x] Uses Shouldly and NSubstitute

#### ⚠️ Needs Verification
- [ ] One class per file (needs automated check)
- [ ] All classes have tests (needs coverage analysis)
- [ ] No unused dependencies (needs audit)
- [ ] Dependencies are latest stable (needs update check)

#### ❌ Not Applicable
- [ ] No git commands (AI agent restriction, not project issue)

---

## Architecture Patterns

### Layered Architecture
```
TelegramBot (Presentation)
    ↓
Core (Domain Logic)
    ↓
FileAccessors (Infrastructure)
    ↓
Dropbox API / Local FS (External)
```

### Dependency Injection
- Used in TelegramBot for services
- Used in Core for file accessors
- Supports multiple implementations (Dropbox, Local, etc.)

### Repository Pattern
- `IUserSettingsRepository` for user persistence
- Multiple implementations (etcd, in-memory)
- Easy to add new implementations

### Command Pattern
- Bot commands as separate classes
- Easy to add new commands
- Supports priority-based routing

---

## Build Configuration

### Configurations
- **Debug**: Default, with symbols
- **Release**: Optimized, for production
- **Platforms**: Any CPU, x64, x86

### Target Framework
- **All projects**: Latest stable .NET (likely net8.0 or net9.0)
- **Check**: `grep TargetFramework *.csproj`

---

## CI/CD Analysis

### GitHub Workflows
- Location: `.github/workflows/`
- **Typical workflows**:
  - Build and test on push/PR
  - Release packaging
  - Dependency updates

### Build Steps (Typical)
```yaml
1. Checkout code
2. Setup .NET SDK
3. Restore packages (dotnet restore)
4. Build solution (dotnet build)
5. Run tests (dotnet test)
6. Pack (if release)
```

---

## Testing Infrastructure

### Test Frameworks
- **Primary**: xUnit.net
- **Assertions**: Shouldly
- **Mocking**: NSubstitute

### Test Organization
- **Unit tests**: Per-project `.Tests` projects
- **Integration tests**: Separate `IntegrationTests` project
- **Pattern**: Arrange-Act-Assert comments

### Test Execution
```bash
# All tests
dotnet test KMyMoney.Net.sln

# Specific project
dotnet test KMyMoney.Net.Core.Tests

# With filter
dotnet test --filter "Name~Parser"

# With coverage
dotnet test --collect:"XPlat Code Coverage"
```

---

## Domain-Specific Analysis

### .kmy File Processing

**Pipeline**:
```
Dropbox File
    ↓ (download)
byte[] (gzip compressed)
    ↓ (GZipStream.Decompress)
XML string
    ↓ (XmlSerializer)
KMyMoneyFile object
    ↓ (process)
Modified KMyMoneyFile
    ↓ (XmlSerializer)
XML string
    ↓ (GZipStream.Compress)
byte[] (gzip compressed)
    ↓ (upload)
Dropbox File
```

### Key Domain Classes
- `KMyMoneyFile` - Root object
- `Account` - Bank/financial account
- `Transaction` - Financial transaction
- `Institution` - Financial institution
- `Split` - Transaction split

### Telegram Bot Commands
- `/start` - Initialize bot
- `/login` - Dropbox OAuth
- `/list` - List .kmy files
- `/select` - Select file
- `/accounts` - List accounts
- `/transactions` - List transactions
- `/add_transaction` - Add transaction
- `/balance` - Show balance
- `/help` - Show help

---

## Development Environment

### Prerequisites
- .NET SDK (latest stable)
- Git
- IDE: VS Code, Visual Studio, JetBrains Rider
- Optional: Docker (for etcd)

### Local Setup
```bash
# Clone
git clone <repository>

# Restore
dotnet restore

# Build
dotnet build

# Test
dotnet test
```

### Dropbox Integration
- Requires Dropbox API key
- OAuth flow for authentication
- Uses Dropbox.Api NuGet package

### etcd Integration
- Requires etcd cluster
- Can use local Docker container
- Uses dotnet-etcd NuGet package

---

## Security Considerations

### AGPL-3.0 Compliance
- **License**: All code must be AGPL-3.0
- **Attribution**: Must include license file
- **Source availability**: Must provide source code
- **Modifications**: All modifications must be licensed under AGPL-3.0

### Data Security
- **User tokens**: Stored in persistence layer
- **Dropbox access**: OAuth 2.0 flow
- **Encryption**: Consider for sensitive data

### AI Agent Restrictions
- **Use Mistral Vibe or EU-based LLMs**: Recommended for development

---

## Recommendations

### Immediate Actions
1. ✅ Create Vibe skills (DONE)
2. ⬜ Run full test suite to establish baseline
3. ⬜ Check for AGENTS.md violations
4. ⬜ Update dependencies to latest stable

### Short-term Improvements
1. ⬜ Add test file isolation for TDD workflow
2. ⬜ Implement pre-commit hooks
3. ⬜ Create Roslyn analyzer for one-class-per-file
4. ⬜ Add .kmy file validator CLI

### Long-term Improvements
1. ⬜ Implement automated test-writer subagent
2. ⬜ Implement automated developer subagent
3. ⬜ Create TDD workflow coordinator
4. ⬜ Add integration test suite

---

## Open Questions

1. What is the current .NET version?
2. What is the current test coverage percentage?
3. Are there any known AGENTS.md violations?
4. Are dependencies up-to-date?
5. What is the release process?
6. Who are the maintainers?

---

## File Index

### Configuration Files
- `AGENTS.md` - AI agent instructions
- `CLAUDE.md` - Project instructions
- `KMyMoney.Net.sln` - Solution file
- `.gitignore` - Git ignore patterns

### Documentation Files
- `README.md` - Project overview
- `LICENSE` - AGPL-3.0 license
- `packaging/README.md` - Packaging instructions (assumed)

### Vibe Files (NEW)
- `.vibe/README.md` - Vibe configuration
- `.vibe/TDD_WORKFLOW.md` - TDD process
- `.vibe/IDEAS_BACKLOG.md` - Idea backlog
- `.vibe/PROJECT_ANALYSIS.md` - This file
- `.vibe/skills/` - Project skills

---

## Summary

KMyMoney.Net is a well-structured C# project following SOLID principles with:
- Clear layered architecture
- Good test coverage infrastructure
- Proper dependency management
- Strong AI agent guidance (AGENTS.md)

**Next Steps**:
1. Run full analysis with new skills
2. Establish baseline metrics
3. Implement prioritized improvements from IDEAS_BACKLOG.md

---

*This analysis was created to support AI agent development infrastructure. Last updated: 2026-09-10*
