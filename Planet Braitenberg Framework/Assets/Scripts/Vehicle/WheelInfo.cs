using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelInfo {

	public const int FrontLeft = 0;
	public const int FrontRight = 1;
	public const int BackLeft = 2;
	public const int BackRight = 3;

	public bool frontLeftGrounded;
	public bool frontRightGrounded;
	public bool backLeftGrounded;
	public bool backRightGrounded;
	public float frontLeftRPM;
	public float frontRightRPM;
	public float backLeftRPM;
	public float backRightRPM;
	public float frontLeftSteerAngle;
	public float frontRightSteerAngle;
	public float backLeftSteerAngle;
	public float backRightSteerAngle;

	internal bool IsStopped(bool applyToFrontWheels)
	{
		//determing whether the relevant wheel colliders have stopped moving
		//if applyToFrontWheels is false, then we determine whether the back wheel colliders are rotating
		//else we determine whether the front wheel colliders are rotating
		float averageAxleRPM;
		if (applyToFrontWheels == false) {
			averageAxleRPM = this.GetAverageRPMRearAxle ();
		} else {
			averageAxleRPM = this.GetAverageRPMFrontAxle ();
		}
		//now determine whether average RPM is zero
		//return Mathf.RoundToInt(averageAxleRPM) == 0;
		return averageAxleRPM == 0;
	}
		
	public float GetAverageRPMRearAxle()
	{
		return (backLeftRPM + backRightRPM) / 2;
	}

	public float GetAverageRPMFrontAxle()
	{
		return (frontLeftRPM + frontRightRPM) / 2;
	}

	public WheelInfo(Vehicle vehicle)
	{
		//is grounded
		this.frontLeftGrounded = vehicle.colls [WheelInfo.FrontLeft].isGrounded;
		this.frontRightGrounded = vehicle.colls [WheelInfo.FrontRight].isGrounded;
		this.backLeftGrounded = vehicle.colls [WheelInfo.BackLeft].isGrounded;
		this.backRightGrounded = vehicle.colls [WheelInfo.BackRight].isGrounded;
		//rpm
		this.frontLeftRPM = vehicle.colls [WheelInfo.FrontLeft].rpm;
		this.frontRightRPM = vehicle.colls [WheelInfo.FrontRight].rpm;
		this.backLeftRPM = vehicle.colls [WheelInfo.BackLeft].rpm;
		this.backRightRPM = vehicle.colls [WheelInfo.BackRight].rpm;
		//steer angle
		this.frontLeftSteerAngle = vehicle.colls [WheelInfo.FrontLeft].steerAngle;
		this.frontRightSteerAngle = vehicle.colls [WheelInfo.FrontRight].steerAngle;
		this.backLeftSteerAngle = vehicle.colls [WheelInfo.BackLeft].steerAngle;
		this.backRightSteerAngle = vehicle.colls [WheelInfo.BackRight].steerAngle;

	}

	public WheelInfo(Vehicle vehicle, float threshold)
		:this(vehicle)
	{
		//rpm
		if (this.frontLeftRPM <= threshold)
			this.frontLeftRPM = 0f;
		if (this.frontRightRPM <= threshold)
			this.frontRightRPM = 0f;
		if (this.backLeftRPM <= threshold)
			this.backLeftRPM = 0f;
		if (this.backRightRPM <= threshold)
			this.backRightRPM = 0f;
	}
}
