using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellController : MonoBehaviour {
    private bool isMoving = false;
    private Coord coord;
    private GameController gameController;
    private bool isOn = true;
    private Level.Cell cell;
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

    public AudioSource rotateAudioSource;

    public void Initialize(Level.Cell cell, Coord coord, GameController gameController, Coord sourceCellCoord) {
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

        if (cell.isEnd || ((cell.paths[0] && cell.paths[2]) && !(cell.paths[1] || cell.paths[3])) || ((cell.paths[1] && cell.paths[3]) && !(cell.paths[0] || cell.paths[2]))) {
            for (int i = 0; i < paths.Count; i++) {
                joints[i].SetActive(false);
            }
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

    private void OnMouseDown() {
        if (isMoving) return;
        if (isGameOver) return;

        isMoving = true;
        rotateAudioSource.Play();
        StartCoroutine(Rotate(rotateTime));
    }

    private IEnumerator Rotate(float duration) {
        gameController.UpdateLevelBeforeRotation(coord);

        Quaternion initialRotation = pathsGameObjects.transform.rotation;
        Quaternion targetRotation = initialRotation * Quaternion.Euler(0f, 90f, 0f);
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
