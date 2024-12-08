# LiarsBarBots

This is a host-only mod. You must be the host in order to add bots to a match.

## Features

- Play alone against bots
- Fill empty spots in a lobby with bots when the match starts
- Add bots during a game when there is an empty spot
- The bots do not know your dice/cards, they play according to probability.

## Installation

- Download [BepInEx](https://github.com/BepInEx/BepInEx/releases/download/v5.4.23.2/BepInEx_win_x64_5.4.23.2.zip)
- Extract BepInEx into the game root directory ([official BepInEx installation instructions](https://docs.bepinex.dev/articles/user_guide/installation/index.html))
- Download the DLL from the [LiarsBarBots Latest Release](https://github.com/Kawaii-Ash/LiarsBarBots/releases/latest).
- Place `LiarsBarBots.dll` in the BepInEx plugin folder (`<game root directory>/BepInEx/plugins`).

## Usage

While in a lobby, you can press `Insert` to toggle filling empty spots in the game with bots.
There is some text at the bottom of the screen which shows whether it's enabled or not.

If you are already in a game and there is a free spot, you can press the '0' key on the top of the alphanumeric keyboard to spawn a bot.
