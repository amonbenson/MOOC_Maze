using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour {
    public Transform playerCamera = null;
    public float mouseSensitivity = 300f;

    public float walkSpeed = 3.0f;
    public float runSpeed = 7.0f;

    public float gravityMultiplier = 1.0f;
    public float jumpVelocity = 3.0f;

    public bool lockCursor = true;
    public float stamina = 3.0f;
    public float restorestam = 0.2f;

    public bool collectcoin = false;
    public bool targetreached = false;

    public UnityEvent<Vector2Int, Vector2Int> gridPositionChangeEvent;

    private Vector2 viewingAngle = Vector2.zero;
    private Vector3 gravityVelocity = Vector3.zero;

    private MazeController mazeController = null;
    private CharacterController characterController = null;

    private Vector2Int previousGridPosition = Vector2Int.zero;
    private bool running;

    void Awake() {
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    void Start() {
        mazeController = GetComponentInParent<MazeController>();
        characterController = GetComponent<CharacterController>();

        // hide and lock the cursor
        if (lockCursor) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void OnSceneUnloaded<Scene>(Scene scene) {
        // restore the cursor
        if (lockCursor) {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void Update() {
        UpdateViewing();

        UpdateMovement();

        Vector2Int gridPosition = (Vector2Int) mazeController.grid.WorldToCell(transform.position);
        if (gridPosition != previousGridPosition) {
            gridPositionChangeEvent.Invoke(gridPosition, previousGridPosition);
            previousGridPosition = gridPosition;
        }
    }

    void UpdateViewing() {
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        viewingAngle += mouseDelta * mouseSensitivity;

        if (viewingAngle.y < -90) viewingAngle.y = -90;
        if (viewingAngle.y > 90) viewingAngle.y = 90;

        transform.localRotation = Quaternion.AngleAxis(viewingAngle.x, Vector3.up);
        playerCamera.localRotation = Quaternion.AngleAxis(-viewingAngle.y, Vector3.right);
    }

    void UpdateMovement() {
        Vector2 inputDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (inputDirection.magnitude > 1.0f) inputDirection.Normalize();

        // if we are running, but not pressing any buttons, we default to a forward direction
        if (running && inputDirection.magnitude <= 0.01f) {
            inputDirection = Vector2.up;
        }

        // apply gravity
        if (characterController.isGrounded) {
            gravityVelocity = Vector3.zero;

            // jump
            if (Input.GetAxis("Jump") > 0) gravityVelocity += Vector3.up * jumpVelocity;
        } else {
            gravityVelocity += Physics.gravity * gravityMultiplier * Time.deltaTime;
        }

        // move the player
        float speed = running ? runSpeed : walkSpeed;
        Vector3 transformedDirection = transform.forward * inputDirection.y + transform.right * inputDirection.x;
        Vector3 velocity = transformedDirection * speed + gravityVelocity;
        characterController.Move(velocity * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Token")) {
            //Debug.Log("Token Collected!");
            collectcoin= true;
            Destroy(other.gameObject);
        }

        if (other.CompareTag("Target")) {
            targetreached=true;
        }
    }

    public void LookAt(Vector3 target) {
        var euler = (Vector2) Quaternion.LookRotation(target - playerCamera.position, Vector3.up).eulerAngles;
        viewingAngle = new Vector2(euler.y, -euler.x);
    }

    public void StartRun() {
        running = true;
    }

    public void StopRun() {
        running = false;
    }
}
