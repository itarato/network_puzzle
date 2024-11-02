using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellController : MonoBehaviour {
    private bool isMoving = false;
    private Coord coord;
    private GameController gameController;
    private bool isOn = true;
    private Level.Cell cell;

    public List<GameObject> paths;
    public GameObject serverGameObject;
    public GameObject clientGameObject;
    public Material onMaterial;
    public Material offMaterial;
    public float rotateTime = 0.2f;

    public void Initialize(Level.Cell cell, Coord coord, GameController gameController, Coord sourceCellCoord) {
        TurnOff();

        clientGameObject.SetActive(cell.isEnd && !sourceCellCoord.Equals(coord));
        serverGameObject.SetActive(sourceCellCoord.Equals(coord));

        this.coord = coord;
        this.gameController = gameController;
        this.cell = cell;

        for (int i = 0; i < paths.Count; i++) {
            paths[i].SetActive(cell.paths[i]);
        }
    }

    public void TurnOn() {
        if (isOn) return;
        isOn = true;

        foreach (GameObject path in paths) {
            path.GetComponent<Renderer>().material = onMaterial;
        }
        if (clientGameObject.activeSelf) {
            clientGameObject.GetComponent<Renderer>().material = onMaterial;
        }
    }

    public void TurnOff() {
        if (!isOn) return;
        isOn = false;

        foreach (GameObject path in paths) {
            path.GetComponent<Renderer>().material = offMaterial;
        }
        if (clientGameObject.activeSelf) {
            clientGameObject.GetComponent<Renderer>().material = offMaterial;
        }
    }

    private void OnMouseDown() {
        if (isMoving) return;

        isMoving = true;
        StartCoroutine(Rotate(rotateTime));
    }

    private IEnumerator Rotate(float duration) {
        gameController.UpdateLevelBeforeRotation(coord);

        Quaternion initialRotation = transform.rotation;
        Quaternion targetRotation = initialRotation * Quaternion.Euler(0f, 90f, 0f);
        float elapsedTime = 0f;

        while (elapsedTime < duration) {
            transform.rotation = Quaternion.Slerp(initialRotation, targetRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.rotation = targetRotation;
        isMoving = false;
        gameController.UpdateLevelAfterRotation(coord);
    }
}
