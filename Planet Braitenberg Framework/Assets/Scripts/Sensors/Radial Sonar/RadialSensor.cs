using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent, RequireComponent(typeof(Vehicle))]
public class RadialSensor : MonoBehaviour {

	[Tooltip("The length of the sensor's rays.")]
	public float rayLength = 10f; //the length of the sonar ray
	[Tooltip("The number of rays associated with the sensor.")]
	public int numberOfRays = 12;

	[Tooltip("The elevation of the sensor, relative to the center of the vehicle's body.")]
	public float elevation = 0.5f;

	[Tooltip("A list of object tags. Specifies the kinds of objects that the radial sensor is sensitive to.")]
	public List<string> ObjectTags;

	internal bool[] rayCollision;
	internal float[] hitDistance;

	private Transform rayOrigin;

	void Start () {
		//create a new game object called RayOrigin
		GameObject go = new GameObject("RayOrigin");
		//parent it to the vehicle
		go.transform.parent = this.transform;
		//set the position to the center of the chassis
		go.transform.localPosition = Vector3.up * this.elevation;
		rayOrigin = go.transform;
		//initialise the array that will represent collisions with the vehicle's rays
		rayCollision = new bool[this.numberOfRays];
		hitDistance = new float[this.numberOfRays];
		for (int i = 0; i < this.numberOfRays; i++)
		{
			rayCollision[i] = false;
			hitDistance[i] = -1f;
		}
		rayOrigin.rotation = this.transform.rotation;
	}

	void Update () {
		{
			//reset ray collision arrays
			for (int i = 0; i < this.numberOfRays; i++)
			{
				rayCollision[i] = false;
				hitDistance [i] = -1;
			}
			for (int i = 0; i < this.numberOfRays; i++)
			{
				//detect whether a ray has collided with a target object
				float angle = i * (360 / this.numberOfRays);
				RaycastHit hit;
				if (Physics.Raycast(rayOrigin.position, this.GetTargetRotation(angle), out hit, rayLength))
				{
					if (this.ObjectTags.Contains(hit.collider.tag))
					{
						rayCollision[i] = true;
						hitDistance[i] = hit.distance;
					}
					else
					{
						rayCollision[i] = false;
						hitDistance [i] = -1;
					}
				}	
			}
			//draw the rays and color code them in a way that reflects their collision status	
			for (int i = 0; i < this.numberOfRays; i++)
			{
				float angle = i * (360 / this.numberOfRays);
				if (rayCollision[i])
				{
					Debug.DrawLine(rayOrigin.position, this.GetTargetPoint(angle), Color.red);
				}
				else
				{
					Debug.DrawLine(rayOrigin.position, this.GetTargetPoint(angle), Color.green);
				}
				if (i == 0)
				{
					Debug.DrawLine(rayOrigin.position, this.GetTargetPoint(angle), Color.magenta);
				}
			}	
		}
	}

	private Vector3 GetTargetPoint(float angle)
	{//get a target point for a ray
		return rayOrigin.position + GetTargetRotation(angle) * rayLength;
	}

	private Vector3 GetTargetRotation(float angle)
	{//get the rotation for a ray
		Quaternion startAngle = Quaternion.Euler (0, angle, 0);
		Quaternion transAngle = rayOrigin.rotation * startAngle;
		Vector3 tDirection = transAngle * Vector3.forward;
		return tDirection;
	}
}
