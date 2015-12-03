using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System;
using System.Threading;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class KinectServer : MonoBehaviour
{
	
	private Vector3 headPosition = Vector3.zero;
	private Vector3 handLeftPos = Vector3.zero;
	private Vector3 handRightPos = Vector3.zero;
	private Vector3 elbowLeftPos = Vector3.zero;
	private Vector3 elbowRightPos = Vector3.zero;
	private Vector3 kneeLeftPos = Vector3.zero;
	private Vector3 kneeRightPos = Vector3.zero;
	private Vector3 footLeftPos = Vector3.zero;
	private Vector3 footRightPos = Vector3.zero;

	public GameObject placeholder;
	public GameObject leftHand;
	public GameObject rightHand;
	public GameObject leftElbow;
	public GameObject rightElbow;
	public GameObject leftKnee;
	public GameObject rightKnee;
	public GameObject leftFoot;
	public GameObject rightFoot;

	public string ipAddress = "192.168.1.2";
	public string mocapIpAddress = "192.168.1.4";

	public GameObject cameraRig;
	public Camera oculusCamera;

	private AudioSource audioSource;
	public AudioClip startSound;
	public AudioClip recordSound;

	private bool recording = false;
	private StreamWriter recordWriter;

	private GameObject kinect;
	private GameObject kinectFOV;
	private GameObject mocapFOV;
	private bool enableKinect = false;
	private bool enableMocap = false;

	public bool usingMocap = true;

	private float mocapHeightAdjust = -1.3f;

	private System.Diagnostics.Process mocapProcess = null;

	// Use this for initialization
	void Start ()
	{
		headPosition = placeholder.transform.position;
		handLeftPos = leftHand.transform.position;
		handRightPos = rightHand.transform.position;

		kinect = this.transform.FindChild ("Kinect").gameObject;
		kinectFOV = this.transform.FindChild ("KinectFOV").gameObject;
		mocapFOV = this.transform.FindChild ("MocapFOV").gameObject;
		
		audioSource = this.gameObject.AddComponent<AudioSource> ();
		audioSource.spatialBlend = 0.0f; // Use 2D sound for sound effects
		audioSource.Play ();

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

		if (usingMocap) {
			System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
			startInfo.Arguments = mocapIpAddress;
			startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Minimized;
			startInfo.FileName = "C:\\rsween\\dev\\motionStream\\Release\\EVaRTSDKExample.exe";

			mocapProcess = System.Diagnostics.Process.Start(startInfo);
		}
	}
	
	private void AcceptCallback (IAsyncResult ar)
	{
		Debug.Log ("accepted");
		Socket listener = (Socket)ar.AsyncState;
		Socket handler = listener.EndAccept (ar);
		
		Thread t = new Thread (new ParameterizedThreadStart (HandleClient));
		t.Start (handler);
		
		listener.BeginAccept (new AsyncCallback (AcceptCallback), listener);
	}
	
	private void HandleClient (object obj)
	{
		Debug.Log ("handling");
		Socket socket = (Socket)obj;
		NetworkStream netStream = new NetworkStream (socket);
		Dictionary<string, double[]> positions = new Dictionary<string, double[]> ();
		IFormatter formatter = new BinaryFormatter ();
		
		Byte[] buffer = new Byte[6];
		netStream.Read (buffer, 0, 6);

		String message = System.Text.Encoding.ASCII.GetString (buffer);
		Debug.Log ("Received message from client: " + message);

		if (message.Equals ("mocaps")) {

			enableMocap = true;
			enableKinect = false;

			using (StreamReader reader = new StreamReader(netStream)) {
				while ((message = reader.ReadLine()) != null) {
					//Debug.Log ("Received message from client: " + message);
					string[] parts = message.Split (new char[]{','});
					if(!parts[1].Equals("1e+007")) { //1e+007
						headPosition = new Vector3 (-float.Parse (parts [1]), float.Parse (parts [2]), float.Parse (parts [3]));
						headPosition = headPosition / 1000;
						headPosition = headPosition + new Vector3 (0, mocapHeightAdjust, 0);
					}
				}
			}
		} else if (message.Equals ("kinect")) {

			enableKinect = true;
			enableMocap = false;

			while (true) {
				positions = (Dictionary<string, double[]>)formatter.Deserialize (netStream);
				if (positions.ContainsKey ("head")) {
					headPosition = new Vector3 ((float)-positions ["head"] [0], (float)positions ["head"] [1], (float)positions ["head"] [2]);
					handLeftPos = new Vector3 ((float)-positions ["handLeft"] [0], (float)positions ["handLeft"] [1], (float)positions ["handLeft"] [2]);
					handRightPos = new Vector3 ((float)-positions ["handRight"] [0], (float)positions ["handRight"] [1], (float)positions ["handRight"] [2]);
					elbowLeftPos = new Vector3 ((float)-positions ["elbowLeft"] [0], (float)positions ["elbowLeft"] [1], (float)positions ["elbowLeft"] [2]);
					elbowRightPos = new Vector3 ((float)-positions ["elbowRight"] [0], (float)positions ["elbowRight"] [1], (float)positions ["elbowRight"] [2]);
					kneeLeftPos = new Vector3 ((float)-positions ["kneeLeft"] [0], (float)positions ["kneeLeft"] [1], (float)positions ["kneeLeft"] [2]);
					kneeRightPos = new Vector3 ((float)-positions ["kneeRight"] [0], (float)positions ["kneeRight"] [1], (float)positions ["kneeRight"] [2]);
					footLeftPos = new Vector3 ((float)-positions ["footLeft"] [0], (float)positions ["footLeft"] [1], (float)positions ["footLeft"] [2]);
					footRightPos = new Vector3 ((float)-positions ["footRight"] [0], (float)positions ["footRight"] [1], (float)positions ["footRight"] [2]);
				}
			}
		}
	}
	
	void FixedUpdate ()
	{

		placeholder.transform.localPosition = headPosition;
		cameraRig.transform.position = placeholder.transform.position;

		leftHand.transform.localPosition = handLeftPos;
		rightHand.transform.localPosition = handRightPos;
		/*leftElbow.transform.localPosition = elbowLeftPos;
		rightElbow.transform.localPosition = elbowRightPos;
		leftKnee.transform.localPosition = kneeLeftPos;
		rightKnee.transform.localPosition = kneeRightPos;
		leftFoot.transform.localPosition = footLeftPos;
		rightFoot.transform.localPosition = footRightPos;*/

		if (kinectFOV.activeSelf != enableKinect) {
			kinect.SetActive (enableKinect);
			kinectFOV.SetActive (enableKinect);
		}

		if (mocapFOV.activeSelf != enableMocap) {
			mocapFOV.SetActive (enableMocap);
		}

		if (recording) {
			recordWriter.WriteLine (Time.time + "," 
				+ oculusCamera.transform.position.x + "," 
				+ oculusCamera.transform.position.y + "," 
				+ oculusCamera.transform.position.z + ","
				+ oculusCamera.transform.rotation.x + ","
				+ oculusCamera.transform.rotation.y + ","
				+ oculusCamera.transform.rotation.z);
		}

		if (Input.GetKeyDown (KeyCode.JoystickButton0)) {
			audioSource.PlayOneShot (startSound);
			UnityEngine.VR.InputTracking.Recenter ();
		}

		if (Input.GetKeyDown (KeyCode.JoystickButton1)) {
			if (recording) {
				audioSource.PlayOneShot (recordSound);
				recording = false;
				recordWriter.Flush ();
				recordWriter.Dispose ();
				recordWriter = null;
			} else {
				audioSource.PlayOneShot (recordSound);
				recording = true;
				recordWriter = new StreamWriter ("ped-sim-output" + Time.time + ".csv");
				recordWriter.WriteLine ("time, head_pos_x, head_pos_y, head_pos_z, head_rot_x, head_rot_y, head_rot_z");
			}
		}
	}

	void OnApplicationQuit() {
		if (mocapProcess != null) {
			mocapProcess.CloseMainWindow();
			mocapProcess.Dispose();
		}

		if (recordWriter != null) {
			recording = false;
			recordWriter.Flush();
			recordWriter.Dispose();
		}
	}


}

