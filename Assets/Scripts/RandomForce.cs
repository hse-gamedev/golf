using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomForce : MonoBehaviour
{
    public float delay;
    public float magnitude;
    IEnumerator Start()
    {
        while (true)
        {
            Rigidbody rb = gameObject.GetComponent<Rigidbody>();
            Vector3 dir = Random.insideUnitSphere;
            dir.y = Math.Abs(dir.y);
            rb.AddForce(dir * magnitude);
            yield return new WaitForSeconds(delay);
        }
    }
}
