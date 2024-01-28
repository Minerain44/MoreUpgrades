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

            // Plugin startup logic
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
            DisplayTextSupplier = MoreUpgrades
        };

        static List<Upgrade> upgrades = new List<Upgrade>(){ // All currently existing upgrades
                new Postman(),
                new BiggerPockets()
            };

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        static void StartPatch()
        {
            AddCommand("MUG", commandShop);
            AddUpgradeCommands();
        }

        static void AddUpgradeCommands()
        {
            foreach (Upgrade upgrade in upgrades)
            {
                Debug.Log($"Adding Command {upgrade.Name}...");
                AddCommand(upgrade.Name, new CommandInfo
                {
                    Category = "hidden",
                    DisplayTextSupplier = () =>
                    {
                        if (upgrade.Upgradelevel < upgrade.UpgradelevelCap)
                        {
                            upgrade.LevelUp();
                            return $"{upgrade.Name} has been upgraded to LVL {upgrade.Upgradelevel}\n";
                        }
                        return $"{upgrade.Name} is already at max LVL {upgrade.UpgradelevelCap}\n";
                    }
                });
            }
        }

        static string MoreUpgrades()
        {
            string storeString = "More Upgrades Shop\n";

            foreach (Upgrade upgrade in upgrades)
            {
                storeString += $"\n* {upgrade.Name}  //  Price: ${upgrade.Price}";
                if (upgrade.Upgradelevel > 0 && upgrade.Upgradelevel < upgrade.UpgradelevelCap)
                    storeString += $" - LVL {upgrade.Upgradelevel}";
                else if(upgrade.Upgradelevel >= upgrade.UpgradelevelCap)
                    storeString += $" - Max LVL {upgrade.Upgradelevel}";
            }

            storeString += "\n\n";

            return storeString;
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
        public Postman()
        {
            Price = 500;
            Name = "Postman";
            Description = "Lets you walk faster while on the surface of the moon";
            UpgradelevelCap = 5;
        }

        public override void Setup()
        {
            throw new NotImplementedException();
        }

        public override void LevelUp()
        {
            Upgradelevel++;
            Price += (int)MathF.Round(Price * .15f);
            Price -= Price % 5;
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