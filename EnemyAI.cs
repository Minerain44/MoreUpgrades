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
        static void UpdatePatch(EnemyAI _instance)
        {
            NavMeshAgent agent = _instance.GetComponent<NavMeshAgent>();
            agent.speed = 50f;
        }


    }
}
