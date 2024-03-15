using BepInEx;
using HarmonyLib;
using TerminalApi.Classes;
using UnityEngine;
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

        static UpgradeManager upgradeManager;
        static Terminal terminal;

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        static void StartPatch(Terminal __instance)
        {
            terminal = __instance;

            if (upgradeManager == null)
                upgradeManager = GameObject.Find("MoreUpgrades.Upgrademanager").GetComponent<UpgradeManager>();

            upgradeManager.CreateUpgrades(); // Needs to be called here since the values are needed

            AddCommand("MUG", commandShop);
            AddUpgradeCommands();
            AddInfoCommands();

            SetGroupCredits(10000000); // Just for testing, needs to be removed later
        }

        static void AddUpgradeCommands()
        {
            foreach (Upgrade upgrade in upgradeManager.upgrades)
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
                            if (upgrade.OnetimeUse)
                                return $"{upgrade.Name} has been purchased\n";
                            else
                                return $"{upgrade.Name} has been upgraded to LVL {upgrade.Upgradelevel}\n";
                        }
                        if (upgrade.OnetimeUse)
                            return $"{upgrade.Name} has already been purchased\n";
                        else
                            return $"{upgrade.Name} is already at max LVL {upgrade.UpgradelevelCap}\n";
                    }
                }, "buy");
            }
        }

        static void AddInfoCommands()
        {
            foreach (Upgrade upgrade in upgradeManager.upgrades)
            {
                AddCommand($"info {upgrade.Name}", new CommandInfo
                {
                    Category = "hidden",
                    DisplayTextSupplier = () =>
                    {
                        return $"\n\n{upgrade.Description}\n\n";
                    }
                });
            }
        }

        static bool CheckForEnoughCredits(int price)
        {
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
                terminal.BuyItemsServerRpc(new int[] { }, newAmount, terminal.numberOfItemsInDropship);
            else
                terminal.SyncGroupCreditsServerRpc(newAmount, terminal.numberOfItemsInDropship);
        }

        static string MoreUpgradesStore()
        {
            string storeString = "Welcome to the MoreUpgrades Store.\nBuy an upgrade or find out more about it using INFO.\n----------------------------\n";

            foreach (Upgrade upgrade in upgradeManager.upgrades)
            {
                storeString += $"\n* {upgrade.Name}";
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