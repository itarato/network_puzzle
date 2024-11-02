using System;
using System.Collections.Generic;

public class LevelGenerator {
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

    private static void RandomizeLevel(Level level) {
        Random rand = new Random();

        foreach (var cell in level.cells) {
            for (int i = 0; i < rand.Next(0, 4); i++) {
                cell.RotateRight();
            }
        }
    }
}
