using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TransferFunctionType
{
	Visual,
	Motor,
	Rotation,
	Drag
};

public class CurveRecorder : DataRecorder {

	[RequiredFieldAttribute, Tooltip("The vehicle from which curve data will be extracted.")]
	public Vehicle vehicle;

	[Tooltip("The transfer function from which data will be extracted.")]
	public TransferFunctionType transferFunction;

	internal override void OnReset()
	{
		this.outputDetails = new DataOutputDetails (string.Empty, 50, Mathf.Infinity, true, this.transferFunction.ToString() + "TransferFunctionData", "txt", "Values Per X Unit");
	}

	public void GetData()
	{
		this.CaptureData ();
	}

	internal override void CaptureData ()
	{
		this.dataList.Clear ();
		//capture data is only called from inspector
		this.CheckRecorderValidity ();
		if (this.isValid == false) {
			Debug.LogError ("Curve recorder configuration is invalid. Could not extract curve data.");
			return;
		}
		AnimationCurve anim = null;

		switch (this.transferFunction) {
		case TransferFunctionType.Visual:
			anim = vehicle.visualProcessingFunction.curve;
			break;
		case TransferFunctionType.Motor:
			anim = vehicle.visuoMotorFunction.curve;
			break;
		case TransferFunctionType.Rotation:
			anim = vehicle.rotationFunction.curve;
			break;
		case TransferFunctionType.Drag:
			anim = vehicle.vehicleDragFunction.curve;
			break;
		}
		//convert the rate to an interval that can be used to increment the x value
		float interval = MyRoutines.ConvertRateToInterval (this.outputDetails.dataCaptureRate);
		if (anim == null) {
			Debug.LogError ("Could not resolve curve.");
			return;
		}
		if (anim.length == 0) {
			Debug.LogError ("The curve does not contain any keyframes.");
			return;
		}
		float maxX = MyRoutines.AnimationCurveMaximumXValue (anim);
		float x = 0f;
		while (x < maxX) {
			this.dataList.Add (x, anim.Evaluate (x));
			x += interval;
		}
		//add in value for last x value
		this.dataList.Add (maxX, anim.Evaluate (maxX));
		//now save the data
		this.recording = true;
		this.SaveData ();
		this.recording = false;
		Debug.Log ("Saved data to " + this.outputDetails.outputPath);
	}

	internal override void CheckRecorderValidity()
	{
		base.CheckRecorderValidity ();
		if (this.vehicle == null) {
			Debug.LogError ("Vehicle reference is null.");
			isValid = false;
		}
	}


	internal override void OnStart()
	{

		//prevent the recorder from running during play mode
		recording = false;

	}
}
