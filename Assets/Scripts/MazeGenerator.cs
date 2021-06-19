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
    public const int TOKEN_DENSITY = 3;

    private System.Random random = new System.Random();
    private Maze maze = null;

    private Maze.WallState GetOppositeWall(Maze.WallState wall) {
        return (Maze.WallState) ((uint) wall >> 2 | (uint) wall << 2) & Maze.WallState.FULL;
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

        maze.walls[0, 0] &= Maze.WallState.UP;
        maze.walls[maze.size.x - 1, maze.size.y - 1] &= Maze.WallState.DOWN;
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
        for (int x = 0; x < maze.size.x; x += TOKEN_DENSITY) {
            for (int y = 0; y < maze.size.y; y += TOKEN_DENSITY) {
                maze.tokens.Add(new Vector2Int(x, y)); // TODO: add normally distributed offset to position
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
