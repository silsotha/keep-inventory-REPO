# KeepInventory

A simple R.E.P.O. mod that keeps items in your inventory slots after death.

Items held in your hands will still drop normally.

## Installation

Install through **r2modman / Thunderstore Mod Manager**, or place `KeepInventoryMod.dll` in:

```text
BepInEx/plugins/
```

Requires BepInEx 5.x.

## Configuration

The mod is enabled by default.

Config file:

```text
BepInEx/config/com.mods.keepinventory.cfg
```

`KeepItemsOnDeath` — enables or disables inventory protection.

## Multiplayer

Works in multiplayer. Installing the mod on the host is recommended.

## Building

```bash
dotnet build -c Release
```

Update the local BepInEx and R.E.P.O. assembly paths in `KeepInventoryMod.csproj` before building.

## License

MIT
