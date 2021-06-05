using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class GolfNetworkManager : NetworkManager
{
    public override void OnStartServer()
    {
        base.OnStartServer();
        var ballPrefab = spawnPrefabs[0];
        GameObject ballObject = Instantiate(ballPrefab);
        NetworkServer.Spawn(ballObject);
    }
}
