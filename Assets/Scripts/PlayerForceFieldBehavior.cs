using UnityEngine;
using System.Collections;

namespace CAVS {

	public class PlayerForceFieldBehavior : MonoBehaviour {

		[SerializeField]
		private GameObject target;

		private Vector3 posTargetLastFrame = Vector3.zero;

		private Material[] field;

		void Start(){
			
			field = gameObject.GetComponent<MeshRenderer> ().materials;

			if (target != null) {
				posTargetLastFrame = target.transform.position;
			}

		}

		// Update is called once per frame
		void Update () {

			if (target == null || field == null) {
				return;
			}

			float distanceFromCenterToEdge = transform.lossyScale.magnitude/2;

			// Get the current 'velocity' of the player
			float vel = Vector3.Distance (posTargetLastFrame, target.transform.position)/Time.deltaTime ;
			vel = 0; // Because this looks weird currentely

			for (int i = 0; i < field.Length; i++) {
				field[i].SetFloat ("_Strength", Mathf.Sqrt(Mathf.Sqrt(Vector3.Distance(transform.position, target.transform.position)/distanceFromCenterToEdge))*3 + vel );
			}

			posTargetLastFrame = target.transform.position;

		}
	}

}