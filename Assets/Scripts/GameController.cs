using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
    public GameObject cellTemplate;
    public GameObject hexCellTemplate;
    public GameObject menuUI;
    public Camera mainCamera;
    public ParticleSystem victoryParticleSystem;
    public AudioSource victoryAudioSource;
    public AudioSource newLevelAudioSource;

    private ILevel currentLevel;
    private List<GameObject> cellInstances = new List<GameObject>();

    private Coord lastUsedBounds = new Coord(3, 3);
    private bool lastIsAcrossBorders = false;
    private CellType lastCellType = CellType.Square;

    private const float hexCellWidth = 0.86602f;

    void Start() {
        victoryParticleSystem.Stop();
    }

    public void Reload(Coord bounds, bool isAcrossBorders, CellType cellType) {
        lastUsedBounds = bounds;
        lastIsAcrossBorders = isAcrossBorders;
        lastCellType = cellType;

        if (cellType == CellType.Square) {
            currentLevel = LevelGenerator.Generate(bounds.x, bounds.y, isAcrossBorders);
        } else if (cellType == CellType.Hex) {
            currentLevel = HexLevelGenerator.Generate(bounds.x, bounds.y, isAcrossBorders);
        }

        CleanupLevel();

        for (int z = 0; z < currentLevel.height; z++) {
            for (int x = 0; x < currentLevel.width; x++) {
                Vector3 pos;

                if (cellType == CellType.Square) {
                    pos = new Vector3(
                        transform.position.x - (currentLevel.width / 2f) + x + 0.5f,
                        transform.position.y,
                        transform.position.z + (currentLevel.height / 2f) - z - 0.5f
                    );
                } else {
                    pos = new Vector3(
                        transform.position.x - (currentLevel.width * hexCellWidth / 2f) + x * hexCellWidth - (hexCellWidth / 2f) * (z % 2) + (hexCellWidth * 0.75f),
                        transform.position.y,
                        transform.position.z + (currentLevel.height * 0.75f / 2f) - (0.75f * z) - 0.375f
                    );
                }

                GameObject cellInstance;
                if (cellType == CellType.Square) {
                    cellInstance = Instantiate(cellTemplate, pos, Quaternion.identity);
                } else {
                    cellInstance = Instantiate(hexCellTemplate, pos, Quaternion.identity);
                }
                cellInstances.Add(cellInstance);

                ICellController cellController = cellInstance.GetComponent<ICellController>();
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
                ICellController cellController = cellInstance.GetComponent<ICellController>();
                cellController.GameOver();
            }

            victoryParticleSystem.Play();
            Invoke(nameof(FinishGameAndOpenMenu), 3f);
        }
    }

    private void FinishGameAndOpenMenu() {
        CleanupLevel();
        menuUI.SetActive(true);
    }

    public void UpdateLevelBeforeRotation(Coord skipCoord) {
        UpdateCellGridState(skipCoord);
    }

    private void UpdateCellGridState(Coord skipCoord) {
        foreach (var cellInstance in cellInstances) {
            ICellController cellController = cellInstance.GetComponent<ICellController>();
            cellController.TurnOff();
        }

        List<Coord> activeCells = currentLevel.ActiveCells(skipCoord);
        foreach (var activeCellCoord in activeCells) {
            GameObject cellInstance = cellInstances[activeCellCoord.y * currentLevel.width + activeCellCoord.x];
            ICellController cellController = cellInstance.GetComponent<ICellController>();
            cellController.TurnOn();
        }
    }

    public void OnClickExitGame() {
        CancelInvoke();
        CleanupLevel();

        menuUI.SetActive(true);
    }

    public void OnClickRegenerateLevel() {
        CancelInvoke();
        Reload(lastUsedBounds, lastIsAcrossBorders, lastCellType);
    }

    private void CleanupLevel() {
        foreach (var cellInstance in cellInstances) {
            Destroy(cellInstance.gameObject);
        }
        cellInstances.Clear();
        victoryParticleSystem.Stop();
    }
}
