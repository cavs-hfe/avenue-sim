﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

namespace CAVS.Recording {

	public class RecordingServiceBehavior : MonoBehaviour {


		/// <summary>
		/// Whether or not we should start recording the scene when the scene is initialized.
		/// </summary>
		[SerializeField]
		private bool recordOnPlay = true;


		/// <summary>
		/// The name of the file that we'll save all recorded movement frames to.
		/// </summary>
		[SerializeField]
		private string fileName;


		/// <summary>
		/// The rate at which we'd like to record our scene at.
		/// </summary>
		[SerializeField]
		private float framesPerSecondForRecording = 30f;


		/// <summary>
		/// The current state of the recording service, as to whether or not it's
		/// recording actors.
		/// </summary>
		private RecordingState currentState = RecordingState.Stopped;


		/// <summary>
		/// The actors in scene that we will be recording
		/// </summary>
		private List<ActorBehavior> actorsInScene;


		/// <summary>
		/// The recording object that is built while we're recording a scene.
		/// </summary>
		private Recording currentRecordingBeingBuilt;


		/// <summary>
		/// Unity's time of the last time we took a snap shot of all the actors
		/// in the scene for recording.
		/// </summary>
		private float timeOfLastFrameCapture = 0f;


		/// <summary>
		/// This position is used to represent in our recording that we don't know
		/// where a certain object is.
		/// Ex.  If we had an actor at the beggining of our recording but now it seems
		/// it's been deleted, then we will assign our frames it's position of 
		/// this variable's value
		/// </summary>
		private Vector3 nullObjectPosition = new Vector3(666,666,666);


		/// <summary>
		/// GameObject instance unique IDs for identifying an actor
		/// </summary>
		private int[] actorIdsBeingRecorded;


		/// <summary>
		/// Begins recording the scene if you pass in a name
		/// </summary>
		public void startRecording(string nameOfRecording){

			// Cleans data
			if (nameOfRecording != null && nameOfRecording == "") {
				Debug.Log ("You can't create a recording with no name!");
				return;
			}

			// Can't start a new recording while currentely recording
			if (currentState == RecordingState.Recording || currentState == RecordingState.Paused) {
				Debug.LogError ("Can't record");
			}

			// Change state to recording
			currentState = RecordingState.Recording;

			// Grab all actors in the scene
			actorsInScene = new List<ActorBehavior>(GameObject.FindObjectsOfType<ActorBehavior> ());

			// Grabs all actor unique instance ids
			actorIdsBeingRecorded = new int[actorsInScene.Count];
			for (int i = 0; i < actorIdsBeingRecorded.Length; i++) {
				actorIdsBeingRecorded[i] = actorsInScene [i].gameObject.GetInstanceID ();
			}

			// Grab actor names
			string[] actorNames = new string[actorsInScene.Count];
			for (int i = 0; i < actorsInScene.Count; i++) {
				actorNames[i] = actorsInScene [i].getNameForRecording ();
			}

			// Grab actors prefered playback representation
			string[] actorplaybackRep = new string[actorsInScene.Count];
			for (int i = 0; i < actorsInScene.Count; i++) {
				actorplaybackRep[i] = actorsInScene [i].getObjToRepresentActor ();
			}

			// Create our recording object that we'll add frame data to.
			currentRecordingBeingBuilt = new Recording (nameOfRecording, getFPS(), actorIdsBeingRecorded, actorNames, actorplaybackRep);

			// Capture our first frame
			captureFrame ();
		}


		/// <summary>
		/// Stops the recording and saves it with the file name originally specified.
		/// </summary>
		public void stopAndSaveRecording(){

			if (currentState != RecordingState.Paused && currentState != RecordingState.Recording) {
				Debug.LogError ("Can't stop a recording because we're not currentely making one!");
				return;
			}

			currentState = RecordingState.Stopped;
			saveRecording ();

		}


		/// <summary>
		/// Stops the and trashes the current recording being made.
		/// </summary>
		public void stopAndTrashRecording(){
			
			if (currentState != RecordingState.Paused && currentState != RecordingState.Recording) {
				Debug.LogError ("Can't stop a recording because we're not currentely making one!");
				return;
			}

			currentState = RecordingState.Stopped;
			currentRecordingBeingBuilt = null;

		}


