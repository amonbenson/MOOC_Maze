using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeAudioSourceController : MonoBehaviour {
    public float range = 10;

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
        // we are on the same cell (normaly a token and its audio source should get destroyed at this point)
        if (playerGridPosition == sourceGridPosition) {
            audioSource.transform.localPosition = Vector3.zero;
            audioSource.Play();
            return;
        }

        // check the distance
        float airDistance = Vector2.Distance(playerGridPosition, sourceGridPosition);
        if (airDistance >= range) {
            audioSource.Stop();
            return;
        }

        // calculate the shortest path to the sound source
        Stack<Vector2Int> path = pathFinder.AStar(mazeController.maze, playerGridPosition, sourceGridPosition);
        if (path == null || path.Count < 1 || path.Count > range) {
            audioSource.Stop();
            return;
        }
        float actualDistance = path.Count;

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
        Vector3 virtualPosition = playerCellCenter + audioDirection * actualDistance;
        virtualPosition.y = transform.position.y; // do not change the y transform
        audioSource.transform.position = virtualPosition;

        Debug.Log(virtualPosition);
    }
}
