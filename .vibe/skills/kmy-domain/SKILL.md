---
name: kmy-domain
description: KMyMoney.Net domain-specific knowledge and helpers
allowed_models: all
---

# KMyMoney.Net Domain Skill

## Purpose
Provide domain-specific knowledge about KMyMoney, .kmy files, Telegram bot workflows, and project architecture.

## Activation
Load this skill when you need to:
- Understand .kmy file format
- Work with KMyMoney data model
- Implement Telegram bot commands
- Navigate project architecture
- Parse or generate .kmy files

## Domain Knowledge

### .kmy File Format

**.kmy files are gzip-compressed XML documents**

```
.kmy file structure:
├── Gzip header (magic: 0x1F 0x8B)
├── Compressed data
└── XML content (KMyMoney XML format)
```

### KMyMoney XML Structure

Root element: `<kmymoney-file>`

Main sections:
```xml
<kmymoney-file>
    <file-information>
        <name>filename.kmy</name>
        <version>1.0</version>
        <date>2025-08-12</date>
    </file-information>
    
    <institution>
        <name>Bank Name</name>
        <id>INST-123</id>
    </institution>
    
    <account>
        <id>ACC-1</id>
        <name>Checking Account</name>
        <type>CHECKING</type>
        <institution-ref>INST-123</institution-ref>
        <opening-date>2025-01-01</opening-date>
        <balance>1000.00</balance>
    </account>
    
    <transaction>
        <id>TXN-1</id>
        <date>2025-08-10</date>
        <memo>Salary</memo>
        <amount>5000.00</amount>
        <account-ref>ACC-1</account-ref>
        <payee>Employer</payee>
        <splits>
            <split>
                <account-ref>ACC-1</account-ref>
                <value>5000.00</value>
            </split>
        </splits>
    </transaction>
</kmymoney-file>
```

### Account Types
| Type | Description |
|------|-------------|
| CHECKING | Checking account |
| SAVINGS | Savings account |
| CREDIT | Credit card |
| INVESTMENT | Investment account |
| ASSET | Asset account |
| LIABILITY | Liability account |
| INCOME | Income category |
| EXPENSE | Expense category |

### Transaction Types
| Type | Description |
|------|-------------|
| DEPOSIT | Money received |
| WITHDRAWAL | Money spent |
| TRANSFER | Money moved between accounts |

## Project Architecture

```
KMyMoney.Net/
├── KMyMoney.Net.Models/               # Data models (POCO)
│   ├── Account.cs
│   ├── Transaction.cs
│   ├── KMyMoneyFile.cs
│   └── ...
│
├── KMyMoney.Net.Core/                 # Core logic
│   ├── Services/
│   │   ├── IKMyMoneyParser.cs
│   │   ├── IKMyMoneyWriter.cs
│   │   └── KMyMoneyService.cs
│   ├── FileAccessors/
│   │   └── IFileAccessor.cs
│   └── ...
│
├── KMyMoney.Net.Core.FileAccessors/   # File accessor interfaces
│   └── IFileAccessor.cs
│
├── KMyMoney.Net.Core.FileAccessors.Dropbox/  # Dropbox implementation
│   └── DropboxFileAccessor.cs
│
├── KMyMoney.Net.TelegramBot/           # Telegram bot
│   ├── Commands/
│   │   ├── StartCommand.cs
│   │   ├── LoginCommand.cs
│   │   ├── ListAccountsCommand.cs
│   │   └── ...
│   ├── Services/
│   │   ├── BotService.cs
│   │   └── UserService.cs
│   └── Program.cs
│
├── KMyMoney.Net.TelegramBot.Persistence/  # Persistence interfaces
│   └── IUserSettingsRepository.cs
│
├── KMyMoney.Net.TelegramBot.Persistence.Etcd/  # etcd implementation
│   └── EtcdUserSettingsRepository.cs
│
├── KMyMoney.Net.TelegramBot.Persistence.InMemory/  # In-memory implementation
│   └── InMemoryUserSettingsRepository.cs
│
├── KMyMoney.Net.Cli/                   # CLI wrapper
│   └── Program.cs
│
└── ***.Tests/                          # Test projects
```

