using System;
using Unity.Netcode;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MoreUpgrades;

public class MoreUpgradesNetworkHandler : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        LevelEvent = null;

        if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
            Instance?.gameObject.GetComponent<NetworkObject>().Despawn();
        Instance = this;

        base.OnNetworkSpawn();
    }

    [ClientRpc]
    public void EventClientRpc(string eventName)
    {
        LevelEvent?.Invoke(eventName); // If the event has subscribers (does not equal null), invoke the event
    }

    public static event Action<String> LevelEvent;

    public static MoreUpgradesNetworkHandler Instance { get; private set; }
}