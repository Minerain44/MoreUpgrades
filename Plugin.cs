﻿using BepInEx;
using HarmonyLib;
using System.IO;
using System.Reflection;
using UnityEngine;
using System.Collections.Generic;
using LethalLib.Modules;

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
            LoadModAssets();
            if (Assets == null)
            {
                Debug.LogError("MoreUpgrades: Failed to load custom assets. Skipping plugin...");
                return;
            }
            LoadShopItems();
            RegisterItemsToShop();

            var harmony = new Harmony(PluginInfo.PLUGIN_GUID);
            harmony.PatchAll();
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }

        private void LoadModAssets()
        {
            Debug.Log("MoreUpgrades: Loading Mod Assets");
            string assemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Assets = AssetBundle.LoadFromFile(Path.Combine(assemblyLocation, "modassets"));
        }

        private void LoadShopItems()
        {
            Debug.Log("MoreUpgrades: Loading Shop Items");
            Item EnergyDrinkItem = Assets.LoadAsset<Item>("assets/items/energydrink/energydrink.asset");
            shopItems.Add(EnergyDrinkItem);
        }

        private void RegisterItemsToShop()
        {
            Debug.Log("MoreUpgrades: Registering Items to Shop");
            foreach (Item item in shopItems)
            {
                if (item == null) Debug.Log("MoreUpgrades: item is null");
                Debug.Log($"MoreUpgrades: item: {item.name} worth: {item.creditsWorth}");
                Items.RegisterShopItem(shopItem: item, price: item.creditsWorth);
            }
        }
    }
}