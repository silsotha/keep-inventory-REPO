using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace KeepInventoryMod.Patches
{
    /// <summary>
    /// Keeps inventory slot items (1/2/3) after death.
    ///
    /// Death in R.E.P.O. has two phases:
    ///
    ///   PlayerDeathRPC() [immediate]:
    ///     -> physGrabber.ReleaseObject(-1, 0.1f)    // drops held item — we allow this
    ///     -> playerTransform.SetActive(false)         // death camera — we allow this
    ///
    ///   PlayerDeathDone() [after animation]:
    ///     -> physGrabber.ReleaseObject(-1, 0.1f)    // drops held item — we allow this
    ///     -> Inventory.instance.ForceUnequip()        // DROPS ALL SLOTS — WE BLOCK THIS
    ///     -> gameObject.SetActive(false)              // death cleanup — we allow this
    ///
    /// Strategy: let physics and camera work normally (no screen glitches),
    /// only block the inventory slot wipe. Held item drops, but slot items survive.
    /// </summary>
    [HarmonyPatch]
    public static class DeathDropPatch
    {
        private static bool _playerIsDead = false;

        // ═══════════════════════════════════════════
        //  Death/revive flag
        // ═══════════════════════════════════════════

        [HarmonyPatch(typeof(PlayerAvatar), "PlayerDeath")]
        [HarmonyPrefix]
        [HarmonyPriority(Priority.First)]
        static void OnPlayerDeath()
        {
            if (!Plugin.KeepItemsOnDeath.Value) return;
            _playerIsDead = true;
            Plugin.Log.LogInfo("[KeepInventory] PlayerDeath — dead flag ON");
        }

        [HarmonyPatch(typeof(PlayerAvatar), "ReviveRPC")]
        [HarmonyPostfix]
        static void OnRevive()
        {
            _playerIsDead = false;
            Plugin.Log.LogInfo("[KeepInventory] ReviveRPC — dead flag OFF");
        }

        // ═══════════════════════════════════════════
        //  Transpiler on PlayerDeathDone
        //  Replaces Inventory.ForceUnequip() with our conditional wrapper
        //  Everything else (ReleaseObject, SetActive) runs normally
        // ═══════════════════════════════════════════

        public static void ConditionalForceUnequip(Inventory inventory)
        {
            if (!Plugin.KeepItemsOnDeath.Value)
            {
                inventory.ForceUnequip();
                return;
            }
            Plugin.Log.LogInfo("[KeepInventory] BLOCKED Inventory.ForceUnequip — slot items kept!");
        }

        [HarmonyPatch(typeof(PlayerAvatar), "PlayerDeathDone")]
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> TranspileDeathDone(IEnumerable<CodeInstruction> instructions)
        {
            var forceUnequipMethod = AccessTools.Method(typeof(Inventory), "ForceUnequip");
            var replacement = AccessTools.Method(typeof(DeathDropPatch), nameof(ConditionalForceUnequip));

            int patchCount = 0;

            foreach (var code in instructions)
            {
                if (forceUnequipMethod != null && code.Calls(forceUnequipMethod))
                {
                    yield return new CodeInstruction(OpCodes.Call, replacement);
                    patchCount++;
                }
                else
                {
                    yield return code;
                }
            }

            Plugin.Log.LogInfo($"[KeepInventory] Transpiler: patched {patchCount} ForceUnequip calls in PlayerDeathDone");
        }

        // ═══════════════════════════════════════════
        //  Safety net: block StatsManager inventory slot clearing
        //  ForceUnequip internally calls PlayerInventoryUpdate("", slot)
        //  but other paths might too — this catches them all
        // ═══════════════════════════════════════════

        [HarmonyPatch(typeof(StatsManager), "PlayerInventoryUpdate")]
        [HarmonyPrefix]
        static bool PreventInventoryClear(string _steamID, string itemName, int spot, bool sync)
        {
            if (!Plugin.KeepItemsOnDeath.Value) return true;

            if (string.IsNullOrEmpty(itemName) && _playerIsDead)
            {
                Plugin.Log.LogInfo($"[KeepInventory] BLOCKED inventory slot {spot} clear");
                return false;
            }

            return true;
        }

        // ═══════════════════════════════════════════
        //  Block individual item ForceUnequip RPC
        //  RPC_ForceUnequip teleports the item and clears the slot
        // ═══════════════════════════════════════════

        [HarmonyPatch(typeof(ItemEquippable), "RPC_ForceUnequip")]
        [HarmonyPrefix]
        static bool BlockItemForceUnequip()
        {
            if (!Plugin.KeepItemsOnDeath.Value) return true;

            if (_playerIsDead)
            {
                Plugin.Log.LogInfo("[KeepInventory] BLOCKED ItemEquippable.RPC_ForceUnequip");
                return false;
            }

            return true;
        }

        // ═══════════════════════════════════════════
        //  Reset
        // ═══════════════════════════════════════════

        public static void Reset()
        {
            _playerIsDead = false;
        }
    }

    [HarmonyPatch(typeof(MainMenuOpen), "Start")]
    public static class MainMenuClearPatch
    {
        [HarmonyPostfix]
        static void Postfix()
        {
            DeathDropPatch.Reset();
            Plugin.Log.LogInfo("[KeepInventory] Main menu — state reset.");
        }
    }
}