using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Maze Generator loosly based on: https://github.com/gamedolphin/youtube_unity_maze
*/

public struct Neighbour
{
    public Vector2Int position;
    public Maze.WallState sharedWall;
}

public class MazeGenerator {
    public const int RANDOM_TOKEN_DENSITY = 4;
    public const int PATH_TOKEN_DENSITY = 4;

    private int seed;
    private System.Random random;
    private Maze maze = null;

    public MazeGenerator() {
        seed = GlobalGameSettings.seed;

        random = new System.Random(seed);
        Debug.Log("Generator using seed " + seed);
    }

    private float NextGaussian(float mean, float stdDev) {
        // Box–Muller transform
        float u1 = 1.0f - (float) random.NextDouble();
        float u2 = 1.0f - (float) random.NextDouble();

        float gaussian = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) * Mathf.Sin(2.0f * Mathf.PI * u2);
        return mean + stdDev * gaussian;
    }

    private Maze.WallState GetOppositeWall(Maze.WallState wall) {
        return (Maze.WallState) (((uint) wall >> 2) & 0b0011 | ((uint) wall << 2) & 0b1100 | (uint) wall & ~0b1111);
    }

    private void GenerateWalls() {
        var positionStack = new Stack<Vector2Int>();
        var position = new Vector2Int(random.Next(0, maze.size.x), random.Next(0, maze.size.y));

        maze.walls[position.x, position.y] |= Maze.WallState.VISITED; // 1000 1111
        positionStack.Push(position);

        while (positionStack.Count > 0)
        {
            var current = positionStack.Pop();
            var neighbours = GetUnvisitedNeighbours(current);

            if(neighbours.Count > 0)
            {
                positionStack.Push(current);

                var randIndex = random.Next(0, neighbours.Count);
                var randomNeighbour = neighbours[randIndex];

                var nPosition = randomNeighbour.position;
                maze.walls[current.x, current.y] &= ~randomNeighbour.sharedWall;
                maze.walls[nPosition.x, nPosition.y] &= ~GetOppositeWall(randomNeighbour.sharedWall);

                maze.walls[nPosition.x, nPosition.y] |= Maze.WallState.VISITED;

                positionStack.Push(nPosition);
            }
        }

        // entrance and exit (not used here)
        /*
        maze.walls[0, 0] &= ~Maze.WallState.DOWN & ~Maze.WallState.LEFT;
        maze.walls[maze.size.x - 1, maze.size.y - 1] &= ~Maze.WallState.UP & ~Maze.WallState.RIGHT;
        */
    }

    private List<Neighbour> GetUnvisitedNeighbours(Vector2Int p)
    {
        var list = new List<Neighbour>();

        if (p.x > 0) //left
        {
            if (!maze.walls[p.x - 1, p.y].HasFlag(Maze.WallState.VISITED))
            {
                list.Add(new Neighbour
                {
                    position = p + Vector2Int.left,
                    sharedWall = Maze.WallState.LEFT
                });
            }
        }
        if (p.y > 0) //DOWN
        {
            if (!maze.walls[p.x, p.y - 1].HasFlag(Maze.WallState.VISITED))
            {
                list.Add(new Neighbour
                {
                    position = p + Vector2Int.down,
                    sharedWall = Maze.WallState.DOWN
                });
            }
        }
        if (p.y < maze.size.y - 1) //UP
        {
            if (!maze.walls[p.x, p.y + 1].HasFlag(Maze.WallState.VISITED))
            {
                list.Add(new Neighbour
                {
                    position = p + Vector2Int.up,
                    sharedWall = Maze.WallState.UP
                });
            }
        }
        if (p.x < maze.size.x - 1) //RIGHT
        {
            if (!maze.walls[p.x + 1, p.y].HasFlag(Maze.WallState.VISITED))
            {
                list.Add(new Neighbour
                {
                    position = p + Vector2Int.right,
                    sharedWall = Maze.WallState.RIGHT
                });
            }
        }
        return list;
    }

    private void GenerateTokens() {
        maze.tokens = new List<Vector2Int>();

        // generate a token every RANDOM_TOKEN_DENSITY column and row
        for (int x = random.Next(0, RANDOM_TOKEN_DENSITY); x < maze.size.x; x += RANDOM_TOKEN_DENSITY) {
            for (int y = random.Next(0, RANDOM_TOKEN_DENSITY); y < maze.size.y; y += RANDOM_TOKEN_DENSITY) {
                // offset the position using a normal distribution
                /*var position = new Vector2Int(
                    Mathf.RoundToInt(NextGaussian(x, RANDOM_TOKEN_DENSITY / 2.0f)),
                    Mathf.RoundToInt(NextGaussian(y, RANDOM_TOKEN_DENSITY / 2.0f))
                );
                position.Clamp(Vector2Int.zero, maze.size - Vector2Int.one);*/
                var position = new Vector2Int(x, y);

                // add the token if it does not conver the origin or target
                if (position != Vector2.zero && position != maze.size - Vector2Int.one) {
                    maze.tokens.Add(position);
                }
            }
        }

        // generate additional tokens along the target path every PATH_TOKEN_DENSITY cell
        var mpf = new MazePathFinder();
        Stack<Vector2Int> path = mpf.AStar(maze, Vector2Int.zero, maze.size - Vector2Int.one);
        var pathlength = path.Count;

        while (path.Count > 0) {
            for (var i = 0; i < PATH_TOKEN_DENSITY && path.Count > 0; i++) path.Pop();
            if (path.Count == 0) break;

            // add a token if it does not exist already and does not cover the origin or target
            var position = path.Peek();
            Debug.Log("place token at " + position);
            if (!maze.tokens.Contains(position)
                    && position != Vector2.zero
                    && position != maze.size - Vector2Int.one) {
                maze.tokens.Add(position);
            }
        }
    }

    public Maze Generate(ref Maze maze) {
        this.maze = maze;

        GenerateWalls();
        GenerateTokens();

        return maze;
    }
}