		/// <summary>
		/// The FPS that the recording service takes snapshots of the state of
		/// the actors in the scene while recording.
		/// </summary>
		/// <returns>The FPS.</returns>
		public float getFPS(){
			return Mathf.Clamp (this.framesPerSecondForRecording, 1, 1000); 
		}


		/// <summary>
		/// Sets the FPS to record by if the service is not already recording
		/// </summary>
		/// <param name="fps">Fps.</param>
		public void setFPSToRecordBy(int fps){
			if (currentelyRecording ()) {
				Debug.LogWarning ("Can't change the fps to record by while recording! Ignoring!");
				return;
			}
			this.framesPerSecondForRecording = Mathf.Clamp (fps, 1, 1000); 
		}


		/// <summary>
		/// Whether or not we're currentely recording the scene.
		/// </summary>
		/// <returns><c>true</c>, if recording is currentely occuring, <c>false</c> otherwise.</returns>
		public bool currentelyRecording(){
			
			if (currentState == RecordingState.Recording) {
				return true;
			}

			return false;

		}


		/// <summary>
		/// Logs an event to recording
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="contents">Contents.</param>
		public void logEventToRecording(string name, string contents){

			if (!currentelyRecording ()) {
				Debug.LogError ("You can log an event when theres no recording going on!");
				return;
			}

			currentRecordingBeingBuilt.logEvent (name, contents);

		}


		// Use this for initialization
		void Start () {

			if (!recordOnPlay) {
				return;
			}

			startRecording (this.fileName);
		
		}


		// Update is called once per frame
		void Update () {
		

			// Don't bother doing anything if we're not recording
			if (currentState != RecordingState.Recording) {
				return;
			}


			// If it's time to capture another frame.
			if(Time.time - timeOfLastFrameCapture >= 1f / getFPS()){
				captureFrame ();
			}


//			if(Input.anyKey){
//				if(Input.inputString != ""){
//					currentRecordingBeingBuilt.logEvent ("Input", Input.inputString);
//				}
//			}

		}


		/// <summary>
		/// *Called by the Unity Engine*
		/// 
		/// Raises the disable event.
		/// Called when the object is disabled, such as when you exit play mode.
		/// Used for saving all frame data if we where recording at the time
		/// </summary>
		void OnDisable(){

			if (currentState == RecordingState.Recording || currentState == RecordingState.Paused) {
				stopAndSaveRecording ();
			}

		}


		/// <summary>
		/// Creates a frame containing nessesary information for playback if we're in a recording state
		/// </summary>
		private void captureFrame(){

			// If we're trying to capture a frame while we're not recording throw an error
			if (currentState != RecordingState.Recording) {
				Debug.LogError ("Your trying to capture a frame while your not recording!");
				return;
			}

			// Update the last time we've captured a frame
			timeOfLastFrameCapture = Time.time;

			// Declare all data we'll need for creating a frame
			Vector3[] postions = new Vector3[actorsInScene.Count];
			Vector3[] rotations = new Vector3[actorsInScene.Count];
			int[] ids = new int[actorsInScene.Count];

			// Go through all actors that where put in our roster at the start of the recording
			for (int i = 0; i < this.actorsInScene.Count; i++) {

				// Make sure they still exist in the scene before recording them
				if (actorsInScene [i] != null && actorsInScene [i].gameObject != null) {

					postions [i] = actorsInScene [i].gameObject.transform.position;
					rotations [i] = actorsInScene [i].gameObject.transform.rotation.eulerAngles;
					ids [i] = actorsInScene [i].gameObject.GetInstanceID ();

				} else {
				
					// setting null object position to indicate we don't know where the object is
					postions [i] = nullObjectPosition;
					ids [i] = actorIdsBeingRecorded [i];
				
				}

			}

			// Create our frame
			currentRecordingBeingBuilt.addFrame(new Frame(ids, postions, rotations, Time.time));

		}


		private void saveRecording(){

			if (currentRecordingBeingBuilt == null) {
				Debug.Log ("Trying to save a recording that doesn't exist!");
				return;
			}

			if(!Directory.Exists("Recordings")){
				Directory.CreateDirectory ("Recordings");
			}

			string path = "Recordings/" + currentRecordingBeingBuilt.Name+".xml";

			using(FileStream outputFile = File.Create(path)){

				XmlSerializer formatter = new XmlSerializer (typeof(Recording));
				formatter.Serialize (outputFile, currentRecordingBeingBuilt);

			}

		}

	}

}