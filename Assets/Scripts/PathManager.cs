using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages the spawning, movement, and pooling of path segments in the game.
/// </summary>
internal class PathManager : MonoBehaviour
{
    [Header("Path Segment Settings")]
    [Tooltip("List of available path segment prefabs.")]
    public List<PathSegment> pathSegments;

    [Header("Pooling Settings")]
    [SerializeField, Tooltip("Pool size for object reuse.")]
    private int _poolSize = 10;

    [SerializeField, Tooltip("Initial number of segments to spawn.")]
    private int _initialSegments = 5;

    [Header("Randomization Settings")]
    [SerializeField, Tooltip("Maximum horizontal offset for random paths.")]
    private int _maxXVariation = 2;

    [SerializeField, Tooltip("Maximum vertical offset for random paths.")]
    private float _maxYVariation = 3f;

    [SerializeField, Tooltip("Step size for vertical movement.")]
    private float _verticalStepSize = 1f;

    [Range(0.1f, 1f)]
    [SerializeField, Tooltip("Probability of horizontal randomness.")]
    private float _maxHorizontalRandomness = 0.6f;

    [Range(0f, 1f)]
    [SerializeField, Tooltip("Probability of vertical randomness.")]
    private float _maxVerticalRandomness = 0f;

    private float _randomnessIncreaseRate = 0.01f;
    private float _currentHorizontalRandomness = 0f;
    private float _currentVerticalRandomness = 0f;

    private readonly IList<Segment> _pooledSegments = new List<Segment>();
    private readonly IList<Segment> _activeSegments = new List<Segment>();
    private Segment _lastActiveSegment = null;

    private bool _lastSpawnOnLeft = false;
    private bool _startMoving = false;
    private bool _readyToSpawn = false;
    private float _itemSpawnProbability = 0;

    private void Awake()
    {
        EventManager.Instance.AddEventListener<bool>(GamePlayEvents.Initialize, OnInitialize);
        EventManager.Instance.AddEventListener<bool>(GamePlayEvents.GameStart, OnGameStartEvent);
        EventManager.Instance.AddEventListener<bool>(GamePlayEvents.GameOver, OnGameOverEvent);
        EventManager.Instance.AddEventListener<bool>(GamePlayEvents.Reset, OnResetEvent);
        EventManager.Instance.AddEventListener<float>(GamePlayEvents.UpdateItemSpawnProb, OnUpdateItemSpawnProb);
    }

    private void OnInitialize(object sender, bool eventArgs)
    {
        GenerateSegmentsPool();
        InitializePath();
        _currentHorizontalRandomness = Random.Range(0, _maxHorizontalRandomness / 2);
        _currentVerticalRandomness = 0;
    }

    private void OnUpdateItemSpawnProb(object sender, float eventArgs)
    {
        _readyToSpawn = true;
        _itemSpawnProbability = eventArgs;
    }

    private void OnResetEvent(object sender, bool eventArgs)
    {
        RemoveAllSegments();
    }

    private void OnGameOverEvent(object sender, bool eventArgs)
    {
        _startMoving = false;
        _itemSpawnProbability = 0;
        _readyToSpawn = false;
        _lastActiveSegment = null;
    }

    private void OnGameStartEvent(object sender, bool eventArgs)
    {
        _startMoving = true;
    }

    private void Update()
    {
        if (_startMoving)
        {
            MoveSegments();
        }
    }

    /// <summary>
    /// Generates a pool of reusable segments.
    /// </summary>
    private void GenerateSegmentsPool()
    {
        foreach (var pathSegment in pathSegments)
        {
            for (int i = 0; i < _poolSize; i++)
            {
                var segment = Instantiate(pathSegment.prefab, transform).GetComponent<Segment>();
                segment.gameObject.SetActive(false);
                _pooledSegments.Add(segment);
            }
        }
    }

    /// <summary>
    /// Initializes the path by spawning the initial segments.
    /// </summary>
    private void InitializePath()
    {
        for (int i = 0; i < _initialSegments; i++)
        {
            SpawnSegment();
        }
    }

    /// <summary>
    /// Moves active segments and handles segment recycling.
    /// </summary>
    private void MoveSegments()
    {
        for (int i = _activeSegments.Count - 1; i >= 0; i--)
        {
            var segment = _activeSegments[i];
            segment.transform.Translate(Vector3.back * GameManager.Speed * Time.deltaTime);

            if (segment.transform.position.z < -20f)
            {
                RemoveSegment(segment);
                SpawnSegment();
            }
        }
    }

    /// <summary>
    /// Pick a Random segment from pooledSegments list.
    /// </summary>
    private Segment GetRandomPooledSegment()
    {
        if (_pooledSegments.Count == 0)
        {
            GenerateSegmentsPool();
        }

        Segment segment = _pooledSegments[Random.Range(0, _pooledSegments.Count)];
        _pooledSegments.Remove(segment);
        return segment;
    }


    /// <summary>
    /// Spawns a new segment at a randomized position.
    /// </summary>
    private void SpawnSegment()
    {
        if (_pooledSegments.Count == 0)
        {
            GenerateSegmentsPool();
        }

        var segment = GetRandomPooledSegment();
        segment.transform.position = CalculateSpawnPosition(segment.transform.localScale.z);
        segment.transform.rotation = Quaternion.identity;
        segment.gameObject.SetActive(true);

        _activeSegments.Add(segment);

        if (_lastActiveSegment != null)
        {
            _lastActiveSegment.SetSegmentData(segment, _readyToSpawn, _itemSpawnProbability);
        }

        _lastActiveSegment = segment;
    }

    /// <summary>
    /// Calculates the spawn position for the next segment.
    /// </summary>
    private Vector3 CalculateSpawnPosition(float selectedSegmentLegth)
    {
        if (_lastActiveSegment == null) return Vector3.zero;

        var lastPosition = _lastActiveSegment.transform.position;

        _currentHorizontalRandomness = Mathf.Min(_currentHorizontalRandomness + _randomnessIncreaseRate, _maxHorizontalRandomness);
        _currentVerticalRandomness = Mathf.Min(_currentVerticalRandomness + _randomnessIncreaseRate, _maxVerticalRandomness);

        float randomX = GetHorizontalOffset(_currentHorizontalRandomness);
        float randomY = GetVerticalOffset(lastPosition.y, _currentVerticalRandomness);
        float randomZ = Random.Range((selectedSegmentLegth * 2) + (GameManager.Speed * 0.3f), selectedSegmentLegth * (GameManager.Speed * 0.6f));
        lastPosition.x = lastPosition.y = 0;
        return lastPosition + new Vector3(randomX, randomY, randomZ);
    }

    private float GetHorizontalOffset(float probability)
    {
        if (Random.value < probability)
        {
            return Random.Range(-_maxXVariation, _maxXVariation);
        }

        _lastSpawnOnLeft = !_lastSpawnOnLeft;
        return _lastSpawnOnLeft ? -Random.Range(_maxXVariation / 2, _maxXVariation)
                                       : Random.Range(_maxXVariation / 2, _maxXVariation);
    }

    private float GetVerticalOffset(float currentY, float probability)
    {
        float randomY= Random.value < probability ? currentY + _verticalStepSize : currentY - _verticalStepSize;
        return Mathf.Clamp(randomY, 0, _maxYVariation);
    }

    private void RemoveSegment(Segment segment)
    {
        _activeSegments.Remove(segment);
        segment.gameObject.SetActive(false);
        _pooledSegments.Add(segment);
    }

    private void RemoveAllSegments()
    {
        while (_activeSegments.Count > 0)
        {
            RemoveSegment(_activeSegments[0]);
        }
    }
}
