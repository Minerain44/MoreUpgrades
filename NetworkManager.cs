using System;
using Unity.Netcode;
using UnityEngine;

namespace MoreUpgrades
{

    public class MoreUpgradesNetworkHandler : NetworkBehaviour
    {
        public static event Action<string> UpgradeEvent;
        public static MoreUpgradesNetworkHandler Instance { get; private set; }

        public override void OnNetworkSpawn()
        {
            UpgradeEvent = null;

            if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
                Instance?.gameObject.GetComponent<NetworkObject>().Despawn();
            Instance = this;

            base.OnNetworkSpawn();
        }

        [ClientRpc]
        public void EventClientRpc(string eventName)
        {
            Debug.Log($"MoreUpgrades: Netowrk event called: {eventName}");
            UpgradeEvent?.Invoke(eventName); // If the event has subscribers (does not equal null), invoke the event
        }

        [ClientRpc]
        public void UpgradeLevelUp(string eventName)
        {
            Debug.Log($"MoreUpgrades: Netowrk event called: {eventName}");
            UpgradeEvent?.Invoke(eventName);
        }
    }
}