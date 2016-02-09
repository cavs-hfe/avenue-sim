using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

namespace CAVS.Recording {


	/// <summary>
	/// This is used for playing a single recording at a time inside the scene.
	/// </summary>
	public class PlaybackServiceBehavior : MonoBehaviour {

		/// <summary>
		/// The different states the playback behavior can be in
		/// </summary>
		enum PlaybackState {
			Stopped,
			Paused,
			Playing
		}


		/// <summary>
		/// The currrent loaded recording that we may or maynot be playing
		/// </summary>
		private Recording currentLoadedRecording = null;


		/// <summary>
		/// The current state we're in that determines whether or not we're animating actors
		/// </summary>
		private PlaybackState currentPlaybackState = PlaybackState.Stopped;


		/// <summary>
		/// The time in the game that playback of a recording started.
		/// </summary>
		private float timePlaybackStarted = 0;


		/// <summary>
		/// The time the current frame we are on started
		/// </summary>
		private float timeFrameStarted = 0;


		/// <summary>
		/// The index that references the frame in the currentely loaded recording
		/// that we are currentely animating.
		/// </summary>
		private int frameCurrentelyOn = 0;


		/// <summary>
		/// How long we have spent paused during our playback
		/// </summary>
		private float timeSpentPaused = 0;


		/// <summary>
		/// The actors currentely being animated from the recording
		/// </summary>
		private Dictionary<int, GameObject> actors;


		/// <summary>
		/// If true, the when doing playback, we'll attempt to begin controlling
		/// objects that where used to create the recording in the first place, 
		/// instead of instantiating objects to represent them.
		/// 
		/// If you don't want the objects behavior to fire, have them implement the
		/// IPlaybackInterference Interface.
		/// </summary>
		private bool replayWithActorsAlreadySpawned;


		/// <summary>
		/// If you use the playback with actors already spawned, this list will contain all those
		/// that where in the scene before the playback was started.
		/// 
		/// This is so when playback stops, we know what not to delete
		/// </summary>
		private List<GameObject> actorsAlreadyInSceneWhenStartPlaying;


		/// <summary>
		/// Gets all recordings file names found in our designated recordings folder
		/// </summary>
		/// <returns>All Files found in recordings folder.</returns>
		public static string[] getAllRecordingsFileNames(){

			string[] recordings = Directory.GetFiles ("Recordings");

			// Clean the name
			for (int i = 0; i < recordings.Length; i++) {
				recordings[i] = Path.GetFileName (recordings[i]);
			}

			return recordings;

		}


		/// <summary>
		/// Sets whether or not when we're playing back, if we'll try using actors that where
		/// used to make the recording or just use the representations they chose.
		/// </summary>
		/// <param name="value">If set to <c>true</c> we'll try finding the original actos to the recording.</param>
		public void setPlaybackWithOriginalActors(bool value){

			if (currentPlaybackState == PlaybackState.Paused || currentPlaybackState == PlaybackState.Playing) {
				Debug.LogWarning ("The playback has already begun!  Setting this value won't do anything to" +
					"the current recording being played.  Try stopping (not pause) the playback and then assign this value!");
			}

			this.replayWithActorsAlreadySpawned = value;
		}


		/// <summary>
		/// Loads the recording located in the Recordings folder.
		/// Will return true if the loading was succesful
		/// </summary>
		/// <returns><c>true</c>, if loading was succesful <c>false</c> otherwise.</returns>
		/// <param name="recordingname">Recordingname.</param>
		public bool setRecordingToPlayback(string recordingname){

			// Clear out any recording that might be loaded
			clearCurrentLoadedRecording();

			currentLoadedRecording = getRecording (recordingname);

			if(currentLoadedRecording == null){
				return false;
			}

			return true;

		}


		/// <summary>
		/// Clears out whatever recording is playing and sets the new one
		/// </summary>
		public void setRecordingToPlayback(Recording recording){

			if (recording == null) {
				return;
			}

			clearCurrentLoadedRecording ();

			currentLoadedRecording = recording;

		}


		/// <summary>
		/// Loads the recording from the Recordings folder
		/// </summary>
		/// <returns>The recording.</returns>
		/// <param name="recordingPath">Recording path.</param>
		public static Recording getRecording(string recordingPath){
			
			// Load in the new recording
			XmlSerializer serializer = new XmlSerializer (typeof(Recording));

			try{
				using (FileStream fileStream = new FileStream("Recordings/"+recordingPath,FileMode.Open)) 
				{
					Recording result = (Recording) serializer.Deserialize(fileStream);
					return result;
				}
			}
			catch {
				Debug.LogError ("Error loading recording! Returning null!");
				return null;
			}

		}

