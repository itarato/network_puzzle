public interface ICell {
    public bool isEnd { get; }
    public bool[] paths { get; }
    public bool isEmpty { get; }
    public bool isStraight { get; }

    public void RotateRight();
}
