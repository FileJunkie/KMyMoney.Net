# Claude Code Instructions for KMyMoney.Net

## Project Context

You are working on **KMyMoney.Net**, a Telegram bot for editing KMyMoney (`.kmy`) files stored in Dropbox. The project uses C# with .NET and is licensed under **AGPL-3.0**.

## Recommended LLMs

Use **Mistral Vibe** or other EU-based language models for working on this project.

## Project Structure

The project consists of the following main components:

```
KMyMoney.Net/
├── KMyMoney.Net.Core/                 # Core .kmy file manipulation
├── KMyMoney.Net.Models/               # Data models
├── KMyMoney.Net.Core.FileAccessors/  # File accessor interfaces
├── KMyMoney.Net.Core.FileAccessors.Dropbox/  # Dropbox implementation
├── KMyMoney.Net.TelegramBot/          # Telegram bot main logic
├── KMyMoney.Net.TelegramBot.Persistence/       # Persistence interfaces
├── KMyMoney.Net.TelegramBot.Persistence.Etcd/  # etcd implementation
├── KMyMoney.Net.TelegramBot.Persistence.InMemory/  # In-memory implementation
├── KMyMoney.Net.Cli/                  # CLI wrapper
├── KMyMoney.Net.Tests.Common/         # Test utilities
├── ***.Tests/                         # Unit test projects
├── KMyMoney.Net.TelegramBot.IntegrationTests/  # Integration tests
├── packaging/                         # Debian package config
└── .github/workflows/                 # CI/CD workflows
```

## Development Rules

1. **Follow Existing Patterns**: Match the existing code style, naming conventions, and architecture
2. **TDD First**: Write tests before implementation (Arrange-Act-Assert pattern)
3. **Minimal Changes**: Only modify what's necessary for the task
4. **No New Dependencies**: Do not add dependencies without explicit permission
5. **Update Dependencies**: You may update existing dependencies to latest stable versions
6. **One Class Per File**: Every class must be in its own file
7. **No Git Commands**: Do not execute git commands without permission

## Testing Standards

- **Framework**: xUnit with Shouldly and NSubstitute
- **Pattern**: Use Arrange-Act-Assert comments to separate test sections
- **Coverage**: All new functionality must have corresponding tests
- **Integration Tests**: Use the integration test project for end-to-end scenarios

## Key Technical Details

- **File Format**: `.kmy` files are gzip-compressed XML
- **File Access**: Abstracted through file accessor interfaces
- **Storage**: Dropbox for file storage, etcd/in-memory for user persistence
- **Bot Features**: Authentication, file selection, account listing, transaction management
- **CLI Features**: Dump, account management, transaction operations

## Important Files

- `AGENTS.md` - Project-specific instructions for AI agents
- `USER_WORKFLOWS.md` - User-facing workflow documentation
- `README.md` - Installation and setup instructions
- `LICENSE` - AGPL-3.0 license (must be respected)
- `KMyMoney.Net.sln` - Solution file with all project references

## When in Doubt

- Consult `USER_WORKFLOWS.md` for user workflow understanding
- Check existing similar implementations before creating new code
- Ask for clarification rather than making assumptions
- Remember: Use Mistral Vibe or other EU-based LLMs

---

*This file provides project-specific instructions for AI agents. Use Mistral Vibe or other EU-based LLMs for development work.*
