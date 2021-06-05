using System;
using Mirror;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Random = UnityEngine.Random;

public class Player : NetworkBehaviour
{
    private GameObject ballObject;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnConnectedToServer()
    {
        ballObject = GameObject.Find("BallName");
        Debug.Log(ballObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            ballObject = GameObject.Find("BallName");
            if (ballObject.GetComponent<NetworkIdentity>().hasAuthority) {
                HitBall();
            }
        }
    }
    
    [Command]
    public void HitBall()
    {
        Vector3 dir = Random.insideUnitSphere * 500;
        dir.y = Math.Abs(dir.y);
        Rigidbody rb = ballObject.GetComponent<Rigidbody>();
        rb.AddForce(dir);
    }
}
