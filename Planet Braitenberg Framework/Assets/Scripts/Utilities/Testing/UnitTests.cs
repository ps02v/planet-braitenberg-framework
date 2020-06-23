using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public enum TestType
{
	MoveForward,
	MoveBackward,
	MoveForwardAndStop,
	MoveBackwardAndStop,
	MoveForwardAndBrake,
	MoveBackwardAndBrake,
	MoveBackwardAndBrakeThenForward,
	SteerLeftThenRight,
	SteerRightThenReturn,
	AutoStopMoveForward,
	AutoStopMoveForwardThenBackward
};

public class UnitTests : MonoBehaviour {

	public TestType test = TestType.MoveForward;
	public Vehicle[] vehicles;
	public float motorTorque;
	public float brakeTorque;
	//public bool invertMotor;
	public float triggerTime = 10f;
	[Range(-20, 20)]
	public float frontSteerAngle = 0f;
	[Range(-20, 20)]
	public float rearSteerAngle = 0f;

	[Space]
	[ReadOnlyAttribute]
	public float frontLeftRPM;
	[ReadOnlyAttribute]
	public float frontRightRPM;
	[ReadOnlyAttribute]
	public float backLeftRPM;
	[ReadOnlyAttribute]
	public float backRightRPM;

	[Space]
	[ReadOnlyAttribute]
	public float rearAxleRPM;

	[Space]
	[ReadOnlyAttribute]
	public float speed;
	[ReadOnlyAttribute]
	public bool isStationary;
	[ReadOnlyAttribute]
	public bool isStopped;

	[ReadOnlyAttribute, HideInInspector]
	public Vector3 wheelRotation;
	[ReadOnlyAttribute, HideInInspector]
	public float wheelRotationPerSecond;

	private bool runTrigger = false;
	//private float triggerTime;
	//private Vehicle vehicle;
	private RunTest trigger;
	private bool triggerExecuted = false;
	private float previousRotation = 0;

	void Start () {
		switch (test) {
		case TestType.MoveForward:
			this.MoveForward ();
			break;
		case TestType.MoveBackward:
			this.MoveBackward ();
			break;
		case TestType.MoveForwardAndStop:
			this.MoveForwardAndStop();
			break;
		case TestType.MoveBackwardAndStop:
			this.MoveBackwardAndStop ();
			break;
		case TestType.MoveForwardAndBrake:
			this.MoveForwardAndBrake ();
			break;
		case TestType.MoveBackwardAndBrake:
			this.MoveBackwardAndBrake ();
			break;
		case TestType.MoveBackwardAndBrakeThenForward:
			this.MoveBackwardAndBrakeThenForward ();
			break;
		case TestType.SteerLeftThenRight:
			this.SteerLeftThenRight ();
			break;
		case TestType.SteerRightThenReturn:
			this.SteerRightThenReturn ();
			break;
		case TestType.AutoStopMoveForward:
			this.AutoStopMoveForward ();
			break;
		case TestType.AutoStopMoveForwardThenBackward:
			this.AutoStopMoveForwardThenBackward ();
			break;
		}
		Controller gControl = GameObject.FindGameObjectWithTag (TagManager.Controller).GetComponent<Controller> ();
		gControl.showInfoUI = true;
	}

	void AutoStopMoveForwardThenBackward()
	{
		InitVehicles (false);
		foreach (Vehicle vehicle in this.vehicles) {
			vehicle.implementAutoStop = true;
			vehicle.autoStopBrakeTorque = this.brakeTorque;
		}
		runTrigger = true;
		trigger = new RunTest (ToggleMotorTorque);
	}

	void ToggleMotorTorque()
	{
		foreach (Vehicle vehicle in this.vehicles) {
			vehicle.invertMotorTorque = !vehicle.invertMotorTorque;
		}
	}

	void AutoStopMoveForward()
	{
		InitVehicles (false);
		foreach (Vehicle vehicle in this.vehicles) {
			vehicle.implementAutoStop = true;
			vehicle.autoStopBrakeTorque = this.brakeTorque;
		}
		runTrigger = true;
		trigger = new RunTest (StopVehicles);
	}

	void SteerRightThenReturn()
	{
		InitVehicles (true);
		foreach (Vehicle vehicle in this.vehicles) {
			vehicle.frontSteerAngle = -20f;
		}
		runTrigger = true;
		trigger = new RunTest (RotateZero);
	}

	void RotateZero()
	{
		foreach (Vehicle vehicle in this.vehicles) {
			vehicle.frontSteerAngle = 0f;
		}
	}

