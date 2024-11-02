using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
    public GameObject cellTemplate;

    private Level currentLevel;
    private List<GameObject> cellInstances = new List<GameObject>();

    void Start() {
        Reload();
    }

    void Reload() {
        currentLevel = LevelGenerator.Generate(2, 2);

        foreach (var cellInstance in cellInstances) {
            Destroy(cellInstance.gameObject);
        }
        cellInstances.Clear();

        for (int z = 0; z < currentLevel.height; z++) {
            for (int x = 0; x < currentLevel.width; x++) {
                Vector3 pos = new Vector3(
                    transform.position.x - (currentLevel.width / 2f) + x,
                    transform.position.y,
                    transform.position.z + (currentLevel.height / 2f) - z
                );

                GameObject cellInstance = Instantiate(cellTemplate, pos, Quaternion.identity);
                cellInstances.Add(cellInstance);

                CellController cellController = cellInstance.GetComponent<CellController>();
                Coord cellCoord = new Coord(x, z);
                cellController.Initialize(currentLevel.CellAt(cellCoord), cellCoord, this, currentLevel.source);
            }
        }

        UpdateCellGridState(null);
    }

    public void UpdateLevelAfterRotation(Coord cellCoord) {
        currentLevel.CellAt(cellCoord).RotateRight();
        UpdateCellGridState(null);

        if (currentLevel.IsSolution()) {
            Reload();
        }
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
