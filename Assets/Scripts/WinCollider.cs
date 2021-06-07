using System;
using UnityEngine;

public class WinCollider : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    { }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.name == "BallName")
        {
            Debug.Log(collider.gameObject.name);
        }
    }
    
}
