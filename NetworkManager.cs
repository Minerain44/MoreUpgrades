using System;
using Unity.Netcode;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MoreUpgrades
{

    public class MoreUpgradesNetworkHandler : NetworkBehaviour
    {
        public static event Action<String> UpgradeEvent;
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
            UpgradeEvent?.Invoke(eventName); // If the event has subscribers (does not equal null), invoke the event
        }
    }
}