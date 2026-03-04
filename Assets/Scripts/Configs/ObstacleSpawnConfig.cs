using System;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ObstacleSpawnConfig", menuName = "Game/ObstacleSpawnConfig")]
public class ObstacleSpawnConfig : ScriptableObject
{
    [SerializeField] private float initialSafeZone;
    [SerializeField] private float baseSpawnInterval;
    [SerializeField] private float minSpawnInterval;
    [SerializeField] private float laneWidth;
    [SerializeField] private int obstaclePoolSize;

    public float InitialSafeZone => initialSafeZone;
    public float BaseSpawnInterval => baseSpawnInterval;
    public float MinSpawnInterval => minSpawnInterval;
    public float LaneWidth => laneWidth;
    public int ObstaclePoolSize => obstaclePoolSize;

    [Serializable]
    public struct ObstacleTypeData
    {
        public ObstacleType type;
        public GameObject prefab;
        public float spawnWeight;
    }

    [SerializeField] public List<ObstacleTypeData> obstacleTypes;
    public List<ObstacleTypeData> ObstacleTypes => obstacleTypes;

    public GameObject GetPrefab(ObstacleType type)
    {
        var data = obstacleTypes.Find(x => x.type == type);
        return data.prefab;
    }

    public ObstacleType GetRandomType()
    {
        float totalWeight = 0;
        foreach (var type in obstacleTypes)
            totalWeight += type.spawnWeight;

        float random = UnityEngine.Random.Range(0, totalWeight);
        float currentWeight = 0;

        foreach (var type in obstacleTypes)
        {
            currentWeight += type.spawnWeight;
            if (random <= currentWeight)
                return type.type;
        }

        return ObstacleType.SlideObstacle;
    }
}

public enum ObstacleType
{
    SwerveObstacle = 1,   
    SlideObstacle = 2,    
    JumpObstacle = 3      
}