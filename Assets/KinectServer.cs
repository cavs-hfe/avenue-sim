using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System;
using System.Threading;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public class KinectServer : MonoBehaviour {
	
	private Vector3 headPosition = Vector3.zero;
	private Vector3 handLeftPos = Vector3.zero;
	private Vector3 handRightPos = Vector3.zero;
	private Vector3 elbowLeftPos = Vector3.zero;
	private Vector3 elbowRightPos = Vector3.zero;
	private Vector3 kneeLeftPos = Vector3.zero;
	private Vector3 kneeRightPos = Vector3.zero;
	private Vector3 footLeftPos = Vector3.zero;
	private Vector3 footRightPos = Vector3.zero;

	public string ipAddress = "192.168.1.2";

	public Camera oculusCamera;
	private Vector3 oculusToKinectVector;
	private Vector3 oculusToHeadVector;
	private bool updatedRotation = false;
	private bool updateOculusHeadVector = false;

	private AudioSource audioSource;
	public AudioClip startSound;

	public GameObject cameraRig;
	public GameObject placeholder;
	public GameObject leftHand;
	public GameObject rightHand;
	public GameObject leftElbow;
	public GameObject rightElbow;
	public GameObject leftKnee;
	public GameObject rightKnee;
	public GameObject leftFoot;
	public GameObject rightFoot;

	public float headTweakX;
	public float headTweakZ;

	
	// Use this for initialization
	void Start () {
		
		headPosition = placeholder.transform.position;
		handLeftPos = leftHand.transform.position;
		handRightPos = rightHand.transform.position;
		
		oculusToKinectVector = gameObject.transform.forward;
		oculusToHeadVector = Vector3.zero;
		
		audioSource = this.gameObject.AddComponent<AudioSource>();
		audioSource.spatialBlend = 0.0f; // Use 2D sound for sound effects
		audioSource.Play();


		//setup socket
		IPAddress address = IPAddress.Parse (ipAddress);
		int port = 8888;
		Socket listeningSocket = new Socket (address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
		IPEndPoint localEndPoint = new IPEndPoint (address, port);
		
		//bind
		//Debug.Log ("binding");
		listeningSocket.Bind (localEndPoint);
		
		//listen
		//Debug.Log ("listening");
		listeningSocket.Listen (5);
		
		//accept
		//Debug.Log ("accepting");
		listeningSocket.BeginAccept (new AsyncCallback (AcceptCallback), listeningSocket);
	}
	
	private void AcceptCallback(IAsyncResult ar) {
		Debug.Log ("accepted");
		Socket listener = (Socket)ar.AsyncState;
		Socket handler = listener.EndAccept (ar);
		
		Thread t = new Thread (new ParameterizedThreadStart (HandleClient));
		t.Start (handler);
		
		listener.BeginAccept (new AsyncCallback (AcceptCallback), listener);
	}
	
	private void HandleClient(object obj) {
		Debug.Log ("handling");
		Socket socket = (Socket)obj;
		NetworkStream netStream = new NetworkStream (socket);
		Dictionary<string, double[]> positions = new Dictionary<string, double[]> ();
		IFormatter formatter = new BinaryFormatter ();
		
		Byte[] buffer = new Byte[5];
		netStream.Read (buffer, 0, 5);
		
		while (true) {
			positions = (Dictionary<string, double[]>)formatter.Deserialize(netStream);
			if(positions.ContainsKey("head")) {
				headPosition = new Vector3((float)-positions["head"][0], (float)positions["head"][1], (float)positions["head"][2]);
				handLeftPos = new Vector3((float)-positions["handLeft"][0], (float)positions["handLeft"][1], (float)positions["handLeft"][2]);
				handRightPos = new Vector3((float)-positions["handRight"][0], (float)positions["handRight"][1], (float)positions["handRight"][2]);
				elbowLeftPos = new Vector3((float)-positions["elbowLeft"][0], (float)positions["elbowLeft"][1], (float)positions["elbowLeft"][2]);
				elbowRightPos = new Vector3((float)-positions["elbowRight"][0], (float)positions["elbowRight"][1], (float)positions["elbowRight"][2]);
				kneeLeftPos = new Vector3((float)-positions["kneeLeft"][0], (float)positions["kneeLeft"][1], (float)positions["kneeLeft"][2]);
				kneeRightPos = new Vector3((float)-positions["kneeRight"][0], (float)positions["kneeRight"][1], (float)positions["kneeRight"][2]);
				footLeftPos = new Vector3((float)-positions["footLeft"][0], (float)positions["footLeft"][1], (float)positions["footLeft"][2]);
				footRightPos = new Vector3((float)-positions["footRight"][0], (float)positions["footRight"][1], (float)positions["footRight"][2]);

			}
		}
	}
	
	void FixedUpdate () {

		//gameObject.transform.localPosition = headPosition;
		placeholder.transform.localPosition = headPosition;
		cameraRig.transform.position = placeholder.transform.position - oculusToHeadVector;

		leftHand.transform.localPosition = handLeftPos;
		rightHand.transform.localPosition = handRightPos;
		leftElbow.transform.localPosition = elbowLeftPos;
		rightElbow.transform.localPosition = elbowRightPos;
		leftKnee.transform.localPosition = kneeLeftPos;
		rightKnee.transform.localPosition = kneeRightPos;
		leftFoot.transform.localPosition = footLeftPos;
		rightFoot.transform.localPosition = footRightPos;

		//Debug.Log("set head position: " + headPosition.ToString());
		
		if (Input.GetKeyDown (KeyCode.JoystickButton0)) {
			audioSource.PlayOneShot(startSound);
			CalibrateHeadset();
		}

		if (updateOculusHeadVector) {
			oculusToHeadVector =  oculusCamera.transform.position - placeholder.transform.position;
			Debug.Log ("OculusHeadVector @" + Time.time + ": " + oculusToHeadVector.ToString ());
			updateOculusHeadVector = false;
		}
		
		if (updatedRotation) {
			gameObject.transform.rotation = Quaternion.LookRotation(oculusToKinectVector);
			updatedRotation = false;
			updateOculusHeadVector = true;
		}
	}
	
	void CalibrateHeadset() {
		oculusToKinectVector = Vector3.ProjectOnPlane(-oculusCamera.transform.forward, Vector3.up);
		updatedRotation = true;
		Debug.Log ("OculusKinectVector @" + Time.time + ": " + oculusToKinectVector.ToString ());
	}

}

