using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Maze : ScriptableObject {
    public Vector2Int size;

    public WallState[,] walls;
    public List<Vector2Int> tokens;

    public void Init(Vector2Int size) {
        this.size = size;

        walls = new WallState[size.x, size.y];
        for (int x = 0; x < size.x; x++) {
            for (int y = 0; y < size.y; y++) {
                walls[x, y] = WallState.FULL;
            }
        }

        tokens = new List<Vector2Int>();
    }

    [Flags]
    public enum WallState {
        UP      = 1 << 0,
        RIGHT   = 1 << 1,
        DOWN    = 1 << 2,
        LEFT    = 1 << 3,

        EMPTY   = 0,
        FULL    = UP | RIGHT | DOWN | LEFT,

        VISITED = 1 << 7 // generator specific flag
    }
}
