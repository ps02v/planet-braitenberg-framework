using UnityEngine;
using System.Collections;


public class UserControlledBehaviour : VehicleBehaviourBase
{
	[Tooltip("A coefficient affecting the rate of change in the vehicle's speed."), Range(0f, 1f)]
	public float moveSpeed = 0.5f;
	[Tooltip("A coefficient affecting the rate of change in the vehicle's steering."), Range(0f, 1f)]
	public float steerSpeed = 0.5f;
	[Tooltip("The amount of brake torque to apply during vehicle braking.")]
	public float defaultBrakeTorque = 50f;

	
	internal override void Execute ()
	{
		//set the motortorque to the absolute value of the input vertical axis
		this.motorTorque = 300 * Mathf.Abs(Input.GetAxis ("Vertical"));
		//if moving backward then set invertMotorTorque to true
		this.invertMotorTorque = Input.GetAxis ("Vertical") < 0;
		//change the steering angle based on the horizontal input axis
		this.frontSteerAngle = Input.GetAxis ("Horizontal") * 20;
		if (Input.GetAxis ("Jump") > 0) {
			//apply brake torque
			this.brakeTorque = this.defaultBrakeTorque;
			this.motorTorque = 0;
		} else {
			this.brakeTorque = 0;
		}
	}
}
