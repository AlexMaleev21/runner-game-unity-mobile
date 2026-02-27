using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ObstacleSpawnConfig", menuName = "Game/ObstacleSpawnConfig")]
public class ObstacleSpawnConfig : ScriptableObject
{
    public float initialSafeZone = 20f;
    public float spawnInterval = 10f;
    public float laneWidth = 2f;

    [System.Serializable]
    public struct ObstacleTypeData
    {
        public ObstacleType type;
        public GameObject prefab;
        public float spawnWeight;
    }

    public List<ObstacleTypeData> obstacleTypes;

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

        float random = Random.Range(0, totalWeight);
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
    SwerveObstacle,   
    SlideObstacle,    
    JumpObstacle      
}