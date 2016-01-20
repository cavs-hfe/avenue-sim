using UnityEngine;
using System.Collections;

public class ObserverCameraScript : MonoBehaviour {

    public float orbitScaling = 1.0f;
    public float zoomScaling = 1.0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        float o = Input.GetAxis("Orbit");
        float z = Input.GetAxis("Zoom");

        gameObject.transform.Rotate(Vector3.up, o * orbitScaling);
        gameObject.transform.Translate(Vector3.forward * (z * zoomScaling));
	}
}
