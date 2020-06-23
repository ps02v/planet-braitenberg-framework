using UnityEngine;
using System.Collections;

[RequireComponent(typeof(RadialSensor))]
public class TerritorialBehaviour : BinocularVehicleBehaviourBase 
{

	[Range(0, 300), Tooltip("The amount of torque to apply when the vehicle is attacking the foreign object.")]
	public float attackTorque = 30f; 
	[Range(0, 300), Tooltip("The amount of torque to apply when the vehicle is rotating.")]
	public float rotateTorque = 100f; 
	[Range(0, 20), Tooltip("The steer angle of the vehicle's wheels (front and back) while it is rotating.")]
	public float rotationAmount = 20f; 

	[RequiredFieldAttribute, Tooltip("The vehicle's territory.")]
	public Territory territory; //the vehicle's territory

	private bool rotating = false; // indicates whether the vehicle is rotating
	private float previousRotation;
	private bool isTouching = false;
	protected RadialSensor rSensor;

	internal override void Start()
	{
		base.Start();
		this.rSensor = GetComponent<RadialSensor>();
	}
		

	internal override void Execute ()
	{
		//do not call Execute in base method - we are not using eyes
		if (territory.penetrated == true)
		{
			this.RemoveObject();
		}
		else
		{
			motorTorque = 0f;
			frontSteerAngle = 0f;
			rearSteerAngle = 0f;
		}
	}

	void OnCollisionStay(Collision collisionInfo)
	{//detect whether the foreign object is touching the vehicle
		if (collisionInfo.collider.tag == TagManager.ForeignObject) {
			this.isTouching = true;
		}
	}

	void OnCollisionExit(Collision collisionInfo)
	{//detect whether the foreign object has stopped touching the vehicle
		if (collisionInfo.collider.tag == TagManager.ForeignObject) {
			this.isTouching = false;
		}
	}

	void OnCollisionEnter(Collision collisionInfo)
	{//detect whether the foreign object has stopped touching the vehicle
		if (collisionInfo.collider.tag == TagManager.ForeignObject) {
			this.isTouching = true;
		}
	}

	private void RemoveObject()
	{
		float sAngle = 0f;
		bool flagHit = false;
		for (int i = 0; i < this.rSensor.numberOfRays; i++)
		{
			if (rSensor.rayCollision[i])
			{
				//if a ray has detected the foreign object
				float half = rSensor.numberOfRays / 2;
				flagHit = true;
				//determine whether we need to rotate left or right
				if (i > half)
				{
					sAngle = -rotationAmount;
				}
				else if ((i > 1) && (i < half))
				{
					sAngle = rotationAmount;
				}
				else
				{
					sAngle = 0f;
				}
				break;
			}
		}
		float mTorque = 0f;
		if ((sAngle == 0) && flagHit)
		{
			// target object is in front of vehicle
			mTorque = this.attackTorque;
			rotating = false;
		}
		else if (sAngle != 0)
		{
			// the vehicle needs to rotate
			mTorque = this.rotateTorque;
			rotating = true;
			//record the rotation of the previous update
			previousRotation = this.frontSteerAngle;
		}
		if (rotating && (flagHit == false))
		{//if the vehicle is rotating, but no collision was detected
			//then continue rotating in the same direction
			mTorque = this.rotateTorque;
			sAngle = previousRotation;
		}
		if (this.isTouching) {
			//if the foreign object is touching the vehicle, then we do not want to rotate the wheels
			mTorque = this.attackTorque;
			rotating = false;
			sAngle = 0f;
		}
		//set the torque and steer angles.
		motorTorque = mTorque;
		frontSteerAngle = sAngle;
		rearSteerAngle = -sAngle;
	}
}
