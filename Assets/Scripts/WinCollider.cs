using System;
using Mirror;
using UnityEngine;

public class WinCollider : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    { }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Hole")
        {
            Destroy(gameObject);
            Debug.Log("Game over");
        }
    }
    
}
