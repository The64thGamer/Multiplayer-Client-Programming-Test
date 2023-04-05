using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetcodeManager : NetworkBehaviour
{
    [SerializeField] List<Player> players;
    [SerializeField] GameObject playerPrefab;

    [SerializeField] int tickRate = 10;
    float tickTimer;

    public void SpawnNewPlayerHost(ulong id)
    {
        if (!IsHost) { return; }
        GameObject thePlayer = GameObject.Instantiate(playerPrefab);
        thePlayer.GetComponent<NetworkObject>().SpawnWithOwnership(id);
    }

    public void AssignNewPlayerClient(Player player)
    {
        players.Add(player);
    }

    private void Update()
    {
        if (!IsHost) { return; }
        if (tickTimer > 1 / tickRate)
        {
            tickTimer = 0;
        }
        else
        {
            tickTimer += Time.deltaTime;
        }
    }

    [ServerRpc]
    private void SendPosServerRpc()
    {

    }
}

public struct PlayerPosData : INetworkSerializable
{
    ulong id;
    Vector2 pos;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref id);
        serializer.SerializeValue(ref pos);
    }
}