		/// <summary>
		/// If a recording has been loaded, then this will spawn all actors in the scene and begin
		/// animating them.
		/// </summary>
		public void playLoadedRecording(){

			if (currentLoadedRecording == null) {
				Debug.LogError ("Trying to play a recording when one isn't loaded yet!");
				return;
			}


			if (currentPlaybackState == PlaybackState.Paused) {
				currentPlaybackState = PlaybackState.Playing;
				return;
			}

			clearCurrentLoadedRecording ();

			// Reset previous recording variables
			actors = new Dictionary<int, GameObject> ();
			currentPlaybackState = PlaybackState.Playing;
			frameCurrentelyOn = 0;
			timePlaybackStarted = Time.time;
			timeFrameStarted = Time.time;
			timeSpentPaused = 0f;
			velocitys = new Vector3[currentLoadedRecording.ActorIds.Length];

			ActorBehavior[] actorsInScene = null;
			if(replayWithActorsAlreadySpawned){
				actorsInScene = GameObject.FindObjectsOfType<ActorBehavior> ();
			}

			actorsAlreadyInSceneWhenStartPlaying = new List<GameObject> ();

			// Create actors
			for (int i = 0; i < currentLoadedRecording.ActorIds.Length; i++) {

				// Get the unique Id of the actor
				int id = currentLoadedRecording.ActorIds [i];

				// Create the actor
				GameObject actorRef = (GameObject)Resources.Load(currentLoadedRecording.getActorPreferedRepresentation(id));

				GameObject actor = null;

				if (actorsInScene != null) {
					
					for (int a = 0; a < actorsInScene.Length; a++) {

						if(actorsInScene[a].getNameForRecording() == currentLoadedRecording.getActorName (id)){

							actor = actorsInScene [a].gameObject;

							// Disable anything that might mess up playback
							PlaybackInterferenceBehavior[] interference = actor.GetComponents<PlaybackInterferenceBehavior>();

							for (int p = 0; p < interference.Length; p++) {

								interference [i].enabled = !interference[i].shouldDisableOnPlayback();

							}

							actorsAlreadyInSceneWhenStartPlaying.Add (actor);

						}
					}

				}

				if(actor == null){

					if (actorRef == null) {
						Debug.LogError ("Unable to find the resource you wanted to use to represent the actor!");
						actor = GameObject.CreatePrimitive (PrimitiveType.Cube);
					} else {
						actor = GameObject.Instantiate (actorRef);
					}

				}

				actors [id] = actor;
				actors [id].transform.name = currentLoadedRecording.getActorName (id);
				actors [id].transform.position = currentLoadedRecording.Frames [0].getPositionOfActor(id);
				actors [id].transform.rotation = Quaternion.Euler(currentLoadedRecording.Frames [0].getRotationOfActor(id));

			}

		}


		/// <summary>
		/// Stops the loaded recording, if there is one to be stopped
		/// </summary>
		public void stopLoadedRecording(){

			if (currentLoadedRecording == null) {
				Debug.LogError ("Trying to stop a recording when one isn't loaded yet!");
				return;
			}

			if(currentPlaybackState == PlaybackState.Stopped){
				Debug.LogWarning ("Trying to stop a recording that's already been stopped!");
				return;
			}

			clearCurrentLoadedRecording ();


		}


		/// <summary>
		/// If a recording is playing, then it will become paused.
		/// </summary>
		public void pauseLoadedRecording(){
			
			if (currentLoadedRecording == null) {
				Debug.LogError ("Trying to pause a recording when one isn't loaded yet!");
				return;
			}

			if (currentPlaybackState == PlaybackState.Playing) {
				currentPlaybackState = PlaybackState.Paused;
			}


		}


		/// <summary>
		/// The velocitys of the actors for smooth interpolation between frames
		/// </summary>
		private Vector3[] velocitys;

		void Update(){

			if (currentPlaybackState == PlaybackState.Playing) {

				// Determine whether or not it's time to go to the next frame

				if(frameCurrentelyOn == currentLoadedRecording.Frames.Length -1){

					if(getTimeThroughRecording() >= currentLoadedRecording.getDuration()){
						frameCurrentelyOn = 0;
						timeFrameStarted = Time.time;
						timePlaybackStarted = Time.time;
						timeSpentPaused = 0;
					}

				} else if(getTimeThroughRecording() >= currentLoadedRecording.Frames [frameCurrentelyOn + 1].TimeStamp -currentLoadedRecording.Frames [0].TimeStamp){
					frameCurrentelyOn++;
					timeFrameStarted = Time.time;
				}

				// Go through each actor and move them to their correct position
				int i = 0;
				foreach(KeyValuePair<int, GameObject> actor in actors){

					// Lerp through frames so no matter what the frame rate is it'll appear smooth
					actor.Value.transform.position = Vector3.SmoothDamp (actor.Value.transform.position, currentLoadedRecording.Frames[frameCurrentelyOn].getPositionOfActor(actor.Key), ref velocitys[i], 1f/currentLoadedRecording.FrameRateRecordedAt);
					actor.Value.transform.rotation = Quaternion.Euler(Vector3.Slerp (actor.Value.transform.rotation.eulerAngles, currentLoadedRecording.Frames[frameCurrentelyOn].getRotationOfActor(actor.Key), .1f));

					i++;
				}

			}

			if (currentPlaybackState == PlaybackState.Paused) {
				timeSpentPaused += Time.deltaTime;
			}

		}


		/// <summary>
		/// The time in seconds of how long the recording has been playing.
		/// </summary>
		/// <returns>The time through recording.</returns>
		private float getTimeThroughRecording(){
			return Time.time - timePlaybackStarted-timeSpentPaused;
		}


		/// <summary>
		/// Clears the current loaded recording of all actors in the scene
		/// </summary>
		private void clearCurrentLoadedRecording(){

			if (actors != null) {
				foreach (KeyValuePair<int, GameObject> actor in actors) {

					if(actorsAlreadyInSceneWhenStartPlaying.Contains(actor.Value)){

						PlaybackInterferenceBehavior[] componentstoReenable = actor.Value.GetComponents<PlaybackInterferenceBehavior> ();

						for (int i = 0; i < componentstoReenable.Length; i++) {
							componentstoReenable [i].enabled = true;
						}

						actorsAlreadyInSceneWhenStartPlaying.Remove (actor.Value);
					
					} else {
						Object.Destroy (actor.Value);
					}

				}

			}

			currentPlaybackState = PlaybackState.Stopped;
			actors = null;

		}

	}

}