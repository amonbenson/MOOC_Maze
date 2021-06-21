using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using FibonacciHeap;

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
        bounds = new BoundsInt(0, 0, 0, maze.size.x, maze.size.y, 1);

        // validation methods
        if (!bounds.Contains((Vector3Int) origin)) return null;
        if (!bounds.Contains((Vector3Int) target)) return null;

        FibonacciHeap<Node, int> openList = new FibonacciHeap<Node, int>(0);
        Dictionary<Vector2Int, Node> closedList = new Dictionary<Vector2Int, Node>();

        Node currentNode = new Node(origin, null, 0, Manhattan(origin, target));
        openList.Insert(new FibonacciHeapNode<Node, int>(currentNode, currentNode.estimatedScore));
        closedList.Add(currentNode.position, currentNode);

        while (!openList.IsEmpty()) {
            currentNode = openList.RemoveMin().Data;

            if (currentNode.position == target) break;

            // check each direction
            foreach ((var direction, var wall) in directions) {
                Vector2Int nextPosition = currentNode.position + direction;

                // ignore if we are off the board or if there is a wall
                if (!bounds.Contains((Vector3Int) nextPosition)) continue;
                if (maze.walls[currentNode.position.x, currentNode.position.y].HasFlag(wall)) continue;

                // calculate the score for this route
                int nextScore = currentNode.routeScore + 1;

                // if the next node exists already, make sure that the nextScore is better than the previous one
                // if the node does not exist, create a new one and add it to the closed list
                Node nextNode;
                if (closedList.TryGetValue(nextPosition, out nextNode)) {
                    if (nextScore >= nextNode.routeScore) continue;
                } else {
                    nextNode = new Node(nextPosition);
                    closedList.Add(nextPosition, nextNode);
                }

                // update the node's parameters
                nextNode.parent = currentNode;
                nextNode.routeScore = nextScore;
                nextNode.estimatedScore = nextScore + Manhattan(nextPosition, target);

                // put the node into the open list
                openList.Insert(new FibonacciHeapNode<Node, int>(nextNode, nextNode.estimatedScore));
            }
        }

        // if the target wasn't reached, no possible path was found
        if (currentNode.position != target) return null;

        // walk back to the origin
        Stack<Vector2Int> path = new Stack<Vector2Int>();
        do {
            path.Push(currentNode.position);
            currentNode = currentNode.parent;
        } while (currentNode != null && currentNode.position != origin);

        return path;
    }

    public int Manhattan(Vector2Int from, Vector2Int to) {
        return Mathf.Abs(from.x - to.x) + Mathf.Abs(from.y - to.y);
    }

    private class Node {
        public Vector2Int position;
        public Node parent;
        public int routeScore;
        public int estimatedScore;

        public Node(Vector2Int position) : this(position, null, int.MaxValue, int.MaxValue) {}

        public Node(Vector2Int position, Node parent, int routeScore, int estimatedScore) {
            this.position = position;
            this.parent = parent;
            this.routeScore = routeScore;
            this.estimatedScore = estimatedScore;
        }
    }
}
