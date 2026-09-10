# KMyMoney.Net Vibe Configuration

This directory contains Vibe CLI configuration and skills for the KMyMoney.Net project.

## Structure

```
.vibe/
├── README.md              # This file - Vibe configuration overview
├── skills/                # Project-specific Vibe skills
│   ├── README.md          # Skills index and usage guide
│   ├── kmy-test/          # Test execution and management
│   ├── kmy-tdd/           # Test-Driven Development workflow
│   ├── kmy-deps/          # NuGet dependency management
│   ├── kmy-build/         # Build, pack, and package
│   ├── kmy-analyze/       # Static analysis and compliance
│   └── kmy-domain/        # Domain-specific knowledge
└── ...                   # Future Vibe configuration files
```

## Quick Start

### Load All Skills
```bash
# Load domain knowledge first
skill kmy-domain

# Then load task-specific skills as needed
skill kmy-test
skill kmy-tdd
```

### Common Workflows

#### Running Tests
```
/load kmy-test
dotnet test KMyMoney.Net.sln
```

#### TDD Development
```
/load kmy-tdd
# Follow the TDD workflow with subagents
```

#### Dependency Updates
```
/load kmy-deps
dotnet outdated KMyMoney.Net.sln
```

#### Build and Package
```
/load kmy-build
dotnet build KMyMoney.Net.sln --configuration Release
dotnet pack KMyMoney.Net.sln --configuration Release
```

#### Code Analysis
```
/load kmy-analyze
# Run compliance checks
```

## Vibe CLI Configuration

### Environment Variables

Set these in your shell profile or `.env` file:

```bash
# Project-specific
VIBE_PROJECT_NAME=KMyMoney.Net
VIBE_PROJECT_ROOT=/home/filejunkie/git/KMyMoney.Net

# Preferences
VIBE_SKILLS_DIR=.vibe/skills
```

### Recommended Settings

```bash
# Always use project-local skills
VIBE_SKILL_DISCOVERY=local

# Enable skill caching
VIBE_SKILL_CACHE=true
```

## Project Rules in Vibe

Vibe respects all rules from `AGENTS.md`:

1. **Use Mistral Vibe or EU-based LLMs** - Recommended for development
2. **TDD approach** - Skills support test-first development
3. **Minimal changes** - Skills provide targeted commands
4. **No new dependencies without permission** - Dependency skill requires explicit approval
5. **One class per file** - Analysis skill can verify this

## Skill Usage Examples

### Example 1: Running Tests with Filtering
```
User: I need to run only the parser tests

AI: /load kmy-test
AI: dotnet test --filter "Name~Parser"
```

### Example 2: Adding a New Feature with TDD
```
User: Add a new LocalFileAccessor implementation

AI: /load kmy-tdd
AI: # Creates test subagent and developer subagent
AI: # Orchestrates the workflow
```

### Example 3: Updating Dependencies
```
User: Update all test projects to latest Shouldly

AI: /load kmy-deps
AI: for proj in $(find . -name "*Tests.csproj"); do dotnet add $proj package Shouldly --version latest; done
```

### Example 4: Understanding .kmy File Format
```
User: How are .kmy files structured?

AI: /load kmy-domain
AI: # Provides detailed information about .kmy format
```

## Custom Commands

Add these to your shell aliases for convenience:

```bash
# Quick test run
alias kmy-test="dotnet test KMyMoney.Net.sln"

# Quick build
alias kmy-build="dotnet build KMyMoney.Net.sln --configuration Release"

# Load all skills
alias kmy-skills="skill kmy-domain && skill kmy-test && skill kmy-tdd"
```

## Troubleshooting

### Skills Not Loading
```bash
# Check skill discovery
VIBE_SKILL_DISCOVERY=local

# Verify skill structure
ls -la .vibe/skills/kmy-test/SKILL.md
```

### Commands Not Working
```bash
# Ensure dotnet CLI is available
which dotnet

# Check .NET SDK version
dotnet --version
```

## Contributing

To add or modify skills:

1. Fork the project
2. Create a new skill directory under `.vibe/skills/`
3. Add `SKILL.md` with proper frontmatter
4. Test the skill locally
5. Submit a PR

### Skill Validation

Before committing, verify:
- [ ] Frontmatter is valid (name, description, allowed_models)
- [ ] Commands are tested and working
- [ ] Examples match project patterns
- [ ] No hardcoded paths
- [ ] References to other skills are correct

## Version History

| Date | Change |
|------|--------|
| 2026-09-10 | Initial skills created (kmy-test, kmy-tdd, kmy-deps, kmy-build, kmy-analyze, kmy-domain) |

## Related Files

- `AGENTS.md` - Project AI agent instructions
- `CLAUDE.md` - Project instructions
- `USER_WORKFLOWS.md` - User workflow documentation
- `.github/workflows/` - CI/CD workflows
