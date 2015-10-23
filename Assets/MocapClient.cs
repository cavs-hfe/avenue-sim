using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class MocapClient : MonoBehaviour {

	private Vector3 headPosition = Vector3.zero;
	private Vector3 handLeftPos = Vector3.zero;
	private Vector3 handRightPos = Vector3.zero;
	private Vector3 elbowLeftPos = Vector3.zero;
	private Vector3 elbowRightPos = Vector3.zero;
		
	public string ipAddress = "192.168.1.2";
	
	public Camera oculusCamera;

	private AudioSource audioSource;
	public AudioClip startSound;
	
	public GameObject cameraRig;
	public GameObject head;
	public GameObject leftHand;
	public GameObject rightHand;
	public GameObject leftElbow;
	public GameObject rightElbow;

	// Use this for initialization
	void Start () {
		//connect to server

		//start getting data
	}

	// Update is called once per frame
	void FixedUpdate () {
	
		head.transform.localPosition = headPosition;
		cameraRig.transform.position = head.transform.position;
		
		leftHand.transform.localPosition = handLeftPos;
		rightHand.transform.localPosition = handRightPos;
		leftElbow.transform.localPosition = elbowLeftPos;
		rightElbow.transform.localPosition = elbowRightPos;
				
		if (Input.GetKeyDown (KeyCode.JoystickButton0)) {
			audioSource.PlayOneShot(startSound);
			UnityEngine.VR.InputTracking.Recenter();
		}

	}
}
