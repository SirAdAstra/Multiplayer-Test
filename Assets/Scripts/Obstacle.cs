using System;
using UnityEngine;

[Serializable]
public class Obstacle
{
    public GameObject prefab;
    public bool willSpawn;
    public int min;
    public int max;
}
