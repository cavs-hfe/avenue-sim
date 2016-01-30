using UnityEngine;

namespace CAVS.Recording {


	/// <summary>
	/// A frame represents a single capture of the scene of all the actors positions and rotations
	/// </summary>
	public struct Frame {


		int[] actorIds;


		Vector3[] positions;


		Vector3[] rotations;


		float timeStamp;


		public Frame(int[] actorIds, Vector3[] positions, Vector3[] rotations, float timeStamp){

			this.actorIds = actorIds;
			this.positions = positions;
			this.rotations = rotations;
			this.timeStamp = timeStamp;

		}


		public int[] ActorIds {
			get {
				return actorIds;
			}
			set {
				actorIds = value;
			}
		}


		public Vector3[] Positions {
			get {
				return positions;
			}
			set {
				positions = value;
			}
		}


		public Vector3[] Rotations {
			get {
				return rotations;
			}
			set {
				rotations = value;
			}
		}


		public float TimeStamp {
			get {
				return timeStamp;
			}
			set {
				timeStamp = value;
			}
		}

	}

}