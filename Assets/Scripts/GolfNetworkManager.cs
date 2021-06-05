using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;
using Random = UnityEngine.Random;

public class GolfNetworkManager : NetworkManager
{
    public GameObject ballObject;
    private LinkedList<NetworkConnection> players = new LinkedList<NetworkConnection>();
    private bool gameStarted = false;

    public override void OnStartServer()
    {
        base.OnStartServer();
        
        var ballPrefab = spawnPrefabs[0];
        ballObject = Instantiate(ballPrefab);
        ballObject.name = "BallName";
        NetworkServer.Spawn(ballObject);
        NetworkServer.RegisterHandler<MoveMessage>(OnPlayerMove);
    }

    // Not needed now, but we can send messages to connections, and server will handle them
    public void OnPlayerMove(NetworkConnection conn, MoveMessage message)
    {
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);
        ballObject.GetComponent<NetworkIdentity>().AssignClientAuthority(conn);
        Debug.Log("Assigned auth on " + conn.connectionId);

        players.AddLast(conn);
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
        players.Remove(conn);
    }
}

public struct MoveMessage : NetworkMessage {
    public Vector3 strike; 
}