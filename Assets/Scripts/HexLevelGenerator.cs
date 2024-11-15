using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HexLevelGenerator {
    // NorthWest, NorthEast, East, SouthEast, SouthWest, West.
    static int[,] evenNeighbourMap = { { 0, -1 }, { 1, -1 }, { 1, 0 }, { 1, 1 }, { 0, 1 }, { -1, 0 } };
    static int[,] oddNeighbourMap = { { -1, -1 }, { 0, -1 }, { 1, 0 }, { 0, 1 }, { -1, 1 }, { -1, 0 } };

    public static HexLevel Generate(int width, int height, bool isAcrossBorders) {
        HexLevel level = GenerateRandomLevel(width, height, isAcrossBorders);

        for (int i = 0; i < 3; i++) {
            RandomizeLevel(level);
            if (!level.IsSolution()) break;
        }

        return level;
    }

    private static HexLevel GenerateRandomLevel(int width, int height, bool isAcrossBorders) {
        System.Random rnd = new System.Random();
        Coord sourceCoord = new Coord(rnd.Next(0, width), rnd.Next(0, height));

        bool[,] cellsBuilderMap = new bool[width * height, 6];
        Queue<Coord> workQueue = new Queue<Coord>();
        workQueue.Enqueue(sourceCoord);

        while (workQueue.Count > 0) {
            Coord currentCoord = workQueue.Dequeue();
            int currentCoordIndex = currentCoord.y * width + currentCoord.x;

            // Find all neighbours.
            Dictionary<int, Coord> neighbourCoords = new Dictionary<int, Coord>();
            int stemCount = 0;
            for (int i = 0; i < 6; i++) {
                Coord neighbourCoord;

                if (currentCoord.y % 2 == 0) {
                    neighbourCoord = new Coord(currentCoord.x + evenNeighbourMap[i, 0], currentCoord.y + evenNeighbourMap[i, 1]);
                } else {
                    neighbourCoord = new Coord(currentCoord.x + oddNeighbourMap[i, 0], currentCoord.y + oddNeighbourMap[i, 1]);
                }

                if (isAcrossBorders) {
                    neighbourCoord = HexLevel.CrossBorderNeighbour(neighbourCoord, currentCoord, i, width, height);
                }

                if (neighbourCoord.x < 0 || neighbourCoord.y < 0 || neighbourCoord.x >= width || neighbourCoord.y >= height) {
                    continue;
                }

                // Remove the one on the stem side (if there is).
                if (cellsBuilderMap[currentCoordIndex, i]) {
                    stemCount++;
                    continue;
                }

                int neighbourCoordIndex = neighbourCoord.y * width + neighbourCoord.x;
                // Remove non empty neighbour facing ones.
                if (
                    cellsBuilderMap[neighbourCoordIndex, 0] ||
                    cellsBuilderMap[neighbourCoordIndex, 1] ||
                    cellsBuilderMap[neighbourCoordIndex, 2] ||
                    cellsBuilderMap[neighbourCoordIndex, 3] ||
                    cellsBuilderMap[neighbourCoordIndex, 4] ||
                    cellsBuilderMap[neighbourCoordIndex, 5]
                ) {
                    continue;
                }

                neighbourCoords.Add(i, neighbourCoord);
            }

            // Pick a few random (1-3).
            int newNeighbourCount = rnd.Next(Math.Min(1, neighbourCoords.Count), Math.Min(5, neighbourCoords.Count));
            List<int> keys = neighbourCoords.Keys.ToList<int>();
            for (int i = 0; i < newNeighbourCount; i++) {
                int randomKeyIndex = rnd.Next(keys.Count);
                int randomKey = keys[randomKeyIndex];
                keys.RemoveAt(randomKeyIndex);

                // Make connection (both cells)
                int currentCoordDirection = randomKey;
                int neighbourCoordDirection = (currentCoordDirection + 3) % 6;
                Coord neighbourCoord = neighbourCoords[randomKey];
                int neighbourCoordIndex = neighbourCoord.y * width + neighbourCoord.x;

                cellsBuilderMap[currentCoordIndex, currentCoordDirection] = true;
                cellsBuilderMap[neighbourCoordIndex, neighbourCoordDirection] = true;

                // Enqueue it.
                workQueue.Enqueue(neighbourCoord);
            }
        }

        List<HexLevel.Cell> cells = new List<HexLevel.Cell>();
        for (int i = 0; i < width * height; i++) {
            cells.Add(new HexLevel.Cell(new bool[] {
                cellsBuilderMap[i, 0],
                cellsBuilderMap[i, 1],
                cellsBuilderMap[i, 2],
                cellsBuilderMap[i, 3],
                cellsBuilderMap[i, 4],
                cellsBuilderMap[i, 5],
            }));
        }

        HexLevel level = new HexLevel(width, height, cells, sourceCoord, isAcrossBorders);
        return level;
    }

    private static void RandomizeLevel(HexLevel level) {
        System.Random rand = new System.Random();

        foreach (var cell in level.cells) {
            for (int i = 0; i < rand.Next(0, 6); i++) {
                cell.RotateRight();
            }
        }
    }
}
