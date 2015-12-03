using UnityEngine;
using System.Collections;

public class RobotToggleScript : MonoBehaviour {

	public GameObject robot1;
	public GameObject robot2;

	private int currentRobot = 1;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.JoystickButton2)) {
			if(currentRobot == 1) {
				robot2.SetActive(true);
				robot1.SetActive(false);
				currentRobot = 2;
			} else {
				robot2.SetActive(false);
				robot1.SetActive(true);
				currentRobot = 1;
			}
		}
	}
}
