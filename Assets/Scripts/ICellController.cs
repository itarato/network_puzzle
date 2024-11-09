using System.Collections.Generic;

public interface ICellController {
    public void Initialize(ICell cell, Coord coord, GameController gameController, Coord sourceCoord);

    public void GameOver();

    public void TurnOn();

    public void TurnOff();
}
