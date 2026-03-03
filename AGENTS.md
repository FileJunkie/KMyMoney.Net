# KMyMoney.Net

## General project description

- This project is a Telegram bot
- It allows user to edit `.kmy` files used by KMyMoney application from KDE collection stored in user's DropBox
- `.kmy` files contain XML data compressed with `gzip`
- Programming language used is C#
- Latest stable .NET version should be used
- Dependencies should be updated to the latest stable versions
- `packaging` directory contains everything required to build a Debian package
- `.github` directory contains definitions for GitHub workflows

## Subprojects description

- All subprojects have names prefixed with `KMyMoney.Net`
- `.Cli` contains a CLI wrapper for the libraries, used for manual testing
- `.Core`  contains the code used to manipulate the `.kmy` files
- `.Core.FileAccessors` contains interface definitions for file accessors
- `.Core.FileAccessors.Dropbox` implements file accessors for the case of using Dropbox
- `.Core.Models` contains the model files used to represent the original `.kmy` file contents
- `.TelegramBot` contains the main logic for the telegram bot
- `.TelegramBot.Persistence` defines the interface for user settings persistence layer
- `.TelegramBot.Persistence.Etcd` implements the user persistence layer for etcd
- `.TelegramBot.Persistence.InMemory` implements the user persistence layer for in-memory storage
- `.Tests.Common` contains common test helpers
- Projects which names end in `.Tests` contain unit tests for the projects their names are derived from 
- All projects should be referenced in the top-level `.sln` file.

## Development instructions

- Unless explicitly requested, change as little code as possible, and ask for permissions before performing major refactoring
- Do not add any dependencies on your own
- You are free to update dependencies' versions
- Use test-driven-development approach
- Every class should be in its own file

## Testing instructions

- Use `dotnet test` to run tests
- Use `Shouldly` and `NSubstitute`
- Add Arrange-Act-Assert comments between sections

## Other instructions

- Do not execute any git-related commands on your own

## Additional documentation

- Supported user workflows are documented in `USER_WORKFLOWS.md`
