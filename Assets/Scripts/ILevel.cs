using System.Collections.Generic;

public interface ILevel {
    public int height { get; }
    public int width { get; }
    public Coord source {  get; }

    public bool IsSolution();

    public List<Coord> ActiveCells(Coord skipCoord);

    public ICell CellAt(Coord coord);
}
