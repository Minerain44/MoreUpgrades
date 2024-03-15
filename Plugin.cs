using BepInEx;
using HarmonyLib;
using System.IO;
using System.Reflection;
using TerminalApi.Classes;
using UnityEngine;
using static TerminalApi.TerminalApi;

namespace MoreUpgrades
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("atomic.terminalapi")]
    [BepInDependency("evaisa.lethallib")]
    public class Plugin : BaseUnityPlugin
    {
        public static AssetBundle Assets;
        private void Awake()
        {
            string assemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Assets = AssetBundle.LoadFromFile(Path.Combine(assemblyLocation, "modassets"));

            if (Assets == null)
            {
                Logger.LogError("Failed to load custom assets."); // ManualLogSource for your plugin
                return;
            }

            var harmony = new Harmony(PluginInfo.PLUGIN_GUID);
            harmony.PatchAll();
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
            LoadAsset();
        }

        public void LoadAsset()
        {
            Item EnergyDrinkItem = Assets.LoadAsset<Item>("Items/EnergyDrink/EnergyDrink.asset");
        }
    }
}