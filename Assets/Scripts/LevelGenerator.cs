using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator {
    // North, east, south, west.
    static int[,] neighbourMap = { { 0, -1 }, { 1, 0 }, { 0, 1 }, { -1, 0 } };

    public static Level Generate(int width, int height) {
        List<Level.Cell> cells = new List<Level.Cell>();
        cells.Add(new Level.Cell(new bool[] { false, true, false, false }));
        cells.Add(new Level.Cell(new bool[] { false, false, true, true }));
        cells.Add(new Level.Cell(new bool[] { false, true, false, false }));
        cells.Add(new Level.Cell(new bool[] { true, false, false, true }));

        Level level = new Level(2, 2, cells, new Coord(0, 0));

        for (int i = 0; i < 3; i++) {
            RandomizeLevel(level);
            if (!level.IsSolution()) break;
        }

        return level;
    }

    private static Level GenerateRandomLevel(int width, int height) {
        System.Random rand = new System.Random();
        List<Level.Cell> cells = new List<Level.Cell>();
        Coord sourceCoord = new Coord(rand.Next(0, width), rand.Next(0, height));

        bool[,] map = new bool[width * height, 4];
        Queue<Coord> queue = new Queue<Coord>();
        queue.Enqueue(sourceCoord);

        while (queue.Count > 0) {
            Coord coord = queue.Dequeue();
            int coordIndex = coord.y * width + coord.x;

            // Find all neighbours.
            Dictionary<int, Coord> neighbourCoords = new Dictionary<int, Coord>();
            int stemCount = 0;
            for (int i = 0; i < 4; i++) {
                Coord neighbourCoord = new Coord(coord.x + neighbourMap[i, 0], coord.y + neighbourMap[i, 1]);
                int neighbourCoordIndex = neighbourCoord.y * width + neighbourCoord.x;

                if (neighbourCoord.x < 0 || neighbourCoord.y < 0 || neighbourCoord.x >= width || neighbourCoord.y >= height) {
                    continue;
                }

                // Remove the one on the stem side (if there is).
                if (map[coordIndex, i]) {
                    stemCount++;
                    continue;
                }

                // Remove non empty neighbour facing ones.
                if (map[neighbourCoordIndex, 0] || map[neighbourCoordIndex, 1] || map[neighbourCoordIndex, 2] || map[neighbourCoordIndex, 3]) {
                    continue;
                }

                neighbourCoords.Add(i, neighbourCoord);
            }

            if (stemCount > 1) Debug.LogError("Stem count over 1");



            // Pick a few random (1-3).
            // Make connection (both cells)
            // Enqueue it.
        }

        Level level = new Level(width, height, cells, sourceCoord);
        return level;
    }

    private static void RandomizeLevel(Level level) {
        System.Random rand = new System.Random();

        foreach (var cell in level.cells) {
            for (int i = 0; i < rand.Next(0, 4); i++) {
                cell.RotateRight();
            }
        }
    }
}
