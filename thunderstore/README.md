# KeepInventory

Keep your inventory slot items after death in **R.E.P.O.**

Die, get revived, and your equipped items are still in your inventory slots.

## Features

- **Inventory slots survive death** — items in slots 1/2/3 stay after death and revive
- **No visual glitches** — death camera, animations, and grab work normally
- **Multiplayer support** — install on host, recommended for all players
- **Configurable** — toggle on/off via BepInEx config
- **Compatible** with MoreInventorySlots and other inventory mods

## What Gets Kept

| Item State | Result |
|---|---|
| Items in inventory slots (1/2/3) | ✅ Kept after death |
| Item held in hand (physics grab) | ❌ Drops normally |

The physically held item cannot be preserved due to how R.E.P.O. handles death physics and camera transitions. All items stored in your inventory slots are safe.

## Installation

### With mod manager (r2modman / Thunderstore Mod Manager)
1. Search for **KeepInventory** in the mod browser
2. Click **Install**
3. Launch the game through the mod manager

### Manual
1. Install [BepInEx 5.x](https://thunderstore.io/c/repo/p/BepInEx/BepInExPack/)
2. Place `KeepInventoryMod.dll` into `BepInEx/plugins/`
3. Launch the game

## Configuration

After first launch, edit `BepInEx/config/com.mods.keepinventory.cfg`:

| Setting | Default | Description |
|---------|---------|-------------|
| `KeepItemsOnDeath` | `true` | Keep inventory slot items on death |

## Multiplayer

- Install on the **host** for the mod to work
- Recommended: all players install for full compatibility
- Works in singleplayer without any restrictions

## Troubleshooting

- **Items still disappear** — make sure the mod is installed on the host
- **Mod not loading** — check `BepInEx/LogOutput.log` for `[KeepInventory]` lines
- **Breaks after game update** — method names may have changed, open an issue on GitHub