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
        AssetBundle Assets;
        List<Item> shopItems = new List<Item>();

        private void Awake()
        {
            //LoadModAssets();
            //if (Assets == null)
            //{
            //    Debug.LogError("MoreUpgrades: Failed to load custom assets."); // ManualLogSource for your plugin
            //    return;
            //}
            //LoadShopItems();
            //RegisterItemsToShop();

            var harmony = new Harmony(PluginInfo.PLUGIN_GUID);
            harmony.PatchAll();
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }

        private void LoadModAssets()
        {
            string assemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Assets = AssetBundle.LoadFromFile(Path.Combine(assemblyLocation, "modassets"));
        }

        private void LoadShopItems()
        {
            Item EnergyDrinkItem = Assets.LoadAsset<Item>("Items/EnergyDrink/EnergyDrink.asset");
            shopItems.Add(EnergyDrinkItem);
        }

        private void RegisterItemsToShop()
        {
            foreach (Item item in shopItems)
                LethalLib.Modules.Items.RegisterShopItem(shopItem: item, price: item.creditsWorth);
        }
    }
}