	void SteerLeftThenRight()
	{
		InitVehicles (true);
		foreach (Vehicle vehicle in this.vehicles) {
			vehicle.frontSteerAngle = -20f;
		}
		runTrigger = true;
		trigger = new RunTest (RotateRight);
	}

	void RotateRight()
	{
		foreach (Vehicle vehicle in this.vehicles) {
			vehicle.frontSteerAngle = 20f;
		}
	}

	void MoveBackwardAndBrakeThenForward()
	{
		InitVehicles (true);
		runTrigger = true;
		//triggerTime = this.triggerTime;
		trigger = new RunTest (AddBrakeTorque);
	}

	void MoveBackwardAndBrake()
	{
		InitVehicles (true);
		runTrigger = true;
		//triggerTime = this.triggerTime;
		trigger = new RunTest (AddBrakeTorque);
	}

	void MoveBackwardAndStop()
	{
		InitVehicles (true);
		runTrigger = true;
		//triggerTime = this.triggerTime;
		trigger = new RunTest (StopVehicles);
	}

	void MoveForwardAndBrake()
	{
		InitVehicles (false);
		runTrigger = true;
		//triggerTime = this.triggerTime;
		trigger = new RunTest (AddBrakeTorque);
	}

	void MoveForwardAndStop()
	{
		InitVehicles (false);
		runTrigger = true;
		//triggerTime = this.triggerTime;
		trigger = new RunTest (StopVehicles);
	}

	void AddBrakeTorque()
	{
		foreach (Vehicle vehicle in this.vehicles) {
			vehicle.motorTorque = 0f;
			vehicle.brakeTorque = this.brakeTorque;
		}
	}

	void StopVehicles()
	{
		foreach (Vehicle vehicle in this.vehicles) {
			vehicle.motorTorque = 0f;
		}

	}

	void MoveForward()
	{
		InitVehicles (false);
	}

	void MoveBackward()
	{
		InitVehicles (true);
	}
	

	void FixedUpdate () {
		Vehicle v = this.vehicles [0];
		if (test == TestType.MoveBackwardAndBrakeThenForward && this.triggerExecuted) {
			//if (v.GetWheelInfo ().IsStopped (true)) {
			if (v.IsStationary()) {
				//the vehicle was reversing and then braked
				//it has now stopped
				//apply forward

				v.invertMotorTorque = false;
				v.brakeTorque = 0f;
				v.motorTorque = this.motorTorque;
			}
		}
		if (test == TestType.AutoStopMoveForwardThenBackward && this.triggerExecuted) {
			v.motorTorque = this.motorTorque;
		}
		if (runTrigger && (Time.time >= triggerTime) && triggerExecuted == false) {
			this.triggerExecuted = true;
			trigger ();
		}
//		if (test == TestType.MoveForwardAndBrake) {
//			this.PrintWheelRPM (); 
//		}
		this.PrintWheelRPM(v);
		this.speed = v.GetSpeed ();
		this.isStationary = v.IsStationary ();
		this.isStopped = v.IsStopped();
		Quaternion q;
		Vector3 p;
		v.colls[WheelInfo.BackLeft].GetWorldPose(out p, out q);
		this.wheelRotation = q.eulerAngles;
		this.wheelRotationPerSecond = (this.wheelRotation.x - this.previousRotation) / Time.fixedDeltaTime;
		this.previousRotation = this.wheelRotation.x;
	}

	private void PrintWheelRPM(Vehicle v)
	{
		WheelInfo info = v.GetWheelInfo ();
		this.frontLeftRPM = info.frontLeftRPM;
		this.frontRightRPM = info.frontRightRPM;
		this.backLeftRPM = info.backLeftRPM;
		this.backRightRPM = info.backRightRPM;
		this.rearAxleRPM = info.GetAverageRPMRearAxle ();
		//Debug.Log ("Time: " + Time.time + "; " + info.frontLeftSteerAngle.ToString ());
	}

	private void InitVehicles(bool invertMotor)
	{
		foreach (Vehicle vehicle in this.vehicles) {
			vehicle.motorTorque = this.motorTorque;
			vehicle.testMode = true;
			vehicle.disableMotor = false;
			vehicle.invertMotorTorque = invertMotor;
			vehicle.frontSteerAngle = this.frontSteerAngle;
			vehicle.rearSteerAngle = this.rearSteerAngle;
		}
	}
}

public delegate void RunTest();
