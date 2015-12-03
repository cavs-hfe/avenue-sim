using UnityEngine;
using System.Collections;

public class FloorBotMoverScript : MonoBehaviour {

	public float speed;
	public float longLeg = 5f;
	public float shortLeg = 2f;

	private float nextChange = 0f;
	private int state = 0;

	// Use this for initialization
	void Start () {
	
	}
	

	void FixedUpdate () {
	
		if (Time.time > nextChange) {
			state += 1;
			if(state == 4) state = 0;

			gameObject.transform.Rotate(new Vector3(0,90,0));

			if(state == 0 || state == 2) {
				nextChange = Time.time + longLeg;
			} else {
				nextChange = Time.time + shortLeg;
			}

		}

		gameObject.transform.Translate (Vector3.forward * speed * Time.deltaTime);

	}
}
