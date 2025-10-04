
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FPSController : MonoBehaviour
{
    public Camera playerCamera;
    public float moveSpeed = 5.0f;
    public float sprintMultiplier = 1.8f;
    public float lookSensitivity = 2.0f;
    public float jumpForce = 5.0f;
    public float gravity = 9.81f;

    private CharacterController controller;
    private float pitch = 0f;
    private float yVelocity = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (playerCamera == null) playerCamera = GetComponentInChildren<Camera>();
        Cursor.lockState = CursorLockMode.Locked; Cursor.visible = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) { Cursor.lockState = CursorLockMode.None; Cursor.visible = true; }
        if (Input.GetMouseButtonDown(0)) { Cursor.lockState = CursorLockMode.Locked; Cursor.visible = false; }

        float mx = Input.GetAxis("Mouse X") * lookSensitivity;
        float my = Input.GetAxis("Mouse Y") * lookSensitivity;
        transform.Rotate(0f, mx, 0f);
        pitch = Mathf.Clamp(pitch - my, -85f, 85f);
        if (playerCamera) playerCamera.transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);

        float speed = moveSpeed * (Input.GetKey(KeyCode.LeftShift) ? sprintMultiplier : 1f);
        float h = Input.GetAxis("Horizontal"), v = Input.GetAxis("Vertical");
        Vector3 move = (transform.right * h + transform.forward * v) * speed;

        if (controller.isGrounded)
        {
            yVelocity = -1f;
            if (Input.GetKeyDown(KeyCode.Space)) yVelocity = jumpForce;
        }
        yVelocity -= gravity * Time.deltaTime;
        move.y = yVelocity;
        controller.Move(move * Time.deltaTime);
    }
}
