using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
    public GameObject cellTemplate;

    private Level currentLevel;
    private List<GameObject> cellInstances = new List<GameObject>();

    void Start() {
        List<Level.Cell> cells = new List<Level.Cell>();
        cells.Add(new Level.Cell(new bool[] { false, true, false, false }));
        cells.Add(new Level.Cell(new bool[] { false, false, true, true }));
        cells.Add(new Level.Cell(new bool[] { false, true, false, false }));
        cells.Add(new Level.Cell(new bool[] { true, false, false, true }));

        currentLevel = new Level(2, 2, cells, new Coord(0, 0));

        Reload();
    }

    void Reload() {
        cellInstances.Clear();

        for (int z = 0; z < currentLevel.height; z++) {
            for (int x = 0; x < currentLevel.width; x++) {
                Vector3 pos = new Vector3(
                    transform.position.x - ((float)currentLevel.width / 2f) + (float)x,
                    transform.position.y,
                    transform.position.z + ((float)currentLevel.height / 2f) - (float)z
                );

                GameObject cellInstance = Instantiate(cellTemplate, pos, Quaternion.identity);
                cellInstances.Add(cellInstance);

                CellController cellController = cellInstance.GetComponent<CellController>();
                Coord cellCoord = new Coord(x, z);
                cellController.Initialize(currentLevel.CellAt(cellCoord), cellCoord, this);
            }
        }

        UpdateCellGridState(null);
    }

    void Update() {

    }

    public void UpdateLevelAfterRotation(Coord cellCoord) {
        currentLevel.CellAt(cellCoord).RotateRight();
        UpdateCellGridState(null);
    }

    public void UpdateLevelBeforeRotation(Coord skipCoord) {
        UpdateCellGridState(skipCoord);
    }

    private void UpdateCellGridState(Coord skipCoord) {
        foreach (var cellInstance in cellInstances) {
            CellController cellController = cellInstance.GetComponent<CellController>();
            cellController.TurnOff();
        }

        List<Coord> activeCells = currentLevel.ActiveCells(skipCoord);
        foreach (var activeCellCoord in activeCells) {
            GameObject cellInstance = cellInstances[activeCellCoord.y * currentLevel.width + activeCellCoord.x];
            CellController cellController = cellInstance.GetComponent<CellController>();
            cellController.TurnOn();
        }
    }
}
