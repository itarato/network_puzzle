using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
    public GameObject cellTemplate;
    public GameObject menuUI;
    public Camera mainCamera;
    public ParticleSystem victoryParticleSystem;
    public AudioSource victoryAudioSource;
    public AudioSource newLevelAudioSource;

    private Level currentLevel;
    private List<GameObject> cellInstances = new List<GameObject>();

    private Coord lastUsedBounds = new Coord(3, 3);

    void Start() {
        victoryParticleSystem.Stop();
    }

    public void Reload(Coord bounds) {
        victoryParticleSystem.Stop();
        lastUsedBounds = bounds;
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

        newLevelAudioSource.Play();
    }

    private void UpdateCameraForView(Coord coord) {
        float minCameraYFromHeight = 3.25f + ((coord.x - 3f) / 7f) * 7.25f;
        mainCamera.orthographicSize = minCameraYFromHeight;
        
        victoryParticleSystem.gameObject.transform.position = new Vector3(
            victoryParticleSystem.gameObject.transform.position.x,
            victoryParticleSystem.gameObject.transform.position.y,
            minCameraYFromHeight
        );

        float particleScale = minCameraYFromHeight / 3.25f;
        victoryParticleSystem.gameObject.transform.localScale = new Vector3(particleScale, particleScale, particleScale);
    }

    public void UpdateLevelAfterRotation(Coord cellCoord) {
        currentLevel.CellAt(cellCoord).RotateRight();
        UpdateCellGridState(null);

        if (currentLevel.IsSolution()) {
            victoryAudioSource.Play();

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

    public void OnClickExitGame() {
        victoryParticleSystem.Stop();
        CancelInvoke();
        menuUI.SetActive(true);
    }

    public void OnClickRegenerateLevel() {
        CancelInvoke();
        Reload(lastUsedBounds);
    }
}
