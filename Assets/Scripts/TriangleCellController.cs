using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriangleCellController : MonoBehaviour, ICellController {
    private bool isMoving = false;
    private Coord coord;
    private GameController gameController;
    private bool isOn = true;
    private ICell cell;
    private bool isGameOver = false;
    private bool isClient = false;

    public List<GameObject> paths;
    public List<GameObject> joints;
    public GameObject serverGameObject;
    public GameObject clientGameObject;
    public Light clientLight;
    public GameObject pathsGameObjects;
    public Material onMaterial;
    public Material offMaterial;
    public float rotateTime = 0.14f;
    public GameObject cellBaseGameObject;
    public Material emptyCellMaterial;

    public AudioSource rotateAudioSource;

    public void Initialize(ICell cell, Coord coord, GameController gameController, Coord sourceCellCoord) {
        isClient = cell.isEnd && !sourceCellCoord.Equals(coord);

        clientGameObject.SetActive(isClient);
        clientLight.gameObject.SetActive(false);
        serverGameObject.SetActive(sourceCellCoord.Equals(coord));

        this.coord = coord;
        this.gameController = gameController;
        this.cell = cell;

        for (int i = 0; i < paths.Count; i++) {
            paths[i].SetActive(cell.paths[i]);
        }

        if (cell.isEnd || cell.isStraight || cell.isEmpty) {
            for (int i = 0; i < joints.Count; i++) {
                joints[i].SetActive(false);
            }
        }

        if (cell.isEmpty) {
            cellBaseGameObject.GetComponent<Renderer>().material = emptyCellMaterial;
        }

        TurnOff();
    }

    public void TurnOn() {
        if (isOn) return;
        isOn = true;

        foreach (GameObject path in paths) {
            path.GetComponent<Renderer>().material = onMaterial;
        }
        if (isClient) {
            clientLight.gameObject.SetActive(true);
        }
    }

    public void TurnOff() {
        if (!isOn) return;
        isOn = false;

        foreach (GameObject path in paths) {
            path.GetComponent<Renderer>().material = offMaterial;
        }
        if (isClient) {
            clientLight.gameObject.SetActive(false);
        }
    }

    public void GameOver() {
        isGameOver = true;
    }

    public void OnMouseDown() {
        if (isMoving) return;
        if (isGameOver) return;
        if (cell.isEmpty) return;

        isMoving = true;
        rotateAudioSource.Play();
        StartCoroutine(Rotate(rotateTime));
    }

    private IEnumerator Rotate(float duration) {
        gameController.UpdateLevelBeforeRotation(coord);

        Quaternion initialRotation = pathsGameObjects.transform.rotation;
        Quaternion targetRotation = initialRotation * Quaternion.Euler(0f, 120f, 0f);
        float elapsedTime = 0f;

        while (elapsedTime < duration) {
            pathsGameObjects.transform.rotation = Quaternion.Slerp(initialRotation, targetRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        pathsGameObjects.transform.rotation = targetRotation;
        pathsGameObjects.transform.position = new Vector3(
            pathsGameObjects.transform.position.x,
            0f,
            pathsGameObjects.transform.position.z
        );
        isMoving = false;
        gameController.UpdateLevelAfterRotation(coord);
    }
}
