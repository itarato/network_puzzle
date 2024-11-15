using System;
using System.Collections.Generic;
using TMPro;

public class HexLevel : ILevel {
    public class Cell : ICell {
        /**
         * 0: NorthWest
         * 1: NorthEast
         * 2: East
         * 3: SouthEast
         * 4: SouthWest
         * 5: West
         */
        public bool[] paths { get; }
        public bool isEnd { get; }
        public bool isEmpty { get; }
        public bool isStraight { get; }

        public Cell(bool[] paths) {
            if (paths.Length != 6) {
                throw new System.ArgumentException("Cell paths count must be 6");
            }

            this.paths = paths;

            int unitSum = 0;
            int squareSum = 0;
            for (int i = 0; i < 6; i++) {
                if (paths[i]) {
                    unitSum++;
                    squareSum += 1 << i;
                }
            }
            isEnd = unitSum == 1;
            isEmpty = unitSum == 0;
            isStraight = squareSum == 9 || squareSum == 18 || squareSum == 36;
        }

        public void RotateLeft() {
            bool oldNorth = paths[0];

            paths[0] = paths[1];
            paths[1] = paths[2];
            paths[2] = paths[3];
            paths[3] = paths[4];
            paths[4] = paths[5];
            paths[5] = oldNorth;
        }

        public void RotateRight() {
            bool oldNorth = paths[0];

            paths[0] = paths[5];
            paths[5] = paths[4];
            paths[4] = paths[3];
            paths[3] = paths[2];
            paths[2] = paths[1];
            paths[1] = oldNorth;
        }
    }

    public int width { get; }
    public int height { get; }
    public List<Cell> cells;
    public Coord source { get; }

    private List<Coord> endCellCoordsCache = new List<Coord>();

    private bool isAcrossBorders;

    // NorthWest, NorthEast, East, SouthEast, SouthWest, West.
    static int[,] evenNeighbourMap = { { 0, -1 }, { 1, -1 }, { 1, 0 }, { 1, 1 }, { 0, 1 }, { -1, 0 } };
    static int[,] oddNeighbourMap = { { -1, -1 }, { 0, -1 }, { 1, 0 }, { 0, 1 }, { -1, 1 }, { -1, 0 } };

    public HexLevel(int width, int height, List<Cell> cells, Coord source, bool isAcrossBorders) {
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
                int neighbourDirectionIndex = (currentDirectionIndex + 3) % 6;

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

        for (int i = 0; i < 6; i++) {
            Coord neighbourCoord;
            if (coord.y % 2 == 0) {
                neighbourCoord = new Coord(coord.x + evenNeighbourMap[i, 0], coord.y + evenNeighbourMap[i, 1]);
            } else {
                neighbourCoord = new Coord(coord.x + oddNeighbourMap[i, 0], coord.y + oddNeighbourMap[i, 1]);
            }

            if (isAcrossBorders) {
                neighbourCoord = CrossBorderNeighbour(neighbourCoord, coord, i, width, height);
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

    public static Coord CrossBorderNeighbour(Coord coord, Coord originCoord, int dirIndex, int width, int height) {
        if (coord.x >= 0 && coord.y >= 0 && coord.x < width && coord.y < height) {
            return coord;
        }

        Coord newCoord;

        if (dirIndex == 2 || dirIndex == 5) {
            return new Coord((coord.x + width) % width, coord.y);
        } else if (dirIndex == 0) {
            int maxX = (width - 1 - coord.x) * 2;
            int maxY = height - 1 - coord.y;

            if (maxX < maxY) {
                if (coord.y % 2 == 0) {
                    newCoord = new Coord(width - 1, coord.y + maxX);
                } else {
                    newCoord = new Coord(width - 1, coord.y + maxX + 1);
                }
            } else {
                newCoord = new Coord(coord.x + (int)Math.Floor(maxY / 2f), height - 1);
            }
        } else if (dirIndex == 1) { // BUG
            int maxX = coord.x * 2;
            int maxY = height - 1 - coord.y;

            if (maxX < maxY) {
                if (coord.y % 2 == 0) {
                    throw new Exception("Direction 1 cannot have even Y coord here.");
                } else {
                    newCoord = new Coord(0, coord.y + maxX);
                }
            } else {
                if (coord.y % 2 == 0) {
                    throw new Exception("Direction 1 cannot have even Y coord here.");
                } else {
                    newCoord = new Coord(coord.x - (int)Math.Ceiling(maxY / 2f), height - 1);
                }
            }
        } else if (dirIndex == 3) {
            int maxX = coord.x * 2;
            int maxY = coord.y;

            if (maxX < maxY) {
                if (coord.y % 2 == 0) {
                    newCoord = new Coord(0, coord.y - (maxX + 1));
                } else {
                    newCoord = new Coord(0, coord.y - maxX);
                }
            } else {
                newCoord = new Coord(coord.x - (int)Math.Ceiling(maxY / 2f), 0);
            }
        } else if (dirIndex == 4) { // BUG
            int maxX = (width - 1 - coord.x) * 2;
            int maxY = coord.y;

            if (maxX < maxY) {
                if (coord.y % 2 == 0) {
                    newCoord = new Coord(width - 1, coord.y - maxX);
                } else {
                    newCoord = new Coord(width - 1, coord.y - (maxX + 1));
                }
            } else {
                newCoord = new Coord(coord.x + (int)Math.Floor(maxY / 2f), 0);
            }
        } else {
            throw new Exception("Invalid dir index");
        }

        if (newCoord.Equals(originCoord)) {
            return coord;
        }

        return newCoord;
    }
}
