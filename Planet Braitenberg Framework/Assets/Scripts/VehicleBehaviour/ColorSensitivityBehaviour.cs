using UnityEngine;
using System.Collections;

public class ColorSensitivityBehaviour : PhotoTaxicBehaviour
{

	[Tooltip("The color sensitivity of the vehicle.")]
	public Color colorSensitivity = Color.red;

	internal override void Start()
	{
		base.Start();
		//make sure the vehicle can process color information
		this.leftEye.hasColorVision = true;
		this.rightEye.hasColorVision = true;
	}
	
	internal override float CalculateEyeOutput(Eye eye)
	{
		float red, green, blue;
		//get color information for the entire retine
		ColorInformation info = eye.GetEntireEyeInformation ();
		switch (this.eyeMode) {
		case EyeMode.Maximum:
			red = info.maxRed;
			green = info.maxGreen;
			blue = info.maxBlue;
			break;
		case EyeMode.Minimum:
			red = info.minRed;
			green = info.minGreen;
			blue = info.minBlue;
			break;
		default:
			red = info.averageRed;
			green = info.averageGreen;
			blue = info.averageBlue;
			break;
		}
		//now we have the color information for each color channel
		float diffRed, diffGreen, diffBlue;
		diffRed = Mathf.Abs(colorSensitivity.r - red);
		diffGreen = Mathf.Abs(colorSensitivity.g - green);
		diffBlue = Mathf.Abs(colorSensitivity.b - blue);
		//now we calculate an average value
		float average = (diffRed + diffGreen + diffBlue) / 3;
		//if we have the an exact match to the preferred color, average will equal 0
		//and if we have the exact opposite, the average will be 1
		return this.vehicle.EvaluateEyeBrightness(1 - average);
	}

	

}
