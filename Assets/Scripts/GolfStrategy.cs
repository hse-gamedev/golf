using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public interface GolfStrategy
{
    Vector3 strike();
}


public class RandomStrategy : GolfStrategy
{
    public Vector3 strike()
    {
        Vector3 dir = Random.insideUnitSphere;
        dir.y = Math.Abs(dir.y);
        return dir;
    }
}

public class UpStrategy : GolfStrategy
{
    public Vector3 strike()
    {
        return Vector3.up;
    }
}

public class FirstStrategy : GolfStrategy
{
    public float magnitude = 10f;

    private Vector3 lastPosition;

    private const float MAX_RAND_ANGLE = 10.0f;
    private const float DIST_EPS = 1.0f;
    private const float RAND_SHOT_MAGNITUDE = 50.0f;    

    public Vector3 strike()
    {
        GameObject ball = GameObject.Find("BallName");
        GameObject hole = GameObject.Find("Hole");
        Vector3 target = hole.transform.position;

        Rigidbody rb = ball.GetComponent<Rigidbody>();
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
        if (force.magnitude > magnitude)
        {
            force = force.normalized * magnitude;
        }

        lastPosition = rb.position;
        return force;
    }
}

public class SecondStrategy : GolfStrategy
{
    public float magnitude = 10f;

    private Vector3 lastPosition;
    private List<Tuple<Vector3, Vector3>> history = new List<Tuple<Vector3, Vector3>>();

    private const float MAX_RAND_ANGLE = 45.0f;
    private const float DIST_EPS = 1.0f;
    private const float RAND_SHOT_MAGNITUDE = 50.0f;

    public Vector3 strike() {
        GameObject ball = GameObject.Find("BallName");
        GameObject hole = GameObject.Find("Hole");
        Vector3 target = hole.transform.position;

        Rigidbody rb = ball.GetComponent<Rigidbody>();
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

        if (force.magnitude > magnitude)
        {
            force = force.normalized * magnitude;
        }
        
        lastPosition = rb.position;
        history.Add(new Tuple<Vector3, Vector3>(lastPosition, dir));
        rb.AddForce(force);
        return force;
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