using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class ServerController : MonoBehaviour {
    public List<Light> lights;

    // Start is called before the first frame update
    void Start() {
        foreach (Light light in lights) {
            light.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update() {
        if (!gameObject.activeSelf) return;

        int randIndex = Random.Range(0, 50);
        if (randIndex < lights.Count) {
            foreach (Light light in lights) {
                light.gameObject.SetActive(false);
            }
            lights[randIndex].gameObject.SetActive(true);
        }
    }
}
