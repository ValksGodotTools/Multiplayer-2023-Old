# GodotUtils Netcode Template
Formely use to be a sandbox testing ground for GodotUtils library (and still is) but is now primarily for a starting project multiplayer template.

## Todo
- Blacklist packets from being logged to prevent console log / GD.Print(...) spam
- Add toggle button in UIConsole to toggle auto scroll
- Only send positions from server to clients if prev position to cur position is greater than 5 pixels (will require keeping track of prev positions server-side)
- Create `SPcketPlayers` (instead of looping through each player and sending it, instead send them all in one go to the joining player)

## Setup
Clone project with modules
```
git clone --recursive https://github.com/Valks-Games/Sandbox2
```

Update submodules (may not be required, I don't have enough experience to say for sure)
```
git submodule update --init --recursive
```
