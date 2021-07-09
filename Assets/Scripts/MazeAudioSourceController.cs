using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeAudioSourceController : MonoBehaviour {
    public bool randomStart = true;
    [Range(0.0f, 10.0f)]
    public float updateSpeed = 3f; 

    private AudioSource audioSource;
    private AudioReverbFilter audioReverbFilter;

    private MazeController mazeController;
    private PlayerController playerController;

    private Vector2Int previousSourceGridPosition = Vector2Int.zero;
    private Vector2Int sourceGridPosition = Vector2Int.zero;
    private Vector2Int playerGridPosition = Vector2Int.zero;
    private Vector3 sourceVirtualWorldPosition = Vector3.zero;

    private MazePathFinder pathFinder = new MazePathFinder();

    private float posx= 0.0f;
    private float posy= 0.0f;

    void Start() {
        audioSource = transform.GetComponentInChildren<AudioSource>();
        audioReverbFilter = GetComponentInChildren<AudioReverbFilter>();

        mazeController = GetComponentInParent<MazeController>();
        if (mazeController == null) {
            Debug.LogError("Maze Audio Source must be placed inside a Maze context");
            return;
        }

        playerController = mazeController.GetComponentInChildren<PlayerController>();
        if (playerController == null) {
            Debug.LogError("No player found");
            return;
        }

        playerController.gridPositionChangeEvent.AddListener(OnGridPositionChange);

        // Sound Position has to been set
        Grid grid = GetComponentInChildren<Grid>();
        transform.position = grid.GetCellCenterLocal(new Vector3Int(mazeController.width, mazeController.height, 0));
        
        //radius has to be set (english main lul)
        if (posx >= posy){audioSource.maxDistance= posx;}//1.9f;}
        else {audioSource.maxDistance= posy;}//1.9f;}
        


        // seek to a random position
        if (randomStart) audioSource.time = Random.Range(0, audioSource.clip.length);
    }

    void Update() {
        // get the source position
        sourceGridPosition = (Vector2Int) mazeController.grid.WorldToCell(transform.position);

        // invoke an update if the source grid position changes
        if (sourceGridPosition != previousSourceGridPosition) {
            RecalculateVirtualPosition();
            previousSourceGridPosition = sourceGridPosition;
        }

        // smoothly update the actual audio source position
        Vector3 virtualLocalPosition = audioSource.transform.InverseTransformPoint(sourceVirtualWorldPosition);
        audioSource.transform.localPosition = Vector3.Lerp(audioSource.transform.localPosition,
                virtualLocalPosition,
                Time.deltaTime * updateSpeed);
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
        Debug.Log(playerGridPosition + " : " + sourceGridPosition + " : " + mazeController.maze.size);
        Stack<Vector2Int> path = pathFinder.AStar(mazeController.maze, playerGridPosition, sourceGridPosition);
        if (path == null) return;
        float distance = path.Count;

        // average the next few cell center positions
        int i = 0, n = 2;
        Vector3 averageCellCenter = Vector3.zero;
        for (; i < n && path.Count > 0; i++) {
            Vector2Int cell = path.Pop();
            averageCellCenter += mazeController.grid.GetCellCenterWorld((Vector3Int) cell);
        }
        if (i != 0) averageCellCenter /= i;

        // use the distance from the player to the averaged cell center to get the audio direction
        Vector3 playerCellCenter = mazeController.grid.GetCellCenterWorld((Vector3Int) playerGridPosition);
        Vector3 audioDirection = (averageCellCenter - playerCellCenter);
        audioDirection.Normalize();

        // set the virtual position in world coordinates
        sourceVirtualWorldPosition = playerCellCenter + audioDirection * distance;
        sourceVirtualWorldPosition.y = transform.position.y; // do not change the y transform

        // play the audio
        if (!audioSource.isPlaying) audioSource.Play();
        
        // update the reverb amount
        audioReverbFilter.room = Mathf.Lerp(-10000, 0, distance / audioSource.maxDistance);
    }
}
