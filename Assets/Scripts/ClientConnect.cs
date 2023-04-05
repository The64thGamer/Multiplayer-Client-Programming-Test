using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class ClientConnect : NetworkBehaviour
{
    /// <summary>
    /// Transfers client info from Unity Netcode object spawned on Connect.
    /// </summary>
    public override void OnNetworkSpawn()
    {
        if (!IsHost) { Destroy(this.gameObject); }
        GameObject.Find("Game Manager").GetComponent<NetcodeManager>().SpawnNewPlayerHost(OwnerClientId);
    }
}
