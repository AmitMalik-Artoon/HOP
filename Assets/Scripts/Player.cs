using UnityEngine;
using System;

/// <summary>
/// Handles player movement and responds to game events such as user input and reset.
/// </summary>
internal class Player : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float maxHorizontalMove = 3f; // Maximum horizontal movement boundary

    private Vector3 _initialPosition; // Player's initial position

    private void Awake()
    {
        // Subscribe to game events
        EventManager.Instance.AddEventListener<Vector3>(GamePlayEvents.UserInput, OnUserInput);
        EventManager.Instance.AddEventListener<bool>(GamePlayEvents.Reset, OnResetEvent);

        // Store the initial position of the player
        _initialPosition = transform.position;
    }

    /// <summary>
    /// Handles the Reset event by resetting the player's position to the initial position.
    /// </summary>
    /// <param name="sender">The sender of the event (unused).</param>
    /// <param name="eventArgs">Event arguments (unused).</param>
    private void OnResetEvent(object sender, bool eventArgs)
    {
        transform.position = _initialPosition;
    }

    /// <summary>
    /// Handles the UserInput event to move the player and constrain horizontal movement.
    /// </summary>
    /// <param name="sender">The sender of the event (unused).</param>
    /// <param name="eventArgs">The movement vector provided by user input.</param>
    private void OnUserInput(object sender, Vector3 eventArgs)
    {
        // Apply the user input to the player's position
        transform.position+=new Vector3(eventArgs.x,0,0);

        // Clamp the X position to stay within the defined horizontal boundaries
        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, -maxHorizontalMove, maxHorizontalMove);
        transform.position = clampedPosition;
    }

    /// <summary>
    /// Handles the UserInput from keyboard keys event to move the player and constrain horizontal movement.
    /// </summary>
    /// <param name="sender">The sender of the event (unused).</param>
    /// <param name="eventArgs">The movement vector provided by user input.</param>
    private void OnUserKeyInput(object sender, Vector3 eventArgs)
    {
        // Apply the user input to the player's position
        transform.Translate(eventArgs, Space.World);

        // Clamp the X position to stay within the defined horizontal boundaries
        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, -maxHorizontalMove, maxHorizontalMove);
        transform.position = clampedPosition;
    }

}
