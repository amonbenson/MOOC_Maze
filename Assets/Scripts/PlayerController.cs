using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    [SerializeField] Transform playerCamera = null;
    [SerializeField] float mouseSensitivity = 3.5f;
    [SerializeField] float walkSpeed = 3.0f;

    [SerializeField] bool lockCursor = true;

    float cameraPitch = 0.0f;
    CharacterController controller;

    void Start() {
        controller = GetComponent<CharacterController>();

        if (lockCursor) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        Debug.Log(Physics.gravity);
    }

    void Update() {
        UpdateViewing();
        UpdateMovement();
    }

    void UpdateViewing() {
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        // pitch
        cameraPitch -= mouseDelta.y * mouseSensitivity;
        cameraPitch = Mathf.Clamp(cameraPitch, -90.0f, 90.0f);
        playerCamera.localEulerAngles = Vector3.right * cameraPitch;

        // yaw
        transform.Rotate(Vector3.up * mouseDelta.x * mouseSensitivity);
    }

    void UpdateMovement() {
        Vector2 inputDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (inputDirection.magnitude > 1.0f) inputDirection.Normalize();

        Vector3 velocity = (transform.forward * inputDirection.y + transform.right * inputDirection.x) * walkSpeed;
        controller.Move(velocity * Time.deltaTime);
    }
}
