using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace KeepInventoryMod
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public const string PLUGIN_GUID = "com.mods.keepinventory";
        public const string PLUGIN_NAME = "KeepInventory";
        public const string PLUGIN_VERSION = "1.0.0";

        public static Plugin Instance { get; private set; }
        public static ManualLogSource Log { get; private set; }

        public static ConfigEntry<bool> KeepItemsOnDeath;

        private Harmony _harmony;

        private void Awake()
        {
            Instance = this;
            Log = Logger;

            KeepItemsOnDeath = Config.Bind(
                "General",
                "KeepItemsOnDeath",
                true,
                "Prevent items from dropping when the player dies"
            );

            _harmony = new Harmony(PLUGIN_GUID);
            _harmony.PatchAll();

            Log.LogInfo($"{PLUGIN_NAME} v{PLUGIN_VERSION} loaded!");
            Log.LogInfo($"  KeepItemsOnDeath = {KeepItemsOnDeath.Value}");
        }

        private void OnDestroy()
        {
            _harmony?.UnpatchSelf();
        }
    }
}