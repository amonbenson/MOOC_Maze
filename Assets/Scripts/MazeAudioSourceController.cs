using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeAudioSourceController : MonoBehaviour {
    public bool randomStart = true;

    private AudioSource audioSource;
    private MazeController mazeController;
    private PlayerController playerController;

    private Vector2Int previousSourceGridPosition = Vector2Int.zero;
    private Vector2Int sourceGridPosition = Vector2Int.zero;
    private Vector2Int playerGridPosition = Vector2Int.zero;

    private MazePathFinder pathFinder = new MazePathFinder();

    void Start() {
        audioSource = transform.GetComponentInChildren<AudioSource>();
        mazeController = transform.GetComponentInParent<MazeController>();
        playerController = mazeController.GetComponentInChildren<PlayerController>();

        playerController.gridPositionChangeEvent.AddListener(OnGridPositionChange);

        // seek to a random position
        if (randomStart) audioSource.time = Random.Range(0, audioSource.clip.length);
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
        // we are on the same cell (normaly the audio source should get destroyed at this point)
        if (playerGridPosition == sourceGridPosition) {
            audioSource.transform.position = transform.position;
            if (!audioSource.isPlaying) audioSource.Play();
            return;
        }

        // check the distance
        float airDistance = Vector2.Distance(playerGridPosition, sourceGridPosition);
        if (airDistance >= audioSource.maxDistance) {
            audioSource.Pause();
            return;
        }

        // calculate the shortest path to the sound source
        Stack<Vector2Int> path = pathFinder.AStar(mazeController.maze, playerGridPosition, sourceGridPosition);
        if (path == null) return;

        // get the direction from which the audio is coming
        // TODO: better algorithm:
        // -> cast a ray to each grid cell starting two cells at the player position.
        // -> the first ray that hits a wall determines the direction the audio should come from.
        // for now we will just use the next cell
        Vector2Int nextCell = path.Pop();
        Vector3 nextCellCenter = mazeController.grid.GetCellCenterWorld((Vector3Int) nextCell);
        Vector3 playerCellCenter = mazeController.grid.GetCellCenterWorld((Vector3Int) playerGridPosition);
        Vector3 audioDirection = (nextCellCenter - playerCellCenter);
        audioDirection.Normalize();

        // set the virtual position in world coordinates
        float distance = path.Count + 1;
        Vector3 virtualPosition = playerCellCenter + audioDirection * distance;
        virtualPosition.y = transform.position.y; // do not change the y transform
        audioSource.transform.position = virtualPosition;

        // play the audio
        if (!audioSource.isPlaying) audioSource.Play();
    }
}
