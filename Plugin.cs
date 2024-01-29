using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using BepInEx;
using GameNetcodeStuff;
using HarmonyLib;
using TerminalApi;
using TerminalApi.Classes;
using UnityEngine;
using static TerminalApi.Events.Events;
using static TerminalApi.TerminalApi;

namespace MoreUpgrades
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("atomic.terminalapi")]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            var harmony = new Harmony(PluginInfo.PLUGIN_GUID);
            harmony.PatchAll();
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }
    }


    [HarmonyPatch(typeof(Terminal))]
    class TerminalPatch
    {
        static CommandInfo commandShop = new CommandInfo
        {
            Title = "MUG (MoreUpgrades)",
            Category = "other",
            Description = "Displays the Items and Upgrades from the MoreUpgrades(Title Pending) Mod",
            DisplayTextSupplier = MoreUpgradesStore
        };

        static UpgradeManger upgradeManger;
        static Terminal terminal;

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        static void StartPatch(Terminal __instance)
        {
            terminal = __instance;
            GameObject upgradeManagerObj = GameObject.Instantiate(new GameObject());
            upgradeManagerObj.AddComponent<UpgradeManger>();
            upgradeManger = upgradeManagerObj.GetComponent<UpgradeManger>();

            AddCommand("MUG", commandShop);
            AddUpgradeCommands();

            SetGroupCredits(10000000); // Just for testing, needs to be removed later
        }

        static void AddUpgradeCommands()
        {
            foreach (Upgrade upgrade in upgradeManger.upgrades)
            {
                AddCommand(upgrade.Name, new CommandInfo
                {
                    Category = "hidden",
                    DisplayTextSupplier = () =>
                    {
                        if (upgrade.Upgradelevel < upgrade.UpgradelevelCap)
                        {
                            if (!CheckForEnoughCredits(upgrade.Price))
                            {
                                return $"You don't have enought credits to buy this upgrade\n";
                            }
                            upgrade.LevelUp();
                            return $"{upgrade.Name} has been upgraded to LVL {upgrade.Upgradelevel}\n";
                        }
                        return $"{upgrade.Name} is already at max LVL {upgrade.UpgradelevelCap}\n";
                    }
                }); // Add second command with info verb to display the info
            }
        }
        static bool CheckForEnoughCredits(int price)
        {
            Debug.Log($"MoreUpgrades: Price {price}, Credits {terminal.groupCredits}");
            if (terminal.groupCredits >= price)
            {
                SetGroupCredits(terminal.groupCredits -= price);
                Debug.Log("MoreUpgrades: Upgrade Purchased");
                return true;
            }
            return false;
        }

        static void SetGroupCredits(int newAmount)
        {
            terminal.groupCredits = newAmount;
            if (terminal.IsClient)
                terminal.BuyItemsServerRpc(new int[]{}, newAmount, terminal.numberOfItemsInDropship);
            else
                terminal.SyncGroupCreditsServerRpc(newAmount, terminal.numberOfItemsInDropship);
        }

        static string MoreUpgradesStore()
        {
            string storeString = "More Upgrades Shop\n";

            foreach (Upgrade upgrade in upgradeManger.upgrades)
            {
                storeString += $"\n* {upgrade.Name}";
                Debug.Log($"MoreUpgrades: Name {upgrade.Name} LVL {upgrade.Upgradelevel} CAP {upgrade.UpgradelevelCap}");
                if (upgrade.Upgradelevel < upgrade.UpgradelevelCap)
                    storeString += $"  //  Price: ${upgrade.Price}";
                if (upgrade.Upgradelevel > 0 && upgrade.Upgradelevel < upgrade.UpgradelevelCap)
                    storeString += $" // LVL {upgrade.Upgradelevel}";
                else if (upgrade.Upgradelevel >= upgrade.UpgradelevelCap)
                    storeString += $" // Max LVL {upgrade.Upgradelevel}";
            }
            storeString += "\n\n";
            return storeString;
        }
    }
}