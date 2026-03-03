# User Workflows

This document describes the user-facing workflows currently implemented in this project.

## Telegram bot workflows

### 1. Receive updates

1. User sends a message to the bot in a private chat.
2. The bot receives the update via webhook (`/webhook`) or Telegram long polling.
3. Non-private chats and non-message updates are ignored.

### 2. Authenticate with Dropbox (`/login`)

1. User sends `/login`.
2. Bot generates a short-lived `state`, stores it with the Telegram user id, and sends a Dropbox authorization URL.
3. User opens the URL, authorizes access, and Dropbox redirects to `/dropbox/callback`.
4. Callback validates `state`, exchanges code for token, and saves user token in persistence.
5. If a previous command failed because token was missing, the bot replays that saved message automatically after login.

### 3. Select a KMyMoney file (`/file`)

1. User sends `/file`.
2. Bot creates a Dropbox accessor using saved user token.
3. Bot lists `.kmy` files from Dropbox and shows them as keyboard options.
4. Bot sets user status to `EnteringFileName`.
5. User sends file path/name.
6. Bot normalizes path to start with `/`, stores it as user `FilePath`, and clears status.

### 4. List accounts from selected file (`/accounts`)

1. User sends `/accounts`.
2. Bot loads selected `.kmy` file from Dropbox.
3. Bot returns non-closed accounts (`Id` + `Name`) in chunks.

### 5. Add transaction interactively (`/add_transaction`)

1. User sends `/add_transaction`.
2. Bot loads selected `.kmy` file and asks for source account (keyboard of account names).
3. Bot sets status `AddTransactionEnteringFromAccount`.
4. User sends source account name/id.
5. Bot validates account, saves `AccountFrom`, asks for destination account, and sets status `AddTransactionEnteringToAccount`.
6. User sends destination account name/id.
7. Bot validates account, saves `AccountTo`, asks for amount and optional currency, and sets status `AddTransactionEnteringPrice`.
8. User sends input like `100` or `100 USD`.
9. Bot validates amount and currency, adds transaction, saves `.kmy`, replies `Saved.`.
10. Bot sets status back to `AddTransactionEnteringFromAccount` so user can continue entering more transactions without reissuing command.

### 6. Command discovery/help

1. If user sends unknown `/command`, bot returns help text with all registered commands and descriptions.

### 7. Error-facing behavior

1. Missing token: bot replies `Use /login to set access token` and preserves current status.
2. Missing file path: bot replies `Use /file to set file path`.
3. Invalid account: bot replies `Wrong account, aborting`.
4. Invalid amount or unknown currency: bot replies with corresponding validation error.
5. Unhandled exceptions: bot sends generic failure message including host name.

## CLI workflows (`KMyMoney.Net.Cli`)

CLI always requires `--file` (`-f`) with a URI (for example `file:///...` or `dropbox:///...`).
The loader supports local file and Dropbox file accessors.

### 1. Dump file XML

1. Run `dump`.
2. CLI loads `.kmy`, decompresses/parses it, and prints XML dump.

### 2. List and query accounts

1. Run `account list` to print all accounts (`id: name`).
2. Run `account get --id <id>` and/or `--name <name>` to filter.

### 3. List and query transactions

1. Run `transaction list` to print transaction ids.
2. Run `transaction get --id <id>` to filter by id.

### 4. Add transaction

1. Run `transaction add --from ... --to ... --amount ... --currency ... [--memo ...]`.
2. CLI appends transaction to model and saves updated `.kmy` to original URI.

## Core file workflow (library behavior)

1. Load `.kmy`: choose accessor by URI scheme, download/open stream, gunzip XML, deserialize into model.
2. Modify model (for example add transaction with optional currency conversion using prices table).
3. Save `.kmy`: serialize XML, prepend XML header/doctype, gzip content, write back through accessor.
