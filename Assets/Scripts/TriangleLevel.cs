using System.Collections.Generic;

public class TriangleLevel : ILevel {
    public class Cell : ICell {
        /**
         *    Right facing | Left facing
         * ----------------+------------
         * 0: Norhteast    | SouthWest
         * 1: SouthEast    | NorthWest
         * 2: West         | East
         */
        public bool[] paths { get; }
        public bool isEnd { get; }
        public bool isEmpty { get; }
        public bool isStraight { get; }

        public Cell(bool[] paths) {
            if (paths.Length != 3) {
                throw new System.ArgumentException("Cell paths count must be 3");
            }

            this.paths = paths;

            int unitSum = 0;
            int squareSum = 0;
            for (int i = 0; i < 3; i++) {
                if (paths[i]) {
                    unitSum++;
                    squareSum += 1 << i;
                }
            }
            isEnd = unitSum == 1;
            isEmpty = unitSum == 0;
            isStraight = false;
        }

        public void RotateRight() {
            bool oldNorth = paths[0];

            paths[0] = paths[2];
            paths[2] = paths[1];
            paths[1] = oldNorth;
        }
    }

    public int width { get; }
    public int height { get; }
    public List<Cell> cells;
    public Coord source { get; }

    private List<Coord> endCellCoordsCache = new List<Coord>();

    // North, east, south, west.
    static int[,] rightFacingNeighbourMap = { { 0, -1 }, { 0, 1 }, { -1, 0 } };
    static int[,] leftFacingNeighbourMap = { { 0, 1 }, { 0, -1 }, { 1, 0 } };

    public TriangleLevel(int width, int height, List<Cell> cells, Coord source) {
        this.width = width;
        this.height = height;
        this.cells = cells;
        this.source = source;

        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                if (cells[y * width + x].isEnd) {
                    endCellCoordsCache.Add(new Coord(x, y));
                }
            }
        }
    }

    public ICell CellAt(Coord coord) {
        if (!IsValidCoord(coord)) {
            throw new System.Exception("Invalid cell coordinates");
        }

        return cells[coord.y * width + coord.x];
    }

    public List<Coord> ActiveCells(Coord skipCoord) {
        List<Coord> activeCells = new List<Coord>();
        HashSet<Coord> visited = new HashSet<Coord>();

        Queue<Coord> workQueue = new Queue<Coord>();
        workQueue.Enqueue(source);
        visited.Add(source);

        while (workQueue.Count > 0) {
            Coord coord = workQueue.Dequeue();
            activeCells.Add(coord);

            if (coord.Equals(skipCoord)) continue;

            List<Coord> neighbourCoords = Neighbours(coord);
            for (int i = 0; i < neighbourCoords.Count; i++) {
                Coord neighbourCoord = neighbourCoords[i];

                if (!IsValidCoord(neighbourCoord)) continue;
                if (visited.Contains(neighbourCoord)) continue;
                if (neighbourCoord.Equals(skipCoord)) continue;

                int currentDirectionIndex = i;
                // Made the opposites the same on purpose.
                int neighbourDirectionIndex = currentDirectionIndex;

                if (CellAt(coord).paths[currentDirectionIndex] && CellAt(neighbourCoord).paths[neighbourDirectionIndex]) {
                    workQueue.Enqueue(neighbourCoord);
                    visited.Add(neighbourCoord);
                }
            }
        }

        return activeCells;
    }

    public List<Coord> Neighbours(Coord coord) {
        List<Coord> neighbours = new List<Coord>();
        bool isRightFacing = (coord.x + coord.y) % 2 == 0;

        for (int i = 0; i < 3; i++) {
            Coord neighbourCoord;
            if (isRightFacing) {
                neighbourCoord = new Coord(coord.x + rightFacingNeighbourMap[i, 0], coord.y + rightFacingNeighbourMap[i, 1]);
            } else {
                neighbourCoord = new Coord(coord.x + leftFacingNeighbourMap[i, 0], coord.y + leftFacingNeighbourMap[i, 1]);
            }
            neighbours.Add(neighbourCoord);
        }

        return neighbours;
    }

    public bool IsValidCoord(Coord coord) {
        return coord.x >= 0 && coord.y >= 0 && coord.x < width && coord.y < height;
    }

    public bool IsSolution() {
        List<Coord> activeCellCoords = ActiveCells(null);

        foreach (var endCellCoord in endCellCoordsCache) {
            if (!activeCellCoords.Contains(endCellCoord)) return false;
        }

        return true;
    }
}
