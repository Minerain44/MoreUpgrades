using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.IO;
using System.Reflection;
using TerminalApi.Classes;
using UnityEngine;
using static TerminalApi.TerminalApi;
using LethalLib;
using System.Collections.Generic;

namespace MoreUpgrades
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("atomic.terminalapi")]
    [BepInDependency("evaisa.lethallib")]
    public class Plugin : BaseUnityPlugin
    {
        public static AssetBundle Assets;
        static List<Item> shopItems = new List<Item>();
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

            shopItems.Add(EnergyDrinkItem);            
        }

        [HarmonyPatch(typeof(Terminal), "Start")]
        [HarmonyPostfix]
        public static void TerminalStartPatch()
        {
            foreach (Item item in shopItems)
                LethalLib.Modules.Items.RegisterItem(item);
        }
    }
}