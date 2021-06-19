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
    public const int TOKEN_DENSITY = 5;

    private System.Random random = new System.Random();
    private Maze maze = null;

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

        // entrance and exit
        maze.walls[0, 0] &= ~Maze.WallState.DOWN & ~Maze.WallState.LEFT;
        maze.walls[maze.size.x - 1, maze.size.y - 1] &= ~Maze.WallState.UP & ~Maze.WallState.RIGHT;
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

        // generate a token every TOKEN_DENSITY column and row
        for (int x = random.Next(0, TOKEN_DENSITY); x < maze.size.x; x += TOKEN_DENSITY) {
            for (int y = random.Next(0, TOKEN_DENSITY); y < maze.size.y; y += TOKEN_DENSITY) {
                // offset the position using a normal distribution
                var position = new Vector2Int(
                    Mathf.RoundToInt(NextGaussian(x, TOKEN_DENSITY / 2.0f)),
                    Mathf.RoundToInt(NextGaussian(y, TOKEN_DENSITY / 2.0f))
                );
                position.Clamp(Vector2Int.zero, maze.size - Vector2Int.one);

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
