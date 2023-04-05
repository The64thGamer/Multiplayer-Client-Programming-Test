using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetcodeManager : NetworkBehaviour
{
    [SerializeField] List<Player> players;
    [SerializeField] GameObject playerPrefab;

    [SerializeField] NetworkVariable<int> ServerTickRate = new NetworkVariable<int>(10);
    [SerializeField] NetworkVariable<int> ClientInputTickRate = new NetworkVariable<int>(10);

    float tickTimer;

    public void SpawnNewPlayerHost(ulong id)
    {
        if (!IsHost) { return; }
        GameObject thePlayer = GameObject.Instantiate(playerPrefab);
        thePlayer.GetComponent<NetworkObject>().SpawnWithOwnership(id);
    }

    public void AssignNewPlayerClient(Player player)
    {
        //Player lists are always sorted by ID to prevent searching in RPC
        players.Add(player);
        players.Sort((p1, p2) => p1.OwnerClientId.CompareTo(p2.OwnerClientId));
    }

    private void Update()
    {
        if (!IsHost && tickTimer > 1.0f / (float)ClientInputTickRate.Value)
        {
            tickTimer = 0;
            SendJoystickServerRpc(GetJoyStickInput(), NetworkManager.Singleton.LocalClientId);
            return;
        }
        else if (IsHost && tickTimer > 1.0f / (float)ServerTickRate.Value)
        {
            tickTimer = 0;
            return;
        }
        tickTimer += Time.deltaTime;
    }

    [ServerRpc]
    private void SendJoystickServerRpc(Vector2 joystick, ulong id)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].OwnerClientId == id)
            {
                players[i].UpdateJoystick(joystick);
                return;
            }
        }
    }
    /// <summary>
    /// Client Data needs to be sorted by ID
    /// </summary>
    /// <param name="data"></param>
    [ClientRpc]
    private void SendPosClientRpc(PlayerPosData[] data)
    {
        if(data.Length != players.Count) { return; }
        for (int i = 0; i < players.Count; i++)
        {
            //Check run incase of player disconnect+reconnect inside same tick.
            if (players[i].OwnerClientId == data[i].id)
            {
                players[i].SetNewClientPosition(data[i].pos);
            }
        }
    }

    Vector2 GetJoyStickInput()
    {
        Vector2 input = Vector2.zero;
        return input;
    }
}

public struct PlayerPosData : INetworkSerializable
{
    public ulong id;
    public Vector2 pos;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref id);
        serializer.SerializeValue(ref pos);
    }
}