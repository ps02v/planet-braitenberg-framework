using UnityEngine;
using System.Collections;

public class BinocularVehicleBehaviourBase: VehicleBehaviourBase {
	[Tooltip("The vehicle's left eye.")]
	public Eye leftEye;
	[Tooltip("The vehicle's right eye.")]
	public Eye rightEye;

	internal float leftEyeOutput = 0f;
	internal float rightEyeOutput = 0f;

	internal override void Start()
	{
		base.Start ();
		if (this.leftEye == null) {
			foreach (Eye e in GetComponents<Eye>()) {
				if (e.EyeTag == "LeftEye") {
					this.leftEye = e;
				}
			}
		} else {
			//check the tag of the eye
			this.CheckEyeTag(this.leftEye, "LeftEye");
		}
		if (this.rightEye == null) {
			foreach (Eye e in GetComponents<Eye>()) {
				if (e.EyeTag == "RightEye") {
					this.rightEye = e;
				}
			}
		}
		else {
			//check the tag of the eye
			this.CheckEyeTag(this.rightEye, "RightEye");
		}
		this.eyes.Add (leftEye);
		this.eyes.Add (rightEye);
		this.eyeCollection.Add (this.leftEye.EyeTag, this.leftEyeOutput);
		this.eyeCollection.Add (this.rightEye.EyeTag, this.rightEyeOutput);
		//render both of the eyes to ensure that data can be displayed in the eye UI
		this.leftEye.Execute ();
		this.rightEye.Execute ();
	}

	protected void CheckEyeTag(Eye e, string eyeTag)
	{
		if (e.EyeTag != eyeTag) {
			Debug.LogWarning (eyeTag + " does match type of vehicle's eye.");
		}
	}
	
	internal virtual float GetLeftEyeOutput()
	{
		//calculate the output of the left eye based on the retinal transfer function.
		return this.CalculateEyeOutput(this.leftEye);
	}
	
	internal virtual float GetRightEyeOutput()
	{
		//calculate the output of the right eye based on the retinal transfer function.
		return this.CalculateEyeOutput(this.rightEye);
	}
		
	internal override void Execute()
	{
		//render both of the eyes
		this.leftEye.Execute ();
		this.rightEye.Execute ();
		//calculate the output of each of the eyes
		leftEyeOutput = this.GetLeftEyeOutput();
		rightEyeOutput = this.GetRightEyeOutput();
		this.eyeCollection [this.leftEye.EyeTag] = leftEyeOutput;
		this.eyeCollection [this.rightEye.EyeTag] = rightEyeOutput;
		//round the output values to 3 decimal places
		//leftEyeOutput = VehicleBehaviourBase.Round (leftEyeOutput, 3);
		//rightEyeOutput = VehicleBehaviourBase.Round (rightEyeOutput, 3);

		//calculate the driving force based on the average of eye outputs
		motorTorque = this.vehicle.EvaluateMotorTorque((leftEyeOutput + rightEyeOutput) / 2);
		//calculate the steering angle
		rotationDelta = rightEyeOutput - leftEyeOutput;
		//if right eye output is at maximum (1) and left is minimum (0), then we will have a value of 1
		//if left eye output is at maximum (1) and right is minimum (0), then we will have a value of -1
		//therefore rotation delta varies from -1 (turn left) to 1 (turn right)
		
		//we pass the absolute value of the rotationDelta to the evaluation rotation function to get our target steering angle
		float absSteerAngle = this.vehicle.EvaluateRotation(Mathf.Abs(rotationDelta));
		//the EvaluateRotation function returns a value between 0 and 1, so we need to change the sign to reflect the direction of turn
		//change the sign of the steer angle
		if (rotationDelta < 0)
		{
			//indicates a left turn
			frontSteerAngle = absSteerAngle * -1;
		}
		else
		{
			//indicates a right turn
			frontSteerAngle = absSteerAngle;
		}
		//for VehicleBehaviourBase, we do not calculate the rearSteerAngle - it is left at the default value of 0f
		//invertMotorTorque is left at the default value, which is false. By default, the vehicle will move forward.
	}
}
