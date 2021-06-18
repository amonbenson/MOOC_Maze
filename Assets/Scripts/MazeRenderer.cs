using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeRenderer : MonoBehaviour
{

    [SerializeField]
    [Range(1,50)]
    private int width = 10;

    [SerializeField]
    [Range(1,50)]
    private int height = 10;

    [SerializeField]
    [Range(0, 10)]
    private int numTokens = 5;

    [SerializeField]
    private float size = 1f;

    [SerializeField]
    private Transform wallPrefab = null;
    [SerializeField]
    private Transform floorPrefab = null;
    [SerializeField]
    private Transform playerPrefab = null;
    [SerializeField]
    private Transform tokenPrefab = null;

    // Start is called before the first frame update
    void Start()
    {
        var maze = MazeGenerator.Generate(width, height);
        Draw(maze);
    }

    private void Draw(WallState[,] maze) 
    {

        var floor = Instantiate(floorPrefab, transform);
        floor.localScale = new Vector3(width, 1, height);

        var player = Instantiate(playerPrefab, transform);
        player.position = new Vector3(-width / 2, 0, -height / 2);
        player.LookAt(Vector3.zero, Vector3.up);

        // generate walls
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                var cell = maze[i, j];
                var position = new Vector3(-width / 2 + i, 1, -height / 2 + j);

                if (cell.HasFlag(WallState.UP))
                {
                    var topWall = Instantiate(wallPrefab, transform) as Transform;
                    topWall.position = position + new Vector3(0, 0, size/2);
                    topWall.localScale = new Vector3(size, topWall.localScale.y, topWall.localScale.z);
                }
                if (cell.HasFlag(WallState.LEFT))
                {
                    var leftWall = Instantiate(wallPrefab, transform) as Transform;
                    leftWall.position = position + new Vector3(-size / 2, 0, 0);
                    leftWall.localScale = new Vector3(size, leftWall.localScale.y, leftWall.localScale.z);
                    leftWall.eulerAngles = new Vector3(0, 90, 0);
                }
                if(i == width - 1)
                {
                    if (cell.HasFlag(WallState.RIGHT))
                    {
                        var rightWall = Instantiate(wallPrefab, transform) as Transform;
                        rightWall.position = position + new Vector3(+size / 2, 0, 0);
                        rightWall.localScale = new Vector3(size, rightWall.localScale.y, rightWall.localScale.z);
                        rightWall.eulerAngles = new Vector3(0, 90, 0);
                    }
                }
                if (j == 0)
                {
                    if (cell.HasFlag(WallState.DOWN))
                    {
                        var bottomWall = Instantiate(wallPrefab, transform) as Transform;
                        bottomWall.position = position + new Vector3(0, 0, -size / 2);
                        bottomWall.localScale = new Vector3(size, bottomWall.localScale.y, bottomWall.localScale.z);
                    }
                }
            }
        }

        // generate tokens TODO: make sure there aren't two tokens placed on the same field
        for (int i = 0; i < numTokens; i++) {
            var position = new Vector3(Random.Range(-width / 2, width / 2 - 1), 1.0f, Random.Range(-width / 2, height / 2 - 1));

            var token = Instantiate(tokenPrefab, transform);
            token.position = position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
