using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
    public GameObject cellTemplate;
    public GameObject menuUI;
    public Camera mainCamera;
    public ParticleSystem victoryParticleSystem;

    private Level currentLevel;
    private List<GameObject> cellInstances = new List<GameObject>();

    void Start() {
        victoryParticleSystem.Stop();
    }

    public void Reload(Coord bounds) {
        currentLevel = LevelGenerator.Generate(bounds.x, bounds.y);

        foreach (var cellInstance in cellInstances) {
            Destroy(cellInstance.gameObject);
        }
        cellInstances.Clear();

        for (int z = 0; z < currentLevel.height; z++) {
            for (int x = 0; x < currentLevel.width; x++) {
                Vector3 pos = new Vector3(
                    transform.position.x - (currentLevel.width / 2f) + x + 0.5f,
                    transform.position.y,
                    transform.position.z + (currentLevel.height / 2f) - z - 0.5f
                );

                GameObject cellInstance = Instantiate(cellTemplate, pos, Quaternion.identity);
                cellInstances.Add(cellInstance);

                CellController cellController = cellInstance.GetComponent<CellController>();
                Coord cellCoord = new Coord(x, z);
                cellController.Initialize(currentLevel.CellAt(cellCoord), cellCoord, this, currentLevel.source);
            }
        }

        UpdateCellGridState(null);
        UpdateCameraForView(bounds);
    }

    private void UpdateCameraForView(Coord coord) {
        // 6.upto(32).map { [_1, (1 + (_1 / 26.0) * 26).round] }.to_h 
        float minCameraYFromHeight = 3.5f + ((coord.x - 3f) / 13f) * 15.5f;
        mainCamera.orthographicSize = minCameraYFromHeight;
    }

    public void UpdateLevelAfterRotation(Coord cellCoord) {
        currentLevel.CellAt(cellCoord).RotateRight();
        UpdateCellGridState(null);

        if (currentLevel.IsSolution()) {
            foreach (var cellInstance in cellInstances) {
                CellController cellController = cellInstance.GetComponent<CellController>();
                cellController.GameOver();
            }

            victoryParticleSystem.Play();
            Invoke(nameof(FinishGameAndOpenMenu), 3f);
        }
    }

    private void FinishGameAndOpenMenu() {
        victoryParticleSystem.Stop();
        menuUI.SetActive(true);
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

    public void OnShuffleLevel() {
        // TODO
    }

    public void OnClickExitGame() {
        victoryParticleSystem.Stop();
        CancelInvoke();
        menuUI.SetActive(true);
    }
}
