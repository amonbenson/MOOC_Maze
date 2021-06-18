using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    [SerializeField] Transform playerCamera = null;
    [SerializeField] float mouseSensitivity = 3.5f;
    [SerializeField] float walkSpeed = 3.0f;
    [SerializeField] float gravityMultiplier = 1.0f;
    [SerializeField] float jumpVelocity = 3.0f;

    [SerializeField] bool lockCursor = true;

    float cameraPitch = 0.0f;
    Vector3 gravityVelocity = Vector3.zero;
    CharacterController controller;

    void Start() {
        controller = GetComponent<CharacterController>();

        if (lockCursor) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
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

        // apply gravity
        if (controller.isGrounded) {
            gravityVelocity = Vector3.zero;

            // jump
            if (Input.GetAxis("Jump") > 0) gravityVelocity += Vector3.up * jumpVelocity;
        } else {
            gravityVelocity += Physics.gravity * gravityMultiplier * Time.deltaTime;
        }

        // move the player
        Vector3 velocity = (transform.forward * inputDirection.y + transform.right * inputDirection.x) * walkSpeed + gravityVelocity;
        controller.Move(velocity * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Token")) {
            Debug.Log("Token Collected!");
            Destroy(other.gameObject);
        }
    }
}
