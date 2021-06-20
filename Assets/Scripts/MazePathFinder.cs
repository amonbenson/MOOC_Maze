using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class MazePathFinder {
    private static readonly (Vector2Int, Maze.WallState)[] directions = {
            (Vector2Int.up, Maze.WallState.UP),
            (Vector2Int.right, Maze.WallState.RIGHT),
            (Vector2Int.down, Maze.WallState.DOWN),
            (Vector2Int.left, Maze.WallState.LEFT),
    };

    private Maze maze;
    private BoundsInt bounds;

    public Stack<Vector2Int> AStar(Maze maze, Vector2Int origin, Vector2Int target) {
        this.maze = maze;
        bounds = new BoundsInt(0, 0, 0, maze.size.x - 1, maze.size.y - 1, 1);

        // validation methods
        if (!bounds.Contains((Vector3Int) origin)) return null;
        if (!bounds.Contains((Vector3Int) target)) return null;

        List<Node> openList = new List<Node>();
        List<Node> closedList = new List<Node>();
        List<Node> adjacencies;

        Node current = new Node(origin);
        openList.Add(current);

        int counter = 100;
        while (openList.Count > 0 && !closedList.Exists(x => x.position == target) && counter-- > 0) {
            current = openList[0];
            openList.Remove(current);
            closedList.Add(current);

            adjacencies = GetAdjacentNodes(current);
            foreach (var n in adjacencies) {
                if (closedList.Contains(n)) continue;
                if (openList.Contains(n)) continue;

                // use manhattan distance to target as heuristic and increment the cost
                n.parent = current;
                n.heuristic = Math.Abs(n.position.x - target.x) + Math.Abs(n.position.y - target.y);
                n.cost = n.parent.cost + 1;

                openList.Add(n);
                openList = openList.OrderBy(node => node.F).ToList<Node>();
            }
        }

        if (!closedList.Exists(x => x.position == target)) return null;
        if (current == null) return null;

        // walk back to the origin
        Stack<Vector2Int> path = new Stack<Vector2Int>();
        do {
            path.Push(current.position);
            current = current.parent;
        } while (current != null && current.position != origin);

        return path;
    }

    private List<Node> GetAdjacentNodes(Node node) {
        List<Node> adjacencies = new List<Node>();

        foreach ((var direction, var wall) in directions) {
            Vector2Int p = node.position + direction;

            if (bounds.Contains((Vector3Int) p) && !maze.walls[node.position.x, node.position.y].HasFlag(wall)) {
                adjacencies.Add(new Node(p));
            }
        }

        return adjacencies;
    }

    private class Node {
        public Vector2Int position;

        public Node parent;
        public float heuristic;
        public float cost;

        public float F {
            get {
                if (heuristic == -1 || cost == -1) return -1;
                else return heuristic + cost;
            }
        }

        public Node(Vector2Int position) {
            this.position = position;

            parent = null;
            heuristic = -1;
            cost = 1;
        }
    }
}
