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
        [HarmonyPatch("SetPlanetsWeather")]
        [HarmonyPostfix]
        static void SetPlanetsWeatherPatch(StartOfRound __instance)
        {
            if (upgradeManager == null)
                upgradeManager = GameObject.Find("MoreUpgrades.Upgrademanager").GetComponent<UpgradeManager>();

            if (upgradeManager.weatherCleaner.weatherClean)
            {
                Debug.Log("MoreUpgrades: WeatherCleaner is active, setting all levels to None");
                for (int i = 0; i < __instance.levels.Length; i++)
                {
                    __instance.levels[i].currentWeather = LevelWeatherType.None;
                }
            }
        }
    }
}
