﻿using System;
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

    class UpgradeManger : MonoBehaviour
    {
        PlayerControllerB player;
        bool playerWasInsideFactory = false;

        static Postman Postman = new Postman();
        static BiggerPockets BiggerPockets = new BiggerPockets();
        public List<Upgrade> upgrades = new List<Upgrade>(){ // All currently existing upgrades
                Postman,
                BiggerPockets
            };

        private void Start()
        {
            if (player == null)
                player = GameNetworkManager.Instance.localPlayerController;
            
            foreach (Upgrade upgrade in upgrades)
            {
                upgrade.Setup();
            }
        }

        private void Update()
        {
            if (player.isInsideFactory != playerWasInsideFactory) //For Postman Upgrade
            {
                playerWasInsideFactory = player.isInsideFactory;
                Postman.UpdateSpeed();
            }
        }
    }

    abstract class Upgrade
    {
        int price;
        string name;
        string description;
        int upgradelevel;
        int upgradelevelCap;

        public int Price { get { return price; } set { price = value; } }
        public string Name { get { return name; } set { name = value; } }
        public string Description { get { return description; } set { description = value; } }
        public int Upgradelevel { get { return upgradelevel; } set { upgradelevel = value; } }
        public int UpgradelevelCap { get { return upgradelevelCap; } set { upgradelevelCap = value; } }

        abstract public void Setup();
        abstract public void LevelUp();
    }

    class Postman : Upgrade
    {
        bool speedUpgradeApplyed = false;
        bool strenghUpgradeApplyed = false;
        PlayerControllerB player;
        float speedOffset = 0;
        float weightOffset = 0;
        public Postman()
        {
            Price = 500;
            Name = "Postman";
            Description = "Lets you walk faster while on the surface of the moon";
            UpgradelevelCap = 5;
        }

        public override void Setup()
        {

        }

        public void UpdateSpeed()
        {
            speedOffset = Upgradelevel * 0.5f;

            if (player.isInsideFactory && speedUpgradeApplyed)
            {
                player.movementSpeed -= speedOffset;
                speedUpgradeApplyed = false;
                Debug.Log("MoreUpgrades: Removed Speed upgrade");
            }
            if (!player.isInsideFactory && !speedUpgradeApplyed)
            {
                player.movementSpeed += speedOffset;
                speedUpgradeApplyed = true;
                Debug.Log("MoreUpgrades: Applyed speed upgrade");
            }
            Debug.Log($"MoreUpgrades: New playerspeed: {player.movementSpeed}");
        }

        public void UpdateStrengh()
        {
            weightOffset = (10 - Upgradelevel) / 10;

            if (player.isInsideFactory && strenghUpgradeApplyed)
            {
                player.carryWeight -= weightOffset;
                strenghUpgradeApplyed = false;
                Debug.Log("MoreUpgrades: Removed Weight upgrade");
            }
            if (!player.isInsideFactory && !strenghUpgradeApplyed)
            {
                player.carryWeight += weightOffset;
                strenghUpgradeApplyed = true;
                Debug.Log("MoreUpgrades: Applyed Weight upgrade");
            }
            Debug.Log($"MoreUpgrades: New Weight: {player.carryWeight}");
        }

        public override void LevelUp()
        {
            if (player == null)
                player = GameNetworkManager.Instance.localPlayerController;

            Debug.Log($"MoreUpgrades: Leveling up Postman to level {Upgradelevel}");
            Upgradelevel++;
            Price += (int)MathF.Round(Price * .15f);
            Price -= Price % 5;
            speedUpgradeApplyed = false;
            UpdateSpeed();
        }
    }

    class BiggerPockets : Upgrade
    {
        public BiggerPockets()
        {
            Price = 750;
            Name = "Bigger Pockets";
            Description = "Gives you one extra inventory slot per level";
            UpgradelevelCap = 3;
        }
        public override void Setup()
        {
            throw new NotImplementedException();
        }

        public override void LevelUp()
        {
            Upgradelevel++;
            Price += (int)MathF.Round(Price * .25f);
            Price -= Price % 5;
        }
    }
}