## Data Model (KMyMoney.Net.Models)

### Core Entities

```csharp
// Account
public class Account
{
    public string Id { get; set; }
    public string Name { get; set; }
    public AccountType Type { get; set; }
    public string InstitutionId { get; set; }
    public DateTime OpeningDate { get; set; }
    public decimal Balance { get; set; }
    public List<Transaction> Transactions { get; set; }
}

// Transaction
public class Transaction
{
    public string Id { get; set; }
    public DateTime Date { get; set; }
    public string Memo { get; set; }
    public decimal Amount { get; set; }
    public string Payee { get; set; }
    public string AccountId { get; set; }
    public List<Split> Splits { get; set; }
}

// Split (for split transactions)
public class Split
{
    public string AccountId { get; set; }
    public decimal Value { get; set; }
    public string Memo { get; set; }
}

// KMyMoneyFile (root)
public class KMyMoneyFile
{
    public FileInformation Information { get; set; }
    public List<Institution> Institutions { get; set; }
    public List<Account> Accounts { get; set; }
    public List<Transaction> Transactions { get; set; }
}
```

### Enums

```csharp
public enum AccountType
{
    Checking,
    Savings,
    Credit,
    Investment,
    Asset,
    Liability,
    Income,
    Expense
}

public enum TransactionType
{
    Deposit,
    Withdrawal,
    Transfer
}
```

## Telegram Bot Commands

### Command Flow
```
User sends /start
    ↓
BotService processes command
    ↓
UserService checks authentication
    ↓
DropboxFileAccessor fetches .kmy files
    ↓
KMyMoneyParser parses file
    ↓
Response sent to user
```

### Supported Commands

| Command | Description | Workflow |
|---------|-------------|----------|
| /start | Start bot, show welcome | Register user, show help |
| /login | Authenticate with Dropbox | OAuth flow, store token |
| /list | List available .kmy files | Fetch from Dropbox, show list |
| /select | Select active .kmy file | Store selection in user settings |
| /accounts | List accounts | Parse file, show accounts |
| /transactions | List transactions | Parse file, show transactions |
| /add_transaction | Add new transaction | Parse, modify, save, sync |
| /balance | Show account balance | Parse, calculate, show |
| /help | Show help | Display all commands |

### Command Processing

```csharp
// Command base class
public abstract class BotCommand
{
    public string Name { get; }
    public string Description { get; }
    public string[] Aliases { get; }
    public int Priority { get; } = 0;
    
    public abstract Task<CommandResult> ExecuteAsync(
        Update update,
        CancellationToken cancellationToken);
    
    public virtual bool CanHandle(Update update) => 
        update.Message?.Text?.StartsWith(Name) == true;
}

// Example: ListAccountsCommand
public class ListAccountsCommand : BotCommand
{
    public override string Name => "/accounts";
    public override string Description => "List all accounts in the selected file";
    
    public override async Task<CommandResult> ExecuteAsync(Update update, CancellationToken ct)
    {
        var userId = update.Message.From.Id;
        var userSettings = await _userSettingsRepository.GetAsync(userId);
        
        if (userSettings?.SelectedFileId == null)
            return CommandResult.Failure("No file selected. Use /list and /select first.");
        
        var file = await _fileAccessor.ReadAsync(userSettings.SelectedFileId);
        var kmyFile = await _parser.ParseAsync(file);
        
        var response = string.Join("\n", kmyFile.Accounts.Select(a => $"🏦 {a.Name}: {a.Balance:C}"));
        return CommandResult.Success(response);
    }
}
```

## File Accessor Pattern

### Interface
```csharp
public interface IFileAccessor
{
    Task<byte[]> ReadAsync(string filePath);
    Task WriteAsync(string filePath, byte[] content);
    Task<bool> ExistsAsync(string filePath);
    Task DeleteAsync(string filePath);
    Task<IEnumerable<string>> ListFilesAsync(string directoryPath);
}
```

