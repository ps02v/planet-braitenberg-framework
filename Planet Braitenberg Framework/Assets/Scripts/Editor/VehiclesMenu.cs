using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class VehiclesMenu {

	[MenuItem("Vehicles/Print Light Settings")]
	private static void GetLightSettings()
	{
		string output = "";
		string newLine = System.Environment.NewLine;
		output += MyRoutines.GetObjectPropertyString ("Ambient Light", RenderSettings.ambientLight);
		output += MyRoutines.GetObjectPropertyString ("Ambient Intensity", RenderSettings.ambientIntensity);
		output += MyRoutines.GetObjectPropertyString ("Ambient Mode", RenderSettings.ambientMode);
		output += MyRoutines.GetObjectPropertyString ("Fog", RenderSettings.fog);
		output += MyRoutines.GetObjectPropertyString ("Fog Color", RenderSettings.fogColor);
		output += MyRoutines.GetObjectPropertyString ("Fog Mode", RenderSettings.fogMode);
		output += MyRoutines.GetObjectPropertyString ("Fog Start", RenderSettings.fogStartDistance);
		output += MyRoutines.GetObjectPropertyString ("Fog End", RenderSettings.fogEndDistance);
		output += MyRoutines.GetObjectPropertyString ("Fog Skybox", RenderSettings.skybox);
		output += MyRoutines.GetObjectPropertyString ("Sun ", RenderSettings.sun);
		Debug.Log (output);
	}

//	[MenuItem("Vehicles/Reset Capture Frame Rate")]
//	private static void ResetCaptureFrameRate()
//	{
//		Time.captureFramerate = 0;
//	}

	[MenuItem("Vehicles/Apply Default Light Settings")]
	private static void ApplyDefaultLightSettings()
	{
//		Ambient Light: RGBA(0.500, 0.500, 0.500, 1.000)
//		Ambient Intensity: 1.09
//		Ambient Mode: Flat
//		Fog: True
//		Fog Color: RGBA(0.110, 0.098, 0.106, 1.000)
//		Fog Mode: Linear
//		Fog Start: 10
//		Fog End: 50
//		Fog Skybox: DawnDusk Skybox (UnityEngine.Material)
//		Sun : Sun (UnityEngine.Light)

		RenderSettings.ambientLight = new Color (0.500f, 0.500f, 0.500f, 1.000f);
		RenderSettings.ambientIntensity = 1.09f;
		RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
		RenderSettings.fog = true;
		RenderSettings.fogColor = new Color (0.110f, 0.098f, 0.106f, 1.000f);
		RenderSettings.fogMode = FogMode.Linear;
		RenderSettings.fogStartDistance = 10;
		RenderSettings.fogEndDistance = 50;
		RenderSettings.sun = GameObject.FindGameObjectWithTag(TagManager.DirectionalLight).GetComponent<Light>();
	}


}
