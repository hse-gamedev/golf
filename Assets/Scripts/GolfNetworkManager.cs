using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;
using Random = UnityEngine.Random;

// Current game flow:
// a) Wait for both players to join, then click Right Mouse Button.
// Then the commands will get sent automatically from each player as the ball reaches BallStopVelocity.
// b) Just start as a host and click Right Mouse Button.
// The game will start with one player.
public class GolfNetworkManager : NetworkManager
{
    public float BallStopVelocity = 1.5f;
    public float StrikePower = 650f;
    
// ===== SERVER =====
    private int lastTurn = -1;
    private Dictionary<int, NetworkConnection> players = new Dictionary<int, NetworkConnection>();
    private GameObject ballObject;
    private GameObject fieldObject;
    private float lastStrikeTime;
    private GolfState state = GolfState.WAITING_FOR_INPUT;

    public override void OnStartServer()
    {
        base.OnStartServer();

        var fieldPrefab = spawnPrefabs[1];
        fieldObject = Instantiate(fieldPrefab);
        fieldObject.name = "Field";
        NetworkServer.Spawn(fieldObject);

        var ballPrefab = spawnPrefabs[0];
        ballObject = Instantiate(ballPrefab);
        NetworkServer.Spawn(ballObject);
        
        NetworkServer.RegisterHandler<MoveMessage>(OnPlayerMove);
    }
    public void OnPlayerMove(NetworkConnection conn, MoveMessage message)
    {
        if (lastTurn == message.Id && players.Count > 1) return;
        Debug.Log("Received turn from " + message.Id);
        state = GolfState.WAITING_FOR_BALL_STOP;
        lastTurn = message.Id;
        lastStrikeTime = Time.time;
        ballObject.GetComponent<Rigidbody>().AddForce(message.Strike * StrikePower);
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);
        var id = conn.connectionId;
        Debug.Log("Connected client on " + id);
        players[id] = conn;
        
        // Choose strategy here
        var strategyName = players.Count == 1 ? "second" : "first";
        
        conn.Send(new InitMessage { Id = conn.connectionId, StrategyName = strategyName });
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
        players.Remove(conn.connectionId);
    }

// ===== CLIENT =====

    private NetworkConnection connection;
    private int clientId;
    private GolfStrategy strategy;
    
    public void OnInitMessage(NetworkConnection conn, InitMessage message)
    {
        clientId = message.Id;
        switch (message.StrategyName)
        {
            case "random":
                strategy = new RandomStrategy();
                break;
            case "up":
                strategy = new UpStrategy();
                break;
            case "first":
                strategy = new FirstStrategy();
                break;
            case "second":
                strategy = new SecondStrategy();
                break;
            // Add more strategies here
            default:
                strategy = new RandomStrategy();
                break;
        }
    }   
    
    public void OnYourTurnMessage(NetworkConnection conn, YourTurnMessage message)
    {
        SendTurn();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        NetworkClient.RegisterHandler<InitMessage>(OnInitMessage);
        NetworkClient.RegisterHandler<YourTurnMessage>(OnYourTurnMessage);
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        connection = conn;
    }

    
    private void SendTurn()
    {
        Vector3 dir = strategy.strike();
        Debug.Log("Sending turn");
        connection.Send(new MoveMessage { Strike = dir, Id = clientId});
    }
    
    private void AskForNextTurn()
    {
        NetworkConnection nextConn;
        if (players.Count > 1)
        {
            nextConn = players.First(pair => pair.Key != lastTurn).Value;
        }
        else
        {
            nextConn = players.First().Value;
        }

        Debug.Log("Asking for next turn " + nextConn.connectionId);
        nextConn.Send(new YourTurnMessage());
    }

// ===== COMMON =====
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log(ballObject.GetComponent<Rigidbody>().velocity.magnitude);
            Debug.Log(state);
            Debug.Log(mode);
        }
        // This branch gets executed on server
        if (mode == NetworkManagerMode.Host || mode == NetworkManagerMode.ServerOnly)
        {
            if (ballObject.GetComponent<Rigidbody>().velocity.magnitude < BallStopVelocity
                && state == GolfState.WAITING_FOR_BALL_STOP
                && Time.time - lastStrikeTime > 0.5f
                )
            {
                state = GolfState.WAITING_FOR_INPUT;
                AskForNextTurn();
            }
        }
        
        // This branch gets executed on client
        if (mode == NetworkManagerMode.Host || mode == NetworkManagerMode.ClientOnly)
        {
            if (connection != null && Input.GetKeyDown(KeyCode.G))
            {
                SendTurn();
            }
        }
    }
}

public enum GolfState
{
    WAITING_FOR_BALL_STOP,
    WAITING_FOR_INPUT
}

public struct MoveMessage : NetworkMessage 
{
    public Vector3 Strike;
    public int Id;
}

public struct YourTurnMessage : NetworkMessage
{
    
}

public struct InitMessage : NetworkMessage
{
    public int Id;
    public string StrategyName;
}