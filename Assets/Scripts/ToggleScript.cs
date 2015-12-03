using UnityEngine;
using System.Collections;

public class ToggleScript : MonoBehaviour {

	public GameObject trigger;

	public GameObject eventObject;
	public float speed = 0.1f;

	private bool triggered = false;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if (triggered) {
			eventObject.transform.Translate(Vector3.right * speed * Time.deltaTime);
		}
	}

	void OnTriggerEnter(Collider other) {

		Debug.Log ("collider triggered!");

		if (other.gameObject.Equals (trigger)) {
			Debug.Log("collider matched trigger!");
			triggered = true;
		}
	}
}
