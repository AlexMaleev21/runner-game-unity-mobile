using UnityEngine;
using Zenject;
using Zenject.SpaceFighter;
using System;

public class BackgroundMover : MonoBehaviour
{
    private SpeedManager _speedManager;
    private SignalBus _signalBus;

    [Header("Prefabs")]
    [SerializeField] private GameObject _leftSegmentPrefab;
    [SerializeField] private GameObject _rightSegmentPrefab;

    [Header("Settings")]
    [SerializeField] private int _segmentCount;        
    [SerializeField] private float _segmentLength; 

    private Transform[] _leftSegments;
    private Transform[] _rightSegments;
    private bool _isGameActive = true;
    private float _startZ;

    [Inject]
    public void Construct(
        SpeedManager speedManager,
        SignalBus signalBus)
    {
        _speedManager = speedManager;
        _signalBus = signalBus;
    }
    private void Start()
    {
        SpawnSegments();
        _startZ = _leftSegments[0].position.z;
        _signalBus.Subscribe<PlayerDiedSignal>(OnPlayerDied);
        enabled = false;
    }

    private void OnDestroy()
    {
        _signalBus?.Unsubscribe<PlayerDiedSignal>(OnPlayerDied);
    }

    private void SpawnSegments()
    {
        Transform leftParent = new GameObject("LeftSegments").transform;
        leftParent.SetParent(transform);
        Transform rightParent = new GameObject("RightSegments").transform;
        rightParent.SetParent(transform);

        _leftSegments = new Transform[_segmentCount];
        _rightSegments = new Transform[_segmentCount];

        for (int i = 0; i < _segmentCount; i++)
        {
            GameObject left = Instantiate(_leftSegmentPrefab, leftParent);
            left.transform.localPosition = new Vector3(left.transform.localPosition.x, left.transform.localPosition.y, i * _segmentLength);
            _leftSegments[i] = left.transform;

            GameObject right = Instantiate(_rightSegmentPrefab, rightParent);
            right.transform.localPosition = new Vector3(right.transform.localPosition.x, right.transform.localPosition.y, i * _segmentLength);
            _rightSegments[i] = right.transform;
        }
    }

    public void StartMovement()
    {
        _isGameActive = true;
        enabled = true;
    }

    public void StopMovement()
    {
        _isGameActive = false;
        enabled = false;
    }

    private void Update()
    {
        if (!_isGameActive) return;

        float speed = _speedManager.CurrentSpeed;

        foreach (var seg in _leftSegments)
            seg.Translate(Vector3.back * speed * Time.deltaTime);
        foreach (var seg in _rightSegments)
            seg.Translate(Vector3.back * speed * Time.deltaTime);

        if (_leftSegments[0].position.z < -_segmentLength)
        {
            float newZ = _leftSegments[_segmentCount - 1].position.z + _segmentLength;
            _leftSegments[0].position = new Vector3(_leftSegments[0].position.x, _leftSegments[0].position.y, newZ);
            RotateArray(_leftSegments);
        }

        if (_rightSegments[0].position.z < -_segmentLength)
        {
            float newZ = _rightSegments[_segmentCount - 1].position.z + _segmentLength;
            _rightSegments[0].position = new Vector3(_rightSegments[0].position.x, _rightSegments[0].position.y, newZ);
            RotateArray(_rightSegments);
        }
    }

    private void RotateArray(Transform[] array)
    {
        Transform first = array[0];
        for (int i = 0; i < array.Length - 1; i++)
            array[i] = array[i + 1];
        array[array.Length - 1] = first;
    }

    private void OnPlayerDied()
    {
        _isGameActive = false;
    }

    public void ResetBackground()
    {
        _isGameActive = true;
        for (int i = 0; i < _leftSegments.Length; i++)
        {
            _leftSegments[i].position = new Vector3(_leftSegments[i].position.x, _leftSegments[i].position.y, i * _segmentLength);
            _rightSegments[i].position = new Vector3(_rightSegments[i].position.x, _rightSegments[i].position.y, i * _segmentLength);
        }
        Array.Sort(_leftSegments, (a, b) => a.position.z.CompareTo(b.position.z));
        Array.Sort(_rightSegments, (a, b) => a.position.z.CompareTo(b.position.z));
    }
}