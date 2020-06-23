using UnityEngine;
using System.Collections;

[RequireComponent(typeof(RadialSensor))]
public class AvoidCaptureBehaviour : BinocularVehicleBehaviourBase
{

	[Tooltip("A multiplier that can be used to increase escape speed based on the proximity of pursuing vehicles.")]
	public float speedMultiplier = 2.0f;

	private float distance = -1;
	private RadialSensor rSensor; 
	
	internal override void Start()
	{
		base.Start();
		this.leftEye.useImaginedColor = true;
		this.rightEye.useImaginedColor = true;
		this.leftEye.imaginedColor = Color.black;
		this.rightEye.imaginedColor = Color.black;
		rSensor = GetComponent<RadialSensor> ();
	}

	private void EvadeObject()
	{
		this.vehicle.invertMotorTorque = false;
		this.leftEye.imaginedColor = Color.black;
		this.rightEye.imaginedColor = Color.black;
		for (int i = 0; i < rSensor.numberOfRays; i++)
		{
			if (rSensor.rayCollision[i])
			{
				//if a virtual whisker has detected the foreign object
				//determine whether we need to rotate left or right
				float half = rSensor.numberOfRays / 2;
				if (i > half)
				{
					//target is on left
					this.leftEye.imaginedColor = Color.black;
					this.rightEye.imaginedColor = Color.white;
				}
				else if ((i > 1) && (i < half))
				{
					//target is on right
					this.leftEye.imaginedColor = Color.white;
					this.rightEye.imaginedColor = Color.black;
				}
				else if (i == half)
				{
					//target is directly behind
					this.leftEye.imaginedColor = Color.white;
					this.rightEye.imaginedColor = Color.white;
				}
				else
				{
					//target is directly ahead
					this.leftEye.imaginedColor = Color.white;
					this.rightEye.imaginedColor = Color.white;
					this.vehicle.invertMotorTorque = true;
				}
			}
		}

	}
	
	internal override void Execute ()
	{
		this.EvadeObject ();
		base.Execute();
		distance = -1;
		for (int i = 0; i < rSensor.numberOfRays; i++)
		{
			if (rSensor.rayCollision[i])
			{
				distance = rSensor.hitDistance [i];
				break;
			}
		}
		if (distance > -1)
		{
			float proximity = this.rSensor.rayLength - distance;
			//the lower the proximity value, the faster we want the vehicle to move
			motorTorque += (speedMultiplier * proximity);
		}
	}
}