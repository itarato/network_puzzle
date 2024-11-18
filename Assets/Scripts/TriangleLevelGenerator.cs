using System;
using System.Collections.Generic;
using System.Linq;

public class TriangleLevelGenerator {
    // NorthEast, SouthEast, West.
    static int[,] rightFacingNeighbourMap = { { 0, -1 }, { 0, 1 }, { -1, 0 } };
    // SouthWest, NorthWest, East.
    static int[,] leftFacingNeighbourMap = { { 0, 1 }, { 0, -1 }, { 1, 0 } };

    public static TriangleLevel Generate(int width, int height, int randomizerAttempt = 3) {
        TriangleLevel level = GenerateRandomLevel(width, height);

        for (int i = 0; i < randomizerAttempt; i++) {
            RandomizeLevel(level);
            if (!level.IsSolution()) break;
        }

        return level;
    }

    private static TriangleLevel GenerateRandomLevel(int width, int height) {
        System.Random rnd = new System.Random();
        Coord sourceCoord = new Coord(rnd.Next(0, width), rnd.Next(0, height));

        bool[,] cellsBuilderMap = new bool[width * height, 3];
        Queue<Coord> workQueue = new Queue<Coord>();
        workQueue.Enqueue(sourceCoord);

        while (workQueue.Count > 0) {
            Coord currentCoord = workQueue.Dequeue();
            int currentCoordIndex = currentCoord.y * width + currentCoord.x;

            // Find all neighbours.
            Dictionary<int, Coord> neighbourCoords = new Dictionary<int, Coord>();
            int stemCount = 0;
            for (int i = 0; i < 3; i++) {
                Coord neighbourCoord;

                if ((currentCoord.x + currentCoord.y) % 2 == 0) {
                    neighbourCoord = new Coord(
                        currentCoord.x + rightFacingNeighbourMap[i, 0],
                        currentCoord.y + rightFacingNeighbourMap[i, 1]
                    );
                } else {
                    neighbourCoord = new Coord(
                        currentCoord.x + leftFacingNeighbourMap[i, 0],
                        currentCoord.y + leftFacingNeighbourMap[i, 1]
                    );
                }
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
                if (cellsBuilderMap[neighbourCoordIndex, 0] || cellsBuilderMap[neighbourCoordIndex, 1] || cellsBuilderMap[neighbourCoordIndex, 2]) {
                    continue;
                }

                neighbourCoords.Add(i, neighbourCoord);
            }

            // Pick a few random (1-3).
            int newNeighbourCount = rnd.Next(Math.Min(1, neighbourCoords.Count), Math.Min(2, neighbourCoords.Count));
            List<int> keys = neighbourCoords.Keys.ToList<int>();
            for (int i = 0; i < newNeighbourCount; i++) {
                int randomKeyIndex = rnd.Next(keys.Count);
                int randomKey = keys[randomKeyIndex];
                keys.RemoveAt(randomKeyIndex);

                // Make connection (both cells)
                int currentCoordDirection = randomKey;
                int neighbourCoordDirection = currentCoordDirection;
                Coord neighbourCoord = neighbourCoords[randomKey];
                int neighbourCoordIndex = neighbourCoord.y * width + neighbourCoord.x;

                cellsBuilderMap[currentCoordIndex, currentCoordDirection] = true;
                cellsBuilderMap[neighbourCoordIndex, neighbourCoordDirection] = true;

                // Enqueue it.
                workQueue.Enqueue(neighbourCoord);
            }
        }

        List<TriangleLevel.Cell> cells = new List<TriangleLevel.Cell>();
        for (int i = 0; i < width * height; i++) {
            cells.Add(new TriangleLevel.Cell(new bool[] {
                cellsBuilderMap[i, 0],
                cellsBuilderMap[i, 1],
                cellsBuilderMap[i, 2],
            }));
        }

        TriangleLevel level = new TriangleLevel(width, height, cells, sourceCoord);
        return level;
    }

    private static void RandomizeLevel(TriangleLevel level) {
        System.Random rand = new System.Random();

        foreach (var cell in level.cells) {
            for (int i = 0; i < rand.Next(0, 3); i++) {
                cell.RotateRight();
            }
        }
    }
}
