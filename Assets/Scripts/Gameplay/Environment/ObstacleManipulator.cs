using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Zenject.SpaceFighter;

public class ObstacleMover : MonoBehaviour
{
    private float _speed;
    private List<Obstacle> _activeObstacles = new List<Obstacle>();
    private bool _isGameActive = false;

    [Inject] private ObstaclePool _obstaclePool;
    [Inject] private SignalBus _signalBus;
    private void Start()
    {
        _signalBus.Subscribe<PlayerDiedSignal>(OnPlayerDied);
    }

    public void SetSpeed(float speed)
    {
        _speed = speed;
    }

    public void RegisterObstacle(Obstacle obstacle)
    {
        _activeObstacles.Add(obstacle);
    }

    public void UnregisterObstacle(Obstacle obstacle)
    {
        if (_activeObstacles.Contains(obstacle))
        {
            _activeObstacles.Remove(obstacle);
            _obstaclePool.Return(obstacle);
        }
    }

    public void RemoveClosestObstacle(float playerZ)
    {
        Obstacle closest = null;
        float minDistance = 99999;

        foreach (var obs in _activeObstacles)
        {
            float dist = Math.Abs(obs.transform.position.z - playerZ);

            if (dist < minDistance)
            {
                minDistance = dist;
                closest = obs;
            }
        }

        UnregisterObstacle(closest);
    }

    public void ResumeGame()
    {
        _isGameActive = true;
    }

    public void ClearAllObstacles()
    {
        foreach (var obstacle in _activeObstacles)
        {
            if (obstacle != null)
                _obstaclePool.Return(obstacle);
        }
        _activeObstacles.Clear();
    }

    public void ResetGame()
    {
        _isGameActive = true;
    }

    private void Update()
    {
        if (!_isGameActive) return;

        for (int i = _activeObstacles.Count - 1; i >= 0; i--)
        {
            var obstacle = _activeObstacles[i];
            if (obstacle == null) 
            {
                _activeObstacles.RemoveAt(i);
                continue;
            }

            obstacle.transform.position -= new Vector3(0, 0, _speed * Time.deltaTime);

            if (obstacle.transform.position.z < -20f)
            {
                UnregisterObstacle(obstacle);
            }
        }
    }

    private void OnPlayerDied()
    {
        _isGameActive = false;
    }
    private void OnDestroy()
    {
        _signalBus?.Unsubscribe<PlayerDiedSignal>(OnPlayerDied);
    }
}