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
        static void StartPatch()
        {
            if(GameObject.Find("MoreUpgrades.Upgrademanager") == null)
            {
                CreateUpgrademanager();
            }
        }

        [HarmonyPatch("SetPlanetsWeather")]
        [HarmonyPostfix]
        static void SetPlanetsWeatherPatch(StartOfRound __instance)
        {

            if (upgradeManager == null)
            {
                GameObject upgradeManagerObj = GameObject.Find("MoreUpgrades.Upgrademanager");
                if (upgradeManagerObj == null)
                {
                    CreateUpgrademanager();
                    upgradeManagerObj = GameObject.Find("MoreUpgrades.Upgrademanager");
                }

                upgradeManager = upgradeManager.GetComponent<UpgradeManager>();
            }

            Debug.Log("MoreUpgrades: Setting Planets Weather");
            if (upgradeManager.weatherCleaner.weatherCleanerActive)
            {
                Debug.Log("MoreUpgrades: WeatherCleaner is active, setting all levels to None");
                for (int i = 0; i < __instance.levels.Length; i++)
                {
                    __instance.levels[i].currentWeather = LevelWeatherType.None;
                }
            }
            else
                Debug.Log("MoreUpgrades: WeatherCleaner is not active, not setting any levels to None");
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
