using UnityEngine;
using UnityEngine.InputSystem;  // New Input System — replaces UnityEngine.Input

/// <summary>
/// Handles first-person player movement and camera mouse-look.
/// Uses the new Unity Input System (Keyboard.current / Mouse.current polling).
///
/// Movement keys — Arrow Keys (not WASD):
///   The PDF assigns S to Shoot, which conflicts with WASD's backward key.
///   Arrow Keys avoid that conflict entirely.
///
/// No jumping is implemented as per spec.
/// Requires a CharacterController component on the same GameObject.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [Tooltip("Walk speed per second.")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Mouse Look")]
    [Tooltip("Drag the Main Camera here in the Inspector.")]
    [SerializeField] private Transform cameraTransform;

    [Tooltip("Mouse sensitivity")]
    [SerializeField] private float mouseSensitivity = 0.15f;

    private CharacterController _characterController;

    /// <summary>Accumulated vertical camera rotation so we can clamp it.</summary>
    private float _cameraXRotation;

    // ─────────────────────────────────────────────────────────────────────────

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        Move();
        Look();
    }

    // ─────────────────────────────────────────────────────────────────────────

    private void Move()
    {
        // Keyboard.current is null if no keyboard device is connected
        var keyBoard = Keyboard.current;
        if (keyBoard == null) return;

        float horizontal = 0f, vertical = 0f;

        // Arrow keys — avoids the S = Shoot conflict
        if (keyBoard.leftArrowKey.isPressed) horizontal = -1f;
        if (keyBoard.rightArrowKey.isPressed) horizontal =  1f;
        if (keyBoard.upArrowKey.isPressed) vertical =  1f;
        if (keyBoard.downArrowKey.isPressed) vertical = -1f;

        Vector3 moveDir = transform.right * horizontal + transform.forward * vertical;
        _characterController.Move(moveDir * moveSpeed * Time.deltaTime);

        // Constant gravity — no jumping
        _characterController.Move(Physics.gravity * Time.deltaTime);
    }

    private void Look()
    {
        var mouse = Mouse.current;
        if (mouse == null) return;

        // In the new Input System, mouse.delta returns raw pixel movement per frame
        Vector2 delta = mouse.delta.ReadValue();

        float mouseX = delta.x * mouseSensitivity;
        float mouseY = delta.y * mouseSensitivity;

        // Vertical: rotate camera up/down, clamped to prevent flipping
        _cameraXRotation -= mouseY;
        _cameraXRotation = Mathf.Clamp(_cameraXRotation, -80f, 80f);
        if (cameraTransform != null)
            cameraTransform.localRotation = Quaternion.Euler(_cameraXRotation, 0f, 0f);

        // Horizontal: rotate the whole player body
        transform.Rotate(Vector3.up * mouseX);
    }
}