### Implementations

| Implementation | Location | Usage |
|----------------|----------|-------|
| DropboxFileAccessor | Core.FileAccessors.Dropbox | Production (Dropbox) |
| LocalFileAccessor | (to be added) | Testing/Development |

## User Persistence Pattern

### Interface
```csharp
public interface IUserSettingsRepository
{
    Task<UserSettings> GetAsync(long userId);
    Task SetAsync(long userId, UserSettings settings);
    Task DeleteAsync(long userId);
    Task<bool> ExistsAsync(long userId);
}
```

### UserSettings Model
```csharp
public class UserSettings
{
    public long UserId { get; set; }
    public string DropboxAccessToken { get; set; }
    public string SelectedFileId { get; set; }
    public DateTime LastActivity { get; set; }
    public BotState State { get; set; }
}

public enum BotState
{
    None,
    AwaitingLogin,
    AwaitingFileSelection,
    AwaitingAccountSelection,
    AwaitingTransactionDetails
}
```

### Implementations

| Implementation | Location | Usage |
|----------------|----------|-------|
| EtcdUserSettingsRepository | TelegramBot.Persistence.Etcd | Production (distributed) |
| InMemoryUserSettingsRepository | TelegramBot.Persistence.InMemory | Testing/Development |

## KMyMoney Parsing

### Parse .kmy File
```csharp
public class KMyMoneyParser : IKMyMoneyParser
{
    private readonly XmlSerializer _serializer;
    
    public KMyMoneyParser()
    {
        _serializer = new XmlSerializer(typeof(KMyMoneyFile));
    }
    
    public async Task<KMyMoneyFile> ParseAsync(Stream stream)
    {
        // Step 1: Decompress gzip
        using var gzipStream = new GZipStream(stream, CompressionMode.Decompress);
        
        // Step 2: Deserialize XML
        var kmyFile = (KMyMoneyFile)_serializer.Deserialize(gzipStream);
        
        return kmyFile;
    }
}
```

### Write .kmy File
```csharp
public class KMyMoneyWriter : IKMyMoneyWriter
{
    private readonly XmlSerializer _serializer;
    
    public async Task<byte[]> WriteAsync(KMyMoneyFile kmyFile)
    {
        // Step 1: Serialize to XML
        using var xmlStream = new MemoryStream();
        _serializer.Serialize(xmlStream, kmyFile);
        xmlStream.Position = 0;
        
        // Step 2: Compress with gzip
        using var gzipStream = new MemoryStream();
        using (var compressStream = new GZipStream(gzipStream, CompressionLevel.Optimal, leaveOpen: true))
        {
            await xmlStream.CopyToAsync(compressStream);
        }
        
        return gzipStream.ToArray();
    }
}
```

## Cli Commands

### CLI Structure
```
kmymoney [command] [options]

Commands:
  dump        Dump .kmy file to stdout
  accounts    List accounts in .kmy file
  transactions List transactions in .kmy file
  add-transaction Add a new transaction
  validate    Validate .kmy file
```

### Example Usage
```bash
# Dump file
kmymoney dump --file /path/to/file.kmy

# List accounts
kmymoney accounts --file /path/to/file.kmy

# Add transaction
kmymoney add-transaction --file /path/to/file.kmy --account "Checking" --amount 100.00 --memo "Test"
```

## Domain-Specific Patterns

### Command Priority
Commands can have priority levels for conflict resolution:
- High priority (100): Critical system commands (/start, /login)
- Medium priority (50): Core functionality (/list, /select, /accounts)
- Low priority (10): Utility commands (/help, /balance)
- Default priority (0): Other commands

### User State Management
The bot maintains user state for multi-step operations:
```
State: None → /start → State: AwaitingLogin
State: AwaitingLogin → OAuth complete → State: AwaitingFileSelection
State: AwaitingFileSelection → /select → State: Ready
State: Ready → /add_transaction → State: AwaitingTransactionDetails
State: AwaitingTransactionDetails → Input received → State: Ready
```

