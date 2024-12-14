using System;
using UnityEngine;

/// <summary>
/// Handles the behavior and physics of the Ball, including its interaction with game events and collision responses.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
internal class Ball : MonoBehaviour
{
    private Rigidbody _rb;
    private MeshRenderer _renderer;
    private bool _isDead;

    /// <summary>
    /// Initializes the Rigidbody and MeshRenderer components and registers event listeners.
    /// </summary>
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _renderer = GetComponent<MeshRenderer>();

        ResetBallState();

        EventManager.Instance.AddEventListener<bool>(GamePlayEvents.Initialize, OnInitializeEvent);
        EventManager.Instance.AddEventListener<bool>(GamePlayEvents.GameStart, OnGameStartEvent);
        EventManager.Instance.AddEventListener<bool>(GamePlayEvents.Reset, OnResetEvent);
    }

    /// <summary>
    /// Handles the initialization of the ball during the game's initialization phase.
    /// </summary>
    private void OnInitializeEvent(object sender, bool eventArgs)
    {
        _renderer.enabled = true;
    }

    /// <summary>
    /// Resets the ball's state to its initial configuration.
    /// </summary>
    private void OnResetEvent(object sender, bool eventArgs)
    {
        ResetBallState();
    }

    /// <summary>
    /// Prepares the ball for gameplay by enabling physics.
    /// </summary>
    private void OnGameStartEvent(object sender, bool eventArgs)
    {
        _rb.isKinematic = false;
    }

    /// <summary>
    /// Dispatches a game-over event and sets the ball to a "dead" state.
    /// </summary>
    private void CallGameOver()
    {
        _isDead = true;
        EventManager.Instance.Dispatch<bool>(GamePlayEvents.GameOver, null, true);
    }

    /// <summary>
    /// Resets the ball to its starting position and state.
    /// </summary>
    private void ResetBallState()
    {
        _isDead = false;
        transform.localPosition = Vector3.zero;
        _rb.isKinematic = true;
        _renderer.enabled = false;
    }

    /// <summary>
    /// Calculates the vertical velocity required for a jump.
    /// </summary>
    /// <param name="horizontalDistance">Horizontal distance to cover.</param>
    /// <param name="heightOffset">Height difference to reach.</param>
    /// <param name="platformSpeed">Speed of the platform.</param>
    /// <returns>The calculated jump velocity as a Vector3.</returns>
    public Vector3 CalculateJump(float horizontalDistance, float heightOffset, float platformSpeed)
    {
        float time = horizontalDistance / platformSpeed;
        float verticalVelocity = (heightOffset + 0.5f * Physics.gravity.magnitude * time * time) / time;

        return new Vector3(0, verticalVelocity, 0);
    }

    /// <summary>
    /// Applies a calculated jump to the ball's Rigidbody.
    /// </summary>
    /// <param name="rb">The Rigidbody to apply the jump to.</param>
    /// <param name="horizontalDistance">Horizontal distance to cover.</param>
    /// <param name="heightOffset">Height difference to reach.</param>
    /// <param name="platformSpeed">Speed of the platform.</param>
    public void ApplyJump(Rigidbody rb, float horizontalDistance, float heightOffset, float platformSpeed)
    {
        Vector3 jumpVelocity = CalculateJump(horizontalDistance, heightOffset, platformSpeed);
        rb.linearVelocity = jumpVelocity;
    }

    /// <summary>
    /// Handles collision events and triggers score updates and jump calculations when colliding with a Segment.
    /// </summary>
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.TryGetComponent<Segment>(out Segment segment))
        {
            EventManager.Instance.Dispatch<bool>(GamePlayEvents.UpdateScore, null, true);

            float targetHorizontalDistance = segment.NextSegment.transform.position.z - transform.position.z;
            float targetHeightOffset = segment.NextSegment.transform.position.y - transform.position.y;

            ApplyJump(_rb, targetHorizontalDistance, targetHeightOffset, GameManager.Speed);
        }
    }

    /// <summary>
    /// Updates the ball's state and checks for out-of-bounds conditions.
    /// </summary>
    private void Update()
    {
        if (transform.position.y < -1 && !_isDead)
        {
            CallGameOver();
        }

        if (_isDead && !_rb.isKinematic && transform.localPosition.y < -20)
        {
            _renderer.enabled = false;
            _rb.isKinematic = true;
        }
    }
}
