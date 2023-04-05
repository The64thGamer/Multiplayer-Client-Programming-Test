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
        players.Add(player);
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

    Vector2 GetJoyStickInput()
    {
        Vector2 input = Vector2.zero;
        return input;
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