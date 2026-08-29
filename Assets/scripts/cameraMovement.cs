using UnityEngine;
using UnityEngine.InputSystem;

public class cameraMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 10f;

    [Header("Mouse Look")]
    public float mouseSensitivity = 0.1f;
    public float maxLookAngle = 89f;

    private float pitch = 0f;

    public GameObject panel;

    public void Awake()
    {
        Cursor.lockState = panel.activeSelf ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = panel.activeSelf;
    }
    void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            panel.SetActive(!panel.activeSelf);
            Cursor.lockState = panel.activeSelf ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = panel.activeSelf;
        }
        if (panel.activeSelf)
            return;
        Look();
        Move();
    }

    void Look()
    {
        if (Mouse.current == null)
            return;

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        float mouseX = mouseDelta.x * mouseSensitivity;
        float mouseY = mouseDelta.y * mouseSensitivity;

        // Look left/right
        transform.Rotate(Vector3.up * mouseX, Space.World);

        // Look up/down
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -maxLookAngle, maxLookAngle);

        transform.localEulerAngles = new Vector3(
            pitch,
            transform.localEulerAngles.y,
            0f
        );
    }

    void Move()
    {
        if (Keyboard.current == null)
            return;

        Vector3 input = Vector3.zero;

        // WASD
        if (Keyboard.current.wKey.isPressed)
            input += Vector3.forward;

        if (Keyboard.current.sKey.isPressed)
            input += Vector3.back;

        if (Keyboard.current.aKey.isPressed)
            input += Vector3.left;

        if (Keyboard.current.dKey.isPressed)
            input += Vector3.right;

        // Spectator-style vertical movement
        if (Keyboard.current.spaceKey.isPressed)
            input += Vector3.up;

        if (Keyboard.current.leftCtrlKey.isPressed)
            input += Vector3.down;

        if (input.sqrMagnitude > 1f)
            input.Normalize();

        // Movement is relative to the camera's rotation
        Vector3 movement = transform.TransformDirection(input);

        transform.position += movement * moveSpeed * Time.deltaTime;
    }
    public void closePanel()
    {
        panel.SetActive(!panel.activeSelf);
        Cursor.lockState = panel.activeSelf ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = panel.activeSelf;
    }
}