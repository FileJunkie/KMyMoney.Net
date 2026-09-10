# KMyMoney.Net Vibe Skills

This directory contains specialized Vibe skills for the KMyMoney.Net project. Each skill provides domain-specific knowledge, commands, and workflows to assist AI agents working on this project.

## Available Skills

| Skill | Description | When to Use |
|-------|-------------|-------------|
| [`kmy-test`](kmy-test/SKILL.md) | Test execution and management | Running tests, filtering, coverage, debugging |
| [`kmy-tdd`](kmy-tdd/SKILL.md) | Test-Driven Development workflow | Implementing features with TDD, test-first approach |
| [`kmy-deps`](kmy-deps/SKILL.md) | Dependency management | Checking, updating, resolving NuGet packages |
| [`kmy-outdated`](kmy-outdated/SKILL.md) | dotnet-outdated tool | Using dotnet-outdated to check and update dependencies |
| [`kmy-build`](kmy-build/SKILL.md) | Build and packaging | Building, packing NuGet/Debian packages, publishing |
| [`kmy-analyze`](kmy-analyze/SKILL.md) | Static analysis | Code quality checks, AGENTS.md compliance, architecture analysis |
| [`kmy-domain`](kmy-domain/SKILL.md) | Domain knowledge | Understanding .kmy format, Telegram bot, project architecture |

## Loading Skills

To load a skill, use the Vibe skill command:
```
/load kmy-test
/load kmy-tdd
/load kmy-domain
```

Or explicitly:
```
skill kmy-test
skill kmy-tdd
skill kmy-domain
```

## Skill Structure

Each skill has:
- **Frontmatter**: Name, description, and allowed models
- **Purpose**: What the skill does
- **Activation**: When to use it
- **Commands**: Ready-to-use shell commands
- **Patterns**: Code patterns and examples
- **Best Practices**: Recommendations for the project

## Usage Patterns

### Single Task
Load one skill for a specific task:
```
/load kmy-test
Run all tests: dotnet test
```

### Multi-Skill Task
Load multiple skills for complex tasks:
```
/load kmy-build
/load kmy-deps
/load kmy-outdated
Update dependencies and rebuild
```

### TDD Workflow
```
/load kmy-tdd
/load kmy-test
Implement feature with test-first approach
```

## Creating New Skills

To add a new skill:
1. Create a directory under `.vibe/skills/`
2. Add a `SKILL.md` file with proper frontmatter
3. Follow the existing structure and style
4. Add the skill to this README

## Project-Specific Rules

All skills must respect:
- **AGENTS.md**: Project development rules
- **AGPL-3.0**: License requirements
- **Use Mistral Vibe or EU-based LLMs**: Recommended for development

## Skill Development Guidelines

1. **Be specific**: Skills should focus on a narrow domain
2. **Be actionable**: Provide ready-to-use commands
3. **Be accurate**: Keep information up-to-date with project state
4. **Be clear**: Use simple, direct language
5. **Include examples**: Show real project patterns

## Maintaining Skills

Regularly review and update skills when:
- Project structure changes
- Dependencies are updated
- New patterns are established
- AGENTS.md rules are modified

## Index of All Files

```
.vibe/skills/
├── README.md                    # This file
├── kmy-test/
│   └── SKILL.md                # Test execution skill
├── kmy-tdd/
│   └── SKILL.md                # TDD workflow skill
├── kmy-deps/
│   └── SKILL.md                # Dependency management skill
├── kmy-outdated/
│   └── SKILL.md                # dotnet-outdated tool skill
├── kmy-build/
│   └── SKILL.md                # Build and packaging skill
├── kmy-analyze/
│   └── SKILL.md                # Static analysis skill
└── kmy-domain/
    └── SKILL.md                # Domain knowledge skill
```
