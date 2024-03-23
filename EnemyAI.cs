using HarmonyLib;
using System;
using System.Text;
using UnityEngine.AI;
using UnityEngine;

namespace MoreUpgrades
{
    [HarmonyPatch(typeof(EnemyAI))]
    class EnemyAIPatch
    {
        static float previousSpeed = 0f;
        static NavMeshAgent agent;
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        static void StartPatch(EnemyAI __instance)
        {
            agent = __instance.GetComponent<NavMeshAgent>();
            Debug.Log("\n----------MoreUpgrades: Enemy Info----------\n" +
                     $"Name: {__instance.gameObject.name}\n" +
                     $"speed: {__instance.agent.speed}\n" +
                     $"ang speed: {__instance.agent.angularSpeed}\n" +
                     $"accel: {__instance.agent.acceleration}\n" +
                     $"hp: {__instance.enemyHP}\n" +
                     $"--------------------------------------------");
        }


        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void UpdatePatch(EnemyAI __instance)
        {
            if (agent.speed != previousSpeed)
            {
                agent.speed *= 3;
                previousSpeed = agent.speed;
            }

        }


    }
}
