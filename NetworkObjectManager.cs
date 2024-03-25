using HarmonyLib;
using System.IO;
using System.Reflection;
using Unity.Netcode;
using UnityEngine;

namespace MoreUpgrades
{
    [HarmonyPatch]
    public class NetworkObjectManager
    {
        static AssetBundle Assets;
        static GameObject networkPrefab;

        [HarmonyPostfix, HarmonyPatch(typeof(GameNetworkManager)), HarmonyPatch("Start")]
        public static void StartPatch()
        {
            Assets = NetworkAssets.LoadAsset();

            if (networkPrefab != null)
                return;

            networkPrefab = (GameObject)Assets.LoadAsset("NetworkHandler");
            networkPrefab.AddComponent<MoreUpgradesNetworkHandler>();
            NetworkManager.Singleton.AddNetworkPrefab(networkPrefab);
        }

        [HarmonyPostfix, HarmonyPatch(typeof(StartOfRound)), HarmonyPatch("Awake")]
        static void AwakePatch()
        {
            if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
            {
                var networkHandlerHost = Object.Instantiate(networkPrefab, Vector3.zero, Quaternion.identity);
                networkHandlerHost.GetComponent<NetworkObject>().Spawn();
            }
        }
    }
}