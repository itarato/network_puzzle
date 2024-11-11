using System.Collections.Generic;

public class Level : ILevel {
    public class Cell : ICell {
        /**
         * 0: North
         * 1: East
         * 2: South
         * 3: West
         */
        public bool[] paths { get; }
        public bool isEnd { get; }
        public bool isEmpty { get; }
        public bool isStraight { get; }

        public Cell(bool[] paths) {
            if (paths.Length != 4) {
                throw new System.ArgumentException("Cell paths count must be 4");
            }

            this.paths = paths;
            
            int unitSum = 0;
            int squareSum = 0;
            for (int i = 0; i < 4; i++) {
                if (paths[i]) {
                    unitSum++;
                    squareSum += 1 << i;
                }
            }
            isEnd = unitSum == 1;
            isEmpty = unitSum == 0;
            isStraight = squareSum == 5 || squareSum == 10;
        }

        public void RotateLeft() {
            bool oldNorth = paths[0];

            paths[0] = paths[1];
            paths[1] = paths[2];
            paths[2] = paths[3];
            paths[3] = oldNorth;
        }

        public void RotateRight() {
            bool oldNorth = paths[0];

            paths[0] = paths[3];
            paths[3] = paths[2];
            paths[2] = paths[1];
            paths[1] = oldNorth;
        }
    }

    public int width { get; }
    public int height { get; }
    public List<Cell> cells;
    public Coord source { get; }

    private bool isAcrossBorders;
    private List<Coord> endCellCoordsCache = new List<Coord>();

    // North, east, south, west.
    static int[,] neighbourMap = { { 0, -1 }, { 1, 0 }, { 0, 1 }, { -1, 0 } };

    public Level(int width, int height, List<Cell> cells, Coord source, bool isAcrossBorders) {
        this.width = width;
        this.height = height;
        this.cells = cells;
        this.source = source;
        this.isAcrossBorders = isAcrossBorders;

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
                int neighbourDirectionIndex = (currentDirectionIndex + 2) % 4;

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

        for (int i = 0; i < 4; i++) {
            Coord neighbourCoord;
            if (isAcrossBorders) {
                neighbourCoord = new Coord(
                    (coord.x + neighbourMap[i, 0] + width) % width,
                    (coord.y + neighbourMap[i, 1] + height) % height
                );
            } else {
                neighbourCoord = new Coord(coord.x + neighbourMap[i, 0], coord.y + neighbourMap[i, 1]);
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
