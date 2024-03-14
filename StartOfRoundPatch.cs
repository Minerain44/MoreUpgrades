using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MoreUpgrades
{
    [HarmonyPatch(typeof(StartOfRound))]
    internal class StartOfRoundPatch
    {
        static UpgradeManager upgradeManager;
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        static void StartPatch(StartOfRound __instance)
        {
            CreateUpgrademanager();
            if (upgradeManager == null)
                upgradeManager = GameObject.Find("MoreUpgrades.Upgrademanager").GetComponent<UpgradeManager>();

            upgradeManager.weatherCleaner.StartOfRound = __instance;
        }

        static void CreateUpgrademanager()
        {
            Debug.Log("MoreUpgrades: Creating Upgrademanager");
            GameObject upgradeManagerObj = GameObject.Instantiate(new GameObject());
            upgradeManagerObj.AddComponent<UpgradeManager>();
            upgradeManagerObj.name = "MoreUpgrades.Upgrademanager"; // Makes it easier to find and more compatible with other mods
        }
    }
}
