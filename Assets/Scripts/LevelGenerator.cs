using System;
using System.Collections.Generic;
using System.Linq;
//using UnityEngine;

public class LevelGenerator {
    // North, east, south, west.
    static int[,] neighbourMap = { { 0, -1 }, { 1, 0 }, { 0, 1 }, { -1, 0 } };

    public static Level Generate(int width, int height) {
        Level level = GenerateRandomLevel(width, height);

        for (int i = 0; i < 3; i++) {
            RandomizeLevel(level);
            if (!level.IsSolution()) break;
        }

        return level;
    }

    private static Level GenerateRandomLevel(int width, int height) {
        System.Random rnd = new System.Random();
        Coord sourceCoord = new Coord(rnd.Next(0, width), rnd.Next(0, height));

        bool[,] cellsBuilderMap = new bool[width * height, 4];
        Queue<Coord> workQueue = new Queue<Coord>();
        workQueue.Enqueue(sourceCoord);

        while (workQueue.Count > 0) {
            Coord currentCoord = workQueue.Dequeue();
            int currentCoordIndex = currentCoord.y * width + currentCoord.x;

            // Find all neighbours.
            Dictionary<int, Coord> neighbourCoords = new Dictionary<int, Coord>();
            int stemCount = 0;
            for (int i = 0; i < 4; i++) {
                Coord neighbourCoord = new Coord(currentCoord.x + neighbourMap[i, 0], currentCoord.y + neighbourMap[i, 1]);
                int neighbourCoordIndex = neighbourCoord.y * width + neighbourCoord.x;

                if (neighbourCoord.x < 0 || neighbourCoord.y < 0 || neighbourCoord.x >= width || neighbourCoord.y >= height) {
                    continue;
                }

                // Remove the one on the stem side (if there is).
                if (cellsBuilderMap[currentCoordIndex, i]) {
                    stemCount++;
                    continue;
                }

                // Remove non empty neighbour facing ones.
                if (cellsBuilderMap[neighbourCoordIndex, 0] || cellsBuilderMap[neighbourCoordIndex, 1] || cellsBuilderMap[neighbourCoordIndex, 2] || cellsBuilderMap[neighbourCoordIndex, 3]) {
                    continue;
                }

                neighbourCoords.Add(i, neighbourCoord);
            }

            //if (stemCount > 1) Debug.LogError("Stem count over 1");

            // Pick a few random (1-3).
            int newNeighbourCount = rnd.Next(Math.Min(1, neighbourCoords.Count), Math.Min(3, neighbourCoords.Count));
            List<int> keys = neighbourCoords.Keys.ToList<int>();
            for (int i = 0; i < newNeighbourCount; i++) {
                int randomKeyIndex = rnd.Next(keys.Count);
                int randomKey = keys[randomKeyIndex];
                keys.RemoveAt(randomKeyIndex);

                // Make connection (both cells)
                int currentCoordDirection = randomKey;
                int neighbourCoordDirection = (currentCoordDirection + 2) % 4;
                Coord neighbourCoord = neighbourCoords[randomKey];
                int neighbourCoordIndex = neighbourCoord.y * width + neighbourCoord.x;

                cellsBuilderMap[currentCoordIndex, currentCoordDirection] = true;
                cellsBuilderMap[neighbourCoordIndex, neighbourCoordDirection] = true;
                
                // Enqueue it.
                workQueue.Enqueue(neighbourCoord);
            }
        }

        List<Level.Cell> cells = new List<Level.Cell>();
        for (int i = 0; i < width * height; i++) {
            cells.Add(new Level.Cell(new bool[] {
                cellsBuilderMap[i, 0],
                cellsBuilderMap[i, 1],
                cellsBuilderMap[i, 2],
                cellsBuilderMap[i, 3],
            }));
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
