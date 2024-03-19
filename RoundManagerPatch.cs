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
            if (upgradeManager.hardMode.isEnabled())
                __instance.scrapAmountMultiplier += 2;

            __instance.scrapAmountMultiplier = upgradeManager.scrapMagnet.UpdateValue();
        }

        [HarmonyPatch("BeginEnemySpawning")]
        [HarmonyPrefix]
        static void BeginEnemySpawningPatch(RoundManager __instance)
        {
            Debug.Log("Activate HardMode: ");
            if (upgradeManager.hardMode.isEnabled())
            {
                __instance.currentLevel.maxEnemyPowerCount *= 2;
                __instance.currentMaxInsidePower *= 2;
                __instance.currentMaxOutsidePower *= 2;
            }

            Debug.Log($"MaxLEVEL: {__instance.currentLevel.maxEnemyPowerCount}");
        }
    }
}