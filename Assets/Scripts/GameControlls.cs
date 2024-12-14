using UnityEngine;

/// <summary>
/// Handles player input for both touch and keyboard controls, dispatching user input events to other game components.
/// </summary>
internal class GameControlls : MonoBehaviour
{
    /// <summary>
    /// Delegate for user input handling.
    /// </summary>
    public delegate void UserInput();

    private UserInput _userInput;

    [SerializeField, Tooltip("Speed of horizontal movement.")]
    private float _horizontalSpeed = 10f;

    private bool _IsKeyPressed = false;
    private bool _isMousePressed = false;

    private bool _isTouching = false;
    private bool _inputEnabled = false;
    private float _playerCamHeightOffset;

    /// <summary>
    /// Initializes the component and registers event listeners.
    /// </summary>
    private void Awake()
    {
        InitializeControls();
        EventManager.Instance.AddEventListener<bool>(GamePlayEvents.GameOver, OnGameOver);
        EventManager.Instance.AddEventListener<bool>(GamePlayEvents.GameStart, OnGameStartEvent);
    }

    /// <summary>
    /// Handles enabling input when the game starts.
    /// </summary>
    private void OnGameStartEvent(object sender, bool eventArgs)
    {
        _isMousePressed = false;
        _IsKeyPressed = false;
        _isTouching = false;
        _inputEnabled = true;
    }

    /// <summary>
    /// Disables input when the game ends.
    /// </summary>
    private void OnGameOver(object sender, bool eventArgs)
    {
        _inputEnabled = false;
    }

    /// <summary>
    /// Updates the input handling each frame.
    /// </summary>
    private void Update()
    {
        if (!_inputEnabled) return;
        _userInput?.Invoke();
    }

    /// <summary>
    /// Initializes the controls based on input method availability.
    /// </summary>
    private void InitializeControls()
    {
        _userInput = Input.touchSupported ? TouchControll : ConsoleControlls;
    }

    private void ConsoleControlls()
    {
        KeyboardControls();
        Mousecontroll();
    }

    /// <summary>
    /// Handles keyboard input for horizontal movement.
    /// </summary>
    private void KeyboardControls()
    {
        if (_isMousePressed) return;
        float horizontalInput = Input.GetAxis("Horizontal");

        if (Mathf.Abs(horizontalInput) > Mathf.Epsilon)
        {
            _IsKeyPressed = true;
            Vector3 horizontalMove = new Vector3(horizontalInput * _horizontalSpeed * Time.deltaTime, 0, 0);
            DispatchUserInput(horizontalMove);
            return;
        }
        _IsKeyPressed = false;
    }

    /// <summary>
    /// Handles mouse input for horizontal movement.
    /// </summary>
    private void Mousecontroll()
    {
        if (_IsKeyPressed) return;
        if (Input.GetMouseButtonDown(0))
        {
            HandlePointerDown(Input.mousePosition);
        }
        if (Input.GetMouseButtonUp(0))
        {
            HandlePointerUp(Input.mousePosition);
        }
        if (Input.GetMouseButton(0))
        {
            HandlePointerDrag(Input.mousePosition);
        }
    }


    /// <summary>
    /// Handles touch input for horizontal movement.
    /// </summary>
    /// 
    Vector3 _preMousePosition = Vector3.zero;
    private void TouchControll()
    {
        if (Input.touchCount > 0)
        {
            
            Touch t = Input.GetTouch(0);
            switch (t.phase)
            {
                case TouchPhase.Began:
                    HandlePointerDown(t.position);
                    break;
                case TouchPhase.Moved:
                    HandlePointerDrag(t.position);
                    break;
                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    //HandlePointerUp(t.position);
                    break;
            }
        }

    }

    /// <summary>
    /// Handles touch input for pointer down.
    /// </summary>
    /// 
    void HandlePointerDown(Vector3 pointerPos)
    {
        _isMousePressed = true;
        _preMousePosition = pointerPos;
    }
    /// <summary>
    /// Handles touch input for pointer drag.
    /// </summary>
    /// 
    void HandlePointerDrag(Vector3 pointerPos)
    {
        Vector3 mCurrPos = pointerPos;
        Vector3 mPrevPos = _preMousePosition;
        mCurrPos.z = mPrevPos.z = Camera.main.transform.position.y - _playerCamHeightOffset;
        Vector3 mWorldSpaceDiff = Camera.main.ScreenToWorldPoint(mCurrPos) - Camera.main.ScreenToWorldPoint(mPrevPos);

        // Apply movement
        DispatchUserInput(mWorldSpaceDiff);

        _preMousePosition = pointerPos;
    }
    void HandlePointerUp(Vector3 pointerPos)
    {
        _isMousePressed = false;
    }


    /// <summary>
    /// Dispatches the user input event to other game components.
    /// </summary>
    /// <param name="horizontalInput">The horizontal movement vector to dispatch.</param>
    private void DispatchUserInput(Vector3 horizontalInput)
    {
        EventManager.Instance.Dispatch(GamePlayEvents.UserInput, null, horizontalInput);
    }
}
