using UnityEngine;
using System.Collections;

public class GamepadBaseMover : MonoBehaviour {

	public float scaling = 1.0f;
	public Camera referenceCamera;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		float h = Input.GetAxis ("Horizontal");
		float v = Input.GetAxis ("Vertical");

		Vector3 projectedForward = Vector3.ProjectOnPlane (referenceCamera.transform.forward, Vector3.up);
		Vector3 projectedRight = Vector3.ProjectOnPlane (referenceCamera.transform.right, Vector3.up);

		gameObject.transform.Translate (projectedForward * (h * scaling));
		gameObject.transform.Translate (projectedRight * (-v * scaling));

	}
}
