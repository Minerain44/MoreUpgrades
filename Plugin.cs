using BepInEx;
using HarmonyLib;
using TerminalApi.Classes;
using UnityEngine;
using static TerminalApi.TerminalApi;

namespace MoreUpgrades
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("NotAtomicBomb-TerminalApi-1.5.2")]
    [BepInDependency("Evaisa-LethalLib-0.14.2")]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            var harmony = new Harmony(PluginInfo.PLUGIN_GUID);
            harmony.PatchAll();
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }
    }
}