using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum VehicleProperty
{
	Speed,
	DistanceTravelled,
	MotorTorque
};

public class VehicleDataRecorder : DataRecorder {

	[RequiredFieldAttribute, Tooltip("The vehicle whose properties will be recorded.")]
	public Vehicle vehicle;

	[Tooltip("The property to record.")]
	public VehicleProperty property;

	internal override void OnReset()
	{
		this.outputDetails = new DataOutputDetails (string.Empty, 10, 30f, true, "Vehicle" + this.property.ToString() + "Data", "txt");
	}

	internal override void CheckRecorderValidity()
	{
		base.CheckRecorderValidity ();
		if (this.vehicle == null) {
			isValid = false;
		}
	}

	internal override void CaptureData ()
	{
		//save the vehicle data
		float x = Time.time;
		float y = 0f;
		switch (this.property) {
		case VehicleProperty.Speed:
			y = vehicle.speed;
			break;
		case VehicleProperty.DistanceTravelled:
			y = vehicle.distanceTravelled;
			break;
		case VehicleProperty.MotorTorque:
			y = vehicle.motorTorque;
			break;
		}
		this.dataList.Add (x, y);
	}
}
