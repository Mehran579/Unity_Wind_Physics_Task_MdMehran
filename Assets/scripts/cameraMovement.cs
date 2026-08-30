using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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
        OnCountSliderChanged(countslider.value);
        OnSizeSliderChanged(radiusslider.value);
        OnMassSliderChanged(massSlider.value);
    }
    void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            togglePanel();
        }
        if (panel.activeSelf)
            return;
        Look();
        Move();
    }
    public void togglePanel()
    {
        panel.SetActive(!panel.activeSelf);
        Cursor.lockState = panel.activeSelf ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = panel.activeSelf;
    }
    public void OnCLickPanel()
    {
        togglePanel();
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
    

    [Header("Ball Spawn")]
    public GameObject ballPrefab;
    public float spawnDistance = 3f;
    public float baseCheckRadius = 0.5f; 
    public LayerMask terrain;

    [Header("Spawn Stats")]
    public int countToSpawn;
    public float ballSize;   
    public float ballMass;   

    [Header ("UI")]
    public Slider countslider;
    public Slider radiusslider;
    public Slider massSlider;

    public TMP_Text countLabel;
    public TMP_Text sizeLabel;
    public TMP_Text massLabel;

    public float minMass = 1f;
    public float maxMass = 10f;

    public Color lightMassColor = Color.green;
    public Color heavyMassColor = Color.red;
    public void OnCountSliderChanged(float value)
    {
        countToSpawn = (int)value;
        countLabel.text = value.ToString("F1"); 
    }
    public void OnSizeSliderChanged(float value)
    {
        ballSize = value;
        sizeLabel.text = value.ToString("F1"); 
    }
    public void OnMassSliderChanged(float value)
    {
        ballMass= value;
        massLabel.text = value.ToString("F1"); 
    }

    public void OnSpawnClicked()
    {

        for (int n = 0; n < countToSpawn; n++)
            TrySpawnOne();
    }

    void TrySpawnOne()
    {
        Vector3 basePos = transform.position + transform.forward * spawnDistance;
        float checkRadius = baseCheckRadius * ballSize;

        for (int i = 0; i < 5; i++)
        {
            Vector3 candidate = basePos + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));

            if (!Physics.CheckSphere(candidate, checkRadius, terrain))
            {
                GameObject ball = Instantiate(ballPrefab, candidate, Random.rotation);
                ball.transform.localScale = Vector3.one * ballSize;
                ball.GetComponent<Rigidbody>().mass = ballMass;

                float mass01 = Mathf.InverseLerp(minMass, maxMass, ballMass);
                Color ballColor = Color.Lerp(lightMassColor, heavyMassColor, mass01);
                Renderer renderer = ball.GetComponent<Renderer>();
                renderer.material.color = ballColor;
                
                return;
            }
        }
    }
}
