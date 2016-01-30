using System.Collections.Generic;
using UnityEngine;

namespace CAVS.Recording {

	/// <summary>
	/// A single recording of what happened in a certain scene with x amount of actors
	/// </summary>
	public class Recording {


		/// <summary>
		/// The name of the recording that will be used for saving
		/// </summary>
		private string name;


		/// <summary>
		/// The frame rate that this recording was created with.
		/// </summary>
		private float frameRateRecordedAt;


		/// <summary>
		/// The actor unique gameobject identifiers.
		/// </summary>
		private int[] actorIds;


		/// <summary>
		/// All our frames of our recording
		/// </summary>
		private List<Frame> frames;


		/// <summary>
		/// Dictionary for getting an actors name by using
		/// it's unique id for lookup
		/// </summary>
		private Dictionary<int, string> actorNames;


		/// <summary>
		/// Dictionary for getting an actors prefered representation by using
		/// it's unique id for lookup
		/// </summary>
		private Dictionary<int, PrimitiveType> actorRepresentations;


		/// <summary>
		/// Initializes a new instance of the <see cref="CAVS.Recording.Recording"/> class.
		/// Recordings must have atleast 1 actor, a name, and a framerate greater that 0
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="frameRateRecordedAt">Frame rate recorded at.</param>
		/// <param name="actorIds">Actor unique gameobject identifiers.</param>
		/// <param name="actorNames">Actor names.</param>
		/// <param name="actorRepresentations">Actor representations.</param>
		public Recording(string name, float frameRateRecordedAt, int[] actorIds, string[] actorNames, PrimitiveType[] actorRepresentations){

			if (actorIds == null || actorIds.Length == 0) {
				throw new System.ArgumentException ("You need atleast 1 actor to make a recording!", "actorIds");
			}

			if (name == null || name == "") {
				throw new System.ArgumentException ("You need a valid name for the recording!", "name");
			}

			if (frameRateRecordedAt <= 0) {
				throw new System.ArgumentException ("You need a frame rate greater than 0!", "frameRateRecordedAt");
			}

			this.name = name;
			this.frameRateRecordedAt = frameRateRecordedAt;
			this.actorIds = actorIds;

			// Set up the names!
			this.actorNames = new Dictionary<int, string> ();
			for (int i = 0; i < actorNames.Length; i++) {
				this.actorNames [actorIds[i]] = actorNames[i];
			}

			// Set up their representations! 
			this.actorRepresentations = new Dictionary<int, PrimitiveType> ();
			for (int i = 0; i < actorRepresentations.Length; i++) {
				this.actorRepresentations [actorIds[i]] = actorRepresentations[i];
			}

			this.frames = new List<Frame> ();

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CAVS.Recording.Recording"/> class.
		/// FOR SEARILZATION PURPOSES
		/// </summary>
		public Recording(){

		}

		/// <summary>
		/// Adds a frame to the recording at hand
		/// </summary>
		/// <param name="frame">Frame.</param>
		public void addFrame(Frame frame){

			frames.Add(frame);

		}
			

		public string Name {
			get {
				return name;
			}
			set {
				name = value;
			}
		}

		public float FrameRateRecordedAt {
			get {
				return frameRateRecordedAt;
			}
			set {
				frameRateRecordedAt = value;
			}
		}

		public int[] ActorIds {
			get {
				return actorIds;
			}
			set {
				actorIds = value;
			}
		}

		public List<Frame> Frames {
			get {
				return frames;
			}
			set {
				frames = value;
			}
		}
	}

}