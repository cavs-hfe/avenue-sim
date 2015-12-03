using UnityEngine;
using System.Collections;

public class GunShowHideScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.JoystickButton2)) {
			GameObject guns = this.transform.FindChild ("Guns").gameObject;
			guns.SetActive(!guns.activeSelf);
		}
	}
}
