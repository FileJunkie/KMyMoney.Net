# KMyMoney.Net

Libraries to work with .kmy files from KMyMoney from .NET, as well as a Telegram bot using these models to manipulate a .kmy file saved in DropBox.

AI disclosure: model classes are mostly (I'm sorry) AI-generated, so are tests, other stuff originally was too, but later almost completely rewritten.

## Telegram bot installation

Public key for the repo is here: https://filejunkie.github.io/KMyMoney.Net/public.asc

Repo line for your Debian-like Linux looks like this:

```
deb [arch=amd64 signed-by=/etc/apt/keyrings/filejunkie.asc] https://filejunkie.github.io/KMyMoney.Net/ main main
```

Where `/etc/apt/keyrings/filejunkie.asc` is wherever you save the key file.

Then install with

```
sudo apt update
sudo apt install kmymoney-net-telegrambot
```

Config file is at `/etc/KMyMoney.Net.TelegramBot/appsettings.json`.
