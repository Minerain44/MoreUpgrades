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
        SetupMUG SetupMUG = new SetupMUG();
        static CommandInfo commands = new CommandInfo
        {
            Title = "MUG (MoreUpgrades)",
            Category = "other",
            Description = "Displays the Items and Upgrades from the MoreUpgrades(Title Pending) Mod",
            DisplayTextSupplier = MoreUpgrades
        };

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        static void StartPatch()
        {
            SetupMUG.Setup();
            AddCommand("MUG", commands);
        }
        static string MoreUpgrades()
        {
            return "MoreUpgrades";
        }
    }

    class SetupMUG
    {
        public static void Setup()
        {
            SetupUpgrades();
            SetupItems();
        }

        static void SetupUpgrades()
        {
            SetupPostman();
        }

        static void SetupItems()
        {

        }

        static void SetupPostman()
        {
            GameObject playerObj = GameObject.Find("Player");
            PlayerControllerB player = playerObj.GetComponent<PlayerControllerB>();
            player.movementSpeed += 10f;
            Debug.Log("Postman Speed: " + player.movementSpeed);
        }
    }

    class Upgrade
    {
        public int upgradelevel;
    }
}