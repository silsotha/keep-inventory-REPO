# KeepInventory — R.E.P.O. Mod

Keep your inventory slot items after death. Die, get revived, and your equipped items are still there.

## Features

- **Inventory slots survive death** — items in slots 1/2/3 stay after you die and get revived
- **No visual glitches** — death camera, animations, and physics work normally
- **Multiplayer compatible** — works as host, recommended for all players
- **Configurable** — toggle on/off via BepInEx config
- **Compatible** with MoreInventorySlots and other inventory mods

## What It Does / Doesn't Do

| | |
|---|---|
| ✅ Items in inventory slots (1/2/3) | Kept after death |
| ✅ Works in singleplayer | Fully supported |
| ✅ Works in multiplayer | Install on host + all players recommended |
| ❌ Item held in hand (physics grab) | Will drop — this is a physics limitation |

## Installation

### With mod manager (r2modman / Thunderstore Mod Manager)
1. Search for **KeepInventory**
2. Click **Install**
3. Launch via mod manager

### Manual
1. Install [BepInEx 5.x](https://thunderstore.io/c/repo/p/BepInEx/BepInExPack/)
2. Place `KeepInventoryMod.dll` into `BepInEx/plugins/`
3. Launch the game

## Configuration

File: `BepInEx/config/com.mods.keepinventory.cfg`

| Setting | Default | Description |
|---------|---------|-------------|
| `KeepItemsOnDeath` | `true` | Keep inventory slot items on death |

## How It Works

Death in R.E.P.O. happens in two phases:

```
Phase 1 — PlayerDeathRPC (immediate):
  physGrabber.ReleaseObject()          → drops held item (we allow this)
  playerTransform.SetActive(false)     → death camera (we allow this)

Phase 2 — PlayerDeathDone (after animation):
  physGrabber.ReleaseObject()          → drops held item (we allow this)
  Inventory.instance.ForceUnequip()    → DROPS ALL SLOTS ← WE BLOCK THIS
  gameObject.SetActive(false)          → cleanup (we allow this)
```

We intercept only the inventory wipe, leaving physics and camera untouched.

## Patched Methods

| Class | Method | Patch Type | Action |
|-------|--------|-----------|--------|
| `PlayerAvatar` | `PlayerDeathDone` | Transpiler | Replace `ForceUnequip()` with conditional wrapper |
| `PlayerAvatar` | `PlayerDeath` | Prefix | Set dead flag |
| `PlayerAvatar` | `ReviveRPC` | Postfix | Clear dead flag |
| `ItemEquippable` | `RPC_ForceUnequip` | Prefix | Block network unequip while dead |
| `StatsManager` | `PlayerInventoryUpdate` | Prefix | Block slot data clearing while dead |
| `MainMenuOpen` | `Start` | Postfix | Reset state |

## Building

```bash
dotnet build -c Release
```

Update paths in `.csproj` to match your BepInEx and game installation.

## Troubleshooting

- **Items in slots still disappear** — make sure the mod is installed on the host
- **Mod not loading** — check `BepInEx/LogOutput.log` for `[KeepInventory]` lines
- **Breaks after game update** — decompile `Assembly-CSharp.dll` with [dnSpy](https://github.com/dnSpyEx/dnSpy) and verify method names

## License

MIT