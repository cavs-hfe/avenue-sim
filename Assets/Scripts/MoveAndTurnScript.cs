using UnityEngine;
using System.Collections;

public class MoveAndTurnScript : MonoBehaviour {

	public float leg1Time = 5f;
	public float leg2Time = 5f;
	public float speed;
	public Transform startPosition;

	private float startTime;
	private int legNum = 1;	

	// Use this for initialization
	void Start () {
		startTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
	
		if (Time.time > (startTime + leg1Time) && legNum == 1) {
			gameObject.transform.Rotate(new Vector3(0,0,270));
			legNum = 2;
		} else if(Time.time > (startTime + leg1Time + leg2Time) && legNum == 2) {
			gameObject.transform.position = startPosition.position;
			gameObject.transform.Rotate(new Vector3(270, 90, 0));
			startTime = Time.time;
			legNum = 1;
		}

		gameObject.transform.Translate (Vector3.right * speed * Time.deltaTime);

	}
}
