using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class AxleJaguarInfo
{
    public WheelCollider leftWheelCollider;

    public WheelCollider rightWheelCollider;

    public bool motor;                          //tells us if the wheel is attached to motor
    public bool steering;                       //tells us if the wheel applies a steer angle
}

public class JaguarController : MonoBehaviour {

    public List<AxleJaguarInfo> axleInfos;                  //List of all wheel pairs
    public float maxMotorTorque;                           //Maximum torque the motor can apply to the wheels
    public float maxSteeringAngle;                            // Maximum steer angle of the wheels

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void FixedUpdate()
    {
        float motor = maxMotorTorque * Input.GetAxis("Vertical");
        float steering = maxSteeringAngle * Input.GetAxis("Horizontal");

        foreach (AxleJaguarInfo axleInfo in axleInfos)
            {
            //Check the steering
            if (axleInfo.steering == true)
            {
                axleInfo.leftWheelCollider.steerAngle = steering;
                axleInfo.rightWheelCollider.steerAngle = steering;
            }

            //Check the motor
            if (axleInfo.motor == true)
            {
                axleInfo.leftWheelCollider.motorTorque = motor;
                axleInfo.rightWheelCollider.motorTorque = motor;
            }
            
        }
    }
}
