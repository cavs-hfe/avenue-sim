using UnityEngine;
using System.Collections;

public class DisplayKinectFOV : MonoBehaviour {

	public float horizontalFOV = 70.0f;
	public float verticalFOV = 60.0f;
	public float displayDistance = 4.0f;
	private float halfHorizontalFOV;
	Vector3 leftRayDirection;
	Vector3 rightRayDirection;

	// Use this for initialization
	void Start () {
		halfHorizontalFOV = horizontalFOV / 2.0f;

		Quaternion leftRayRotation = Quaternion.AngleAxis (-halfHorizontalFOV, Vector3.up);
		Quaternion rightRayRotation = Quaternion.AngleAxis (halfHorizontalFOV, Vector3.up);

		leftRayDirection = leftRayRotation * transform.forward;
		rightRayDirection = rightRayRotation * transform.forward;

	}
	
	// Update is called once per frame
	void Update () {
		Gizmos.DrawRay (transform.position, leftRayDirection * displayDistance);
		Gizmos.DrawRay (transform.position, rightRayDirection * displayDistance);
	}
}
