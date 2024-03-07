using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using UnityEngine;

namespace MoreUpgrades
{
    [HarmonyPatch(typeof(RoundManager))]
    internal class RoundManagerPatch
    {
        static UpgradeManager upgradeManager;

        [HarmonyPatch("SpawnScrapInLevel")]
        [HarmonyPrefix]
        static void SpawnScrapInLevelPatch(RoundManager __instance)
        {
            if (upgradeManager == null)
                upgradeManager = GameObject.Find("MoreUpgrades.Upgrademanager").GetComponent<UpgradeManager>();

            __instance.scrapValueMultiplier = upgradeManager.scrapPurifier.UpdateValue();
            __instance.scrapAmountMultiplier = upgradeManager.scrapMagnet.UpdateValue();
        }
    }
}