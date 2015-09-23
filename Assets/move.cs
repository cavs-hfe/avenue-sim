using UnityEngine;
using System.Collections;

public class move : MonoBehaviour {

	private bool moveBack = false;
	public Transform startPosition;
	private Quaternion startOrientation;
	public float speed;
	public bool debug;
	public double timeout = 33.0;
	private float lastResetTime;
    
	void Start ()
	{
		startOrientation = gameObject.transform.rotation;

		if(speed==0)
			speed = Random.Range (15.0F, 30.0F);

		lastResetTime = Time.time;
	}

    void FixedUpdate () 
    {
        if (moveBack) {
			if (debug)
				Debug.Log (gameObject.name + " one");

			transform.position = startPosition.position;
			transform.rotation = startOrientation;
			moveBack = false;
			lastResetTime = Time.time;
		} else if ((Time.time - lastResetTime) > timeout) {
			transform.position = startPosition.position;
			transform.rotation = startOrientation;
			lastResetTime = Time.time;
		}
        else
        {
			if(debug)
				Debug.Log(gameObject.name + " two");

			transform.Translate(Vector3.forward * Time.deltaTime * speed);
        }
    }

        void OnTriggerEnter (Collider other)
        {
            if(other.tag == "Trigger")
            {
				if(debug)
					Debug.Log(gameObject.name + " triggered");

                moveBack = true;
            }

			if(debug)
				Debug.Log(gameObject.name + " three");
    }
}
