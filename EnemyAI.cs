using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.AI;

namespace MoreUpgrades
{
    [HarmonyPatch(typeof(EnemyAI))]
    class EnemyAIPatch
    {
        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void UpdatePatch(EnemyAI __instance)
        {
            NavMeshAgent agent = __instance.GetComponent<NavMeshAgent>();
            agent.speed = 30f;
        }


    }
}
