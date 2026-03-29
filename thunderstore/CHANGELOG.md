# Changelog

## 1.0.0

- Initial release
- Keep inventory slot items (1/2/3) after death and revive
- Block Inventory.ForceUnequip during death sequence
- Block ItemEquippable.RPC_ForceUnequip network calls during death
- Block StatsManager.PlayerInventoryUpdate slot clearing during death
- Configurable via BepInEx config
- Compatible with MoreInventorySlots and other inventory mods
- Multiplayer support (install on host)