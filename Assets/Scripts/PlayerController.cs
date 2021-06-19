using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour {
    [SerializeField] Transform playerCamera = null;
    [SerializeField] float mouseSensitivity = 300f;
    [SerializeField] float walkSpeed = 3.0f;
    [SerializeField] float gravityMultiplier = 1.0f;
    [SerializeField] float jumpVelocity = 3.0f;

    [SerializeField] bool lockCursor = true;
    
    public UnityEvent<Vector2Int, Vector2Int> gridPositionChangeEvent;

    private Vector2 viewingAngle = Vector2.zero;
    private Vector3 gravityVelocity = Vector3.zero;

    private MazeController mazeController = null;
    private CharacterController characterController = null;

    private Vector2Int previousGridPosition = Vector2Int.zero;

    void Start() {
        mazeController = GetComponentInParent<MazeController>();
        characterController = GetComponent<CharacterController>();

        if (lockCursor) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void Update() {
        UpdateViewing();
        UpdateMovement();

        Vector2Int gridPosition = (Vector2Int) mazeController.grid.LocalToCell(transform.localPosition);
        if (gridPosition != previousGridPosition) {
            gridPositionChangeEvent.Invoke(gridPosition, previousGridPosition);
            previousGridPosition = gridPosition;
        }
    }

    void UpdateViewing() {
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        viewingAngle += mouseDelta * mouseSensitivity * Time.deltaTime;

        if (viewingAngle.y < -90) viewingAngle.y = -90;
        if (viewingAngle.y > 90) viewingAngle.y = 90;

        transform.localRotation = Quaternion.AngleAxis(viewingAngle.x, Vector3.up);
        playerCamera.localRotation = Quaternion.AngleAxis(-viewingAngle.y, Vector3.right);
    }

    void UpdateMovement() {
        Vector2 inputDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (inputDirection.magnitude > 1.0f) inputDirection.Normalize();

        // apply gravity
        if (characterController.isGrounded) {
            gravityVelocity = Vector3.zero;

            // jump
            if (Input.GetAxis("Jump") > 0) gravityVelocity += Vector3.up * jumpVelocity;
        } else {
            gravityVelocity += Physics.gravity * gravityMultiplier * Time.deltaTime;
        }

        // move the player
        Vector3 velocity = (transform.forward * inputDirection.y + transform.right * inputDirection.x) * walkSpeed + gravityVelocity;
        characterController.Move(velocity * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Token")) {
            Debug.Log("Token Collected!");
            Destroy(other.gameObject);
        }
    }

    public void LookAt(Vector3 target) {
        var euler = (Vector2) Quaternion.LookRotation(target - playerCamera.position, Vector3.up).eulerAngles;
        viewingAngle = new Vector2(euler.y, -euler.x);
    }
}
