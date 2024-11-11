#nullable enable

public class Coord {
    public int x;
    public int y;

    public Coord(int x, int y) {
        this.x = x;
        this.y = y;
    }

    public override string ToString() {
        return "<Coord " + x.ToString() + ":" + y.ToString() + ">";
    }

    public override int GetHashCode() {
        return (x.ToString() + ":" + y.ToString()).GetHashCode();
    }

    public override bool Equals(object? obj) {
        if (obj is Coord other) {
            return x == other.x && y == other.y;
        }
        return false;
    }
}
