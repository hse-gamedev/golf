using System;
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