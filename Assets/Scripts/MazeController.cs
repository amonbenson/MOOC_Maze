using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeController : MonoBehaviour {
    [Range(1, 50)]
    public int width = 10;
    [Range(1, 50)]
    public int height = 10;

    public GameObject wallPrefab = null;
    public GameObject tokenPrefab = null;
    
    public Maze maze = null;

    [Range(0f, 5.0f)]
    public float tokenHeight = 1.0f;
    [Range(1.0f, 5.0f)]
    public float wallWidth = 1.0f;
    [Range(0.0f, 5.0f)]
    public float wallHeight = 3.0f;
    [Range(0f, 1.0f)]
    public float wallDepth = 0.05f;
    [Range(0f, 100f)]
    public float floorTextureScale = 4.0f;

    private Grid grid = null;
    private GameObject player = null;
    private GameObject floor = null;
    private Material floorMaterial;

    private MazeGenerator generator = null;

    void Start() {
        player = transform.Find("Player").gameObject;

        floor = transform.Find("Floor").gameObject;
        floorMaterial = floor.GetComponent<Renderer>().sharedMaterial;

        // setup the grid
        grid = GetComponent<Grid>();
        grid.cellSize = new Vector3(wallWidth, wallWidth, 1.0f);
        grid.cellGap = Vector3.zero;

        // setup the prefabs
        wallPrefab.transform.localScale = new Vector3(wallWidth, wallHeight, wallDepth);

        // TODO: move to GameController
        maze = ScriptableObject.CreateInstance<Maze>();
        maze.Init(new Vector2Int(width, height));

        generator = new MazeGenerator();
        generator.Generate(ref maze);

        StartCoroutine(RenderNextFrame());
    }

    private IEnumerator RenderNextFrame() {
        yield return new WaitForEndOfFrame();
        Render();
    }

    void Update() {
    }

    private Vector3 CellToLocalScale(Vector2 scale) {
        return grid.CellToLocalInterpolated(new Vector3(scale.x, scale.y, 1.0f));
    }

    void Clear() {
        // remove all dynamic objects
        foreach (Transform child in transform) {
            var obj = child.gameObject;
            if (obj.CompareTag("Wall") || obj.CompareTag("Token")) {
                GameObject.Destroy(obj);
            }
        }
    }

    void Render() {
        Clear();
        if (maze == null) return;

        // place the player
        player.transform.localPosition = CellToLocalScale(Vector2.zero);
        player.transform.LookAt(CellToLocalScale(Vector2.one), Vector3.up);

        // resize the floor
        floor.transform.localScale = CellToLocalScale(maze.size);
        floorMaterial.mainTextureScale = (Vector2) maze.size * floorTextureScale;

        // create the walls
        for (int x = 0; x < maze.size.x; x++) {
            for (int y = 0; y < maze.size.y; y++) {
                Maze.WallState wallState = maze.walls[x, y];
                Vector2Int position = new Vector2Int(x, y);

                if (wallState.HasFlag(Maze.WallState.LEFT)) {
                    InstantiateWall(position + new Vector2(0, 0.5f), false);
                }

                if (wallState.HasFlag(Maze.WallState.RIGHT) && x == maze.size.x - 1) {
                    InstantiateWall(position + new Vector2(1.0f, 0.5f), false);
                }

                if (wallState.HasFlag(Maze.WallState.UP)) {
                    InstantiateWall(position + new Vector2(0.5f, 1.0f), true);
                }

                if (wallState.HasFlag(Maze.WallState.DOWN) && y == 0) {
                    InstantiateWall(position + new Vector2(0.5f, 0.0f), true);
                }
            }
        }

        // create the tokens
        foreach (var token in maze.tokens) {
            InstantiateToken(token + new Vector2(0.5f, 0.5f));
        }
    }

    private void InstantiateWall(Vector2 gridPosition, bool horizontal) {
        var wall = Instantiate(wallPrefab,
                grid.CellToLocalInterpolated(gridPosition) + Vector3.up * wallPrefab.transform.position.y,
                horizontal ? Quaternion.identity : Quaternion.AngleAxis(90, Vector3.up),
                transform);
    }

    private void InstantiateToken(Vector2 gridPosition) {
        Instantiate(tokenPrefab,
                grid.CellToLocalInterpolated(gridPosition) + Vector3.up * tokenHeight,
                Quaternion.identity,
                transform);
    }
}
