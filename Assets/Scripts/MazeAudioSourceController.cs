using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeAudioSourceController : MonoBehaviour {
    AudioSource audioSource;
    MazeController mazeController;
    PlayerController playerController;

    void Start() {
        audioSource = transform.GetComponentInChildren<AudioSource>();
        mazeController = transform.GetComponentInParent<MazeController>();
        playerController = mazeController.GetComponentInChildren<PlayerController>();

        playerController.gridPositionChangeEvent.AddListener(OnGridPositionChange);
    }

    void Update() {
        
    }

    public void OnGridPositionChange(Vector2Int playerGridPosition, Vector2Int playerPrevGridPosition) {
        Debug.Log("player is now at " + playerGridPosition);
    }
}
