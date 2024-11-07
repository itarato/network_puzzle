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

        foreach (Light light in lights) {
            if (Random.Range(0, 100) < 1) {
                light.gameObject.SetActive(!light.gameObject.activeSelf);
            }
        }
    }
}
