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
        StartCoroutine(ServerStartCheck());
    }

    IEnumerator ServerStartCheck()
    {
        NetcodeManager netcodeManager = GameObject.Find("Game Manager").GetComponent<NetcodeManager>();
        while(!netcodeManager.GetServerStatus())
        {
            Debug.Log("checking");
            yield return null;
        }
        if (!IsHost) { Destroy(this.gameObject); }
        netcodeManager.SpawnNewPlayerHost(OwnerClientId);
        Destroy(this.gameObject);
    }
}
