using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AIPlayer1 : MonoBehaviour
{
    public float delay;
    public float magnitude;
	public Vector3 target;
	
	private Vector3 lastPosition;
	
	private const float MAX_RAND_ANGLE = 10.0f;
	private const float DIST_EPS = 1.0f;
	private const float RAND_SHOT_MAGNITUDE = 50.0f;
	
    IEnumerator Start()
    {		
        while (true)
        {
			DoShot();
            yield return new WaitForSeconds(delay);
        }
    }
	
	private void DoShot() {
		Rigidbody rb = gameObject.GetComponent<Rigidbody>();
		Vector3 diff = lastPosition - rb.position;
		Vector3 dir = new Vector3(0, 0, 0);
		Vector3 force;

		if (Vector3.Distance(lastPosition, rb.position) < DIST_EPS) {
			dir = Random.insideUnitSphere;
			force = dir * magnitude * RAND_SHOT_MAGNITUDE;
		} else {
			dir = target - rb.position;
			float length = dir.magnitude;
			float angle = Random.Range(0.0f, MAX_RAND_ANGLE);
			dir = Quaternion.Euler(0.0f, angle, 0.0f) * dir;
			dir = dir.normalized;
			force = dir * length * magnitude;
		}
		
		lastPosition = rb.position;
		rb.AddForce(force);
	}
}