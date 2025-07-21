using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private List<Obstacle> obstacles;
    [SerializeField] private float obstacleFieldSize = 10.0f;
    [SerializeField] private float obstacleSpacing = 1.0f;
    [SerializeField] private LayerMask obstacleLayerMask;
    [SerializeField] private int seed;

    private void Start()
    {
        GenerateObstacles();
    }

    private void GenerateObstacles()
    {
        if (seed != 0)
        {
            Random.InitState(seed);
        }
        else
        {
            seed = (int)System.DateTime.Now.Ticks;
            Debug.Log("Using random seed: " + seed);
            Random.InitState(seed);
        }

        foreach (var obstacle in obstacles)
        {
            if (obstacle.willSpawn)
            {
                int count = Random.Range(obstacle.min, obstacle.max + 1);
                
                for (int i = 0; i < count; i++)
                {
                    Vector3 newPosition = GetRandomPoint(obstacleFieldSize, -obstacleFieldSize);

                    if (Physics.CheckSphere(newPosition, obstacleSpacing, obstacleLayerMask))
                    {
                        continue;
                    }

                    Instantiate(obstacle.prefab, newPosition, Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up));
                }
            }
        }
    }

    private Vector3 GetRandomPoint(float minX, float minZ)
    {
        Vector3 newVec = new Vector3(Random.Range(minX, -minX), 0f, Random.Range(minZ, -minZ));
        return newVec;
    }
    

}
