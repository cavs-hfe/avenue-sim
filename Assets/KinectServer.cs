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
	public string ipAddress = "192.168.1.2";
	private Vector3 oculusToKinectVector;

	// Use this for initialization
	void Start () {

		headPosition = gameObject.transform.localPosition;

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
		double[] position = new double[3];
		IFormatter formatter = new BinaryFormatter ();

		Byte[] buffer = new Byte[5];
		netStream.Read (buffer, 0, 5);

		while (true) {
			position = (double[])formatter.Deserialize(netStream);
			headPosition = new Vector3((float)-position[0], (float)position[1], (float)position[2]);
		}
	}

	void FixedUpdate () {
		gameObject.transform.localPosition = headPosition;
		//Debug.Log("set head position: " + headPosition.ToString());
	}
}

