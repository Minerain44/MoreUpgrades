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

        static List<CommandInfo> commadUpgrades = new List<CommandInfo>();

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        static void StartPatch()
        {
            AddCommand("MUG", commandShop);
        }

        static string MoreUpgrades()
        {
            List<Upgrade> upgrades = new List<Upgrade>();


            string storeString = "More Upgrades Shop\n";

            foreach (Upgrade upgrade in upgrades)
            {
                storeString += $"\n* {upgrade.Name}  //  Price: ${upgrade.Price}";
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

        public int Price { get { return price; } set { price = value; } }
        public string Name { get { return name; } set { name = value; }}
        public string Description { get { return description; } set { description = value; }}
        public int Upgradelevel { get { return upgradelevel; } set { upgradelevel = value; }}

        abstract public void Setup();
    }

    class Postman : Upgrade
    {
        public override void Setup()
        {
            throw new NotImplementedException();
        }
    }

    class BiggerPockets : Upgrade
    {
        public override void Setup()
        {
            throw new NotImplementedException();
        }
    }
}