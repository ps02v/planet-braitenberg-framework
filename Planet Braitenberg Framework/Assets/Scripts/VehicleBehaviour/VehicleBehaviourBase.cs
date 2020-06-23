using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Vehicle)), DisallowMultipleComponent]
public abstract class VehicleBehaviourBase : MonoBehaviour {

	//Vehicle behaviours are used to compute behaviour-related information for the vehicle
	//This includes the steering angle for the wheels and the motor torque,

	[Tooltip("Specifies whether the eye output is based on maximum, average, or minimum brightness levels.")]
	public EyeMode eyeMode = EyeMode.Average;
	[Tooltip("Specifies how the eye will render on the relevant eye UI. Thus does not affect visual processing!")]
	public EyeMode eyeDisplayMode = EyeMode.Average;

	internal Dictionary<string, float> eyeCollection = new Dictionary<string, float> ();
	internal List<Eye> eyes;
	internal Vehicle vehicle;
	internal float motorTorque = 0f;
	internal float rotationDelta = 0f;
	internal float frontSteerAngle = 0f;
	internal float rearSteerAngle = 0f;
	internal float brakeTorque = 0f;
	internal bool invertMotorTorque = false;

	void Awake()
	{
		this.OnAwake ();
	}

	internal virtual void OnAwake()
	{
		//get a reference to the vehicle component
		this.vehicle = GetComponent<Vehicle>();
	}

	internal virtual void Start()
	{
		//instantiate the list of eyes.
		this.eyes = new List<Eye>();
	}
		
	internal virtual void Update()
	{
		foreach (Eye e in this.eyes) {
			e.eyeDisplayMode = this.eyeDisplayMode;
		}
	}

	internal virtual float CalculateEyeOutput(Eye eye)
	{
		//calculates the output for a specific eye
		//by default eye output is calculated for the entire eye
		ColorInformation eyeInfo = eye.GetEntireEyeInformation ();
		switch (this.eyeMode) {
		case EyeMode.Maximum:
			return eyeInfo.maxBrightness;
		case EyeMode.Minimum:
			return eyeInfo.minBrightness;
		default:
			return eyeInfo.averageBrightness;
		}
	}
		
	abstract internal void Execute (); //Execute is called by the vehicle component to compute behaviour-related information
}