### Error Handling
All commands should handle errors gracefully:
- Invalid input → User-friendly error message
- Missing file → Prompt to select file
- Authentication error → Prompt to re-authenticate
- Parse error → Show error details, suggest validation

## Common Operations

### Validate .kmy File
```csharp
public class KMyMoneyValidator
{
    public async Task<ValidationResult> ValidateAsync(Stream stream)
    {
        try
        {
            // Check gzip header
            var buffer = new byte[2];
            await stream.ReadAsync(buffer, 0, 2);
            stream.Position = 0;
            
            if (buffer[0] != 0x1F || buffer[1] != 0x8B)
                return ValidationResult.Invalid("Not a valid gzip file");
            
            // Try to parse
            var kmyFile = await _parser.ParseAsync(stream);
            
            // Validate structure
            if (kmyFile.Accounts == null)
                return ValidationResult.Invalid("No accounts found");
            
            return ValidationResult.Valid();
        }
        catch (Exception ex)
        {
            return ValidationResult.Invalid(ex.Message);
        }
    }
}
```

### Search Transactions
```csharp
public class TransactionSearchService
{
    public IEnumerable<Transaction> Search(
        KMyMoneyFile kmyFile,
        TransactionSearchCriteria criteria)
    {
        var query = kmyFile.Transactions.AsEnumerable();
        
        if (criteria.AccountId != null)
            query = query.Where(t => t.AccountId == criteria.AccountId);
        
        if (criteria.DateFrom != null)
            query = query.Where(t => t.Date >= criteria.DateFrom);
        
        if (criteria.DateTo != null)
            query = query.Where(t => t.Date <= criteria.DateTo);
        
        if (criteria.MinAmount != null)
            query = query.Where(t => t.Amount >= criteria.MinAmount);
        
        if (!string.IsNullOrEmpty(criteria.Memo))
            query = query.Where(t => t.Memo.Contains(criteria.Memo));
        
        return query.ToList();
    }
}
```

## Testing Patterns

### Test Data Factory
```csharp
public static class TestDataFactory
{
    public static KMyMoneyFile CreateSampleKmyFile()
    {
        return new KMyMoneyFile
        {
            Information = new FileInformation
            {
                Name = "test.kmy",
                Version = "1.0"
            },
            Accounts = new List<Account>
            {
                new Account { Id = "ACC-1", Name = "Checking", Type = AccountType.Checking, Balance = 1000m },
                new Account { Id = "ACC-2", Name = "Savings", Type = AccountType.Savings, Balance = 5000m }
            },
            Transactions = new List<Transaction>
            {
                new Transaction { Id = "TXN-1", AccountId = "ACC-1", Amount = 100m, Date = DateTime.Now, Memo = "Test" }
            }
        };
    }
    
    public static byte[] CreateGzippedKmyFile(KMyMoneyFile kmyFile)
    {
        var writer = new KMyMoneyWriter();
        return writer.WriteAsync(kmyFile).Result;
    }
}
```

### Mocking FileAccessor
```csharp
// Using NSubstitute
var fileAccessor = Substitute.For<IFileAccessor>();
fileAccessor.ReadAsync("test.kmy").Returns(TestDataFactory.CreateGzippedKmyFile(TestDataFactory.CreateSampleKmyFile()));
```

## Useful Queries

### Find All Commands
```bash
grep -r "public class.*Command" KMyMoney.Net.TelegramBot --include="*.cs"
```

### Find All Models
```bash
grep -r "public class" KMyMoney.Net.Models --include="*.cs"
```

### Find All FileAccessor Implementations
```bash
grep -r "IFileAccessor" . --include="*.cs" | grep "class"
```

### Find All UserSettingsRepository Implementations
```bash
grep -r "IUserSettingsRepository" . --include="*.cs" | grep "class"
```
