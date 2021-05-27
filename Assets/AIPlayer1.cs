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
	private List<Tuple<Vector3, Vector3>> history;
	
	private const float MAX_RAND_ANGLE = 45.0f;
	private const float DIST_EPS = 1.0f;
	private const float RAND_SHOT_MAGNITUDE = 50.0f;
	
    IEnumerator Start()
    {
		history = new List<Tuple<Vector3, Vector3>>();
		
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
			dir.y = 0;
			force = dir * magnitude * RAND_SHOT_MAGNITUDE;
		}
		else if (CheckHistory(rb.position, ref dir)) {
			force = dir * magnitude * RAND_SHOT_MAGNITUDE;
		}
		else {
			dir = target - rb.position;
			dir.y = 0;
			float length = dir.magnitude;
			dir = dir.normalized;
			force = dir * length * magnitude;
		}
		
		lastPosition = rb.position;
		history.Add(new Tuple<Vector3, Vector3>(lastPosition, dir));
		rb.AddForce(force);
	}
	
	private bool CheckHistory(Vector3 currentPosition, ref Vector3 newDir) {
		for (int i = history.Count - 1; i >= 0; i--) {
		    Tuple<Vector3, Vector3> t = history[i];
			if (Vector3.Distance(currentPosition, t.Item1) < DIST_EPS) {
				float angle = Random.Range(0.0f, MAX_RAND_ANGLE);
				Vector3 rotated = Quaternion.Euler(0.0f, angle, 0.0f) * t.Item2;
				newDir.x = -rotated.x;
				newDir.y = -rotated.y;
				newDir.z = -rotated.z;
				return true;
			}
		}
		
		return false;
	}
}
