using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeAudioSourceController : MonoBehaviour {
    AudioSource audioSource;
    MazeController mazeController;
    PlayerController playerController;

    Vector2Int previousSourceGridPosition = Vector2Int.zero;
    Vector2Int sourceGridPosition = Vector2Int.zero;
    Vector2Int playerGridPosition = Vector2Int.zero;

    void Start() {
        audioSource = transform.GetComponentInChildren<AudioSource>();
        mazeController = transform.GetComponentInParent<MazeController>();
        playerController = mazeController.GetComponentInChildren<PlayerController>();

        playerController.gridPositionChangeEvent.AddListener(OnGridPositionChange);
    }

    void Update() {
        sourceGridPosition = (Vector2Int) mazeController.grid.LocalToCell(transform.localPosition);
        if (sourceGridPosition != previousSourceGridPosition) {
            RecalculateVirtualPosition();
            previousSourceGridPosition = sourceGridPosition;
        }
    }

    public void OnGridPositionChange(Vector2Int gridPosition, Vector2Int previousGridPosition) {
        playerGridPosition = gridPosition;
        RecalculateVirtualPosition();
    }

    public void RecalculateVirtualPosition() {
        Debug.Log("Need to calculate path " + playerGridPosition + " -> " + sourceGridPosition);
    }
}
