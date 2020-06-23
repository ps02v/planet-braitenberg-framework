using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

public class MyRoutines {

	public static void LightConfigurationGUI(bool value, ref Color color, ref Vector3 rotation, ref float intensity)
	{
		if (value) {
			color = EditorGUILayout.ColorField ("Color", color);
			rotation = EditorGUILayout.Vector3Field ("Rotation", rotation);
			intensity = EditorGUILayout.FloatField ("Intensity", intensity);
			if (GUILayout.Button ("Same As Directional Light")) {
				Light light = GameObject.FindGameObjectWithTag (TagManager.DirectionalLight).GetComponent<Light> ();
				color = light.color;
				rotation = light.transform.localEulerAngles;
				intensity = light.intensity;
			}
		}
	}

	public static float AnimationCurveMaximumXValue(AnimationCurve curve)
	{
		if (curve.length > 0) {
			Keyframe lastFrame = curve [curve.length - 1];
			return lastFrame.time;
		} else {
			throw new UnityException ("Tried to retrieve maximum value from empty curve.");
		}
	}

	public static bool CreateDirectory(string folderPath, bool deleteExisting)
	{
		try{
			if (System.IO.Directory.Exists(folderPath) && deleteExisting)
			{
				System.IO.Directory.Delete(folderPath, true);
			}
			else if (System.IO.Directory.Exists(folderPath) == false)
				System.IO.Directory.CreateDirectory(folderPath);
		}
		catch (System.Exception e) {
			Debug.LogError (e);
			return false;
		}  
		return true;
	}

	public static float ConvertRateToInterval(float rate)
	{
		return 1 / rate;
	}

	public static Vehicle GetPrimaryVehicle()
	{
		GameObject go = GameObject.FindGameObjectWithTag (TagManager.Vehicle);
		if (go == null)
			return null;
		return go.GetComponent<Vehicle> ();
	}

	public static float Round(float value, int digits)
	{//rounds a float value to the specified number of decimal places
		float mult = Mathf.Pow(10.0f, (float)digits);
		return Mathf.Round(value * mult) / mult;
	}

	public static string GetObjectPropertyString(string objectName, object o)
	{
		return objectName + ": " + o.ToString () + System.Environment.NewLine;
	}

	public static string GetTooltip(System.Type ty, string fieldName)
	{
		TooltipAttribute[] attributes = ty.GetField(fieldName).GetCustomAttributes (typeof(TooltipAttribute), true) as TooltipAttribute[];
		string s = "";
		if (attributes.Length > 0) {
			s = attributes [0].tooltip;
		}
		return s;
	}

	public static float GetEditorGUIStandardLineHeight()
	{
		return EditorGUIUtility.singleLineHeight;
	}

	public static float GetEditorGUILabelWidth()
	{
		return EditorGUIUtility.labelWidth;
	}

	public static float GetEditorGUIControlSpacing()
	{
		return 6f;
	}

	public static float GetEditorGUINextControlYPos(float yPos)
	{
		return yPos + MyRoutines.GetEditorGUIStandardLineHeight() + MyRoutines.GetEditorGUIStandardVerticalSpacing();
	}

	public static float GetEditorGUIStandardVerticalSpacing()
	{
		return EditorGUIUtility.standardVerticalSpacing;
	}

	public static float GetGUIPropertyHeight(int lines)
	{
		return (MyRoutines.GetEditorGUIStandardLineHeight() * lines) + (MyRoutines.GetEditorGUIStandardVerticalSpacing() * (lines - 1));
	}

//	public static void CreateScreenRecordingControl(string displayName, string tooltip, Rect pos, SerializedProperty outputFolder, SerializedProperty rate, string defaultFolderName)
//	{
//
//		float labelWidth = MyRoutines.GetEditorGUILabelWidth();
//		float padding = MyRoutines.GetEditorGUIControlSpacing();
//		float lineHeight = MyRoutines.GetEditorGUIStandardLineHeight ();
//		int folderButtonWidth = 25;
//
//		//create the label field
//		EditorGUI.LabelField (new Rect (pos.x, pos.y, labelWidth, lineHeight), new GUIContent (displayName, tooltip));
//		//reset the indent level
//		int indentLevel = EditorGUI.indentLevel;
//		EditorGUI.indentLevel = 0;
//		//disable the GUI
//		GUI.enabled = false;
//		if (string.IsNullOrEmpty(outputFolder.stringValue))
//			outputFolder.stringValue = System.IO.Path.Combine(Application.dataPath, defaultFolderName);
//		float outputFolderWidth = pos.width - labelWidth - folderButtonWidth - padding; 
//		//create the outputfolder field
//		EditorGUI.TextField (new Rect (pos.x + labelWidth, pos.y, outputFolderWidth, lineHeight), outputFolder.stringValue);
//		//renable the GUI
//		GUI.enabled = true;
//		float buttonXPos = pos.x + labelWidth + outputFolderWidth + padding; 
//		if (GUI.Button(new Rect(buttonXPos, pos.y, folderButtonWidth, lineHeight), "...")) {
//			string value = EditorUtility.OpenFolderPanel ("Select Output Folder", "", "");
//			if (string.IsNullOrEmpty (value) == false)
//				outputFolder.stringValue = value;
//		}
//		EditorGUI.indentLevel = indentLevel;
//		//get the vertical position of the next control
//		float ypos = MyRoutines.GetEditorGUINextControlYPos(pos.y);
//		EditorGUI.PropertyField(new Rect(pos.x, ypos, pos.width, lineHeight), rate);
//	}

	public static void CreateOutputPathControl(string displayName, string tooltip, Rect pos, SerializedProperty outputPath, bool isFile, string fileExtension, string defaultFileName)
	{

		float labelWidth = MyRoutines.GetEditorGUILabelWidth();
		float padding = MyRoutines.GetEditorGUIControlSpacing();
		float lineHeight = MyRoutines.GetEditorGUIStandardLineHeight ();
		int folderButtonWidth = 25;

		//create the label field
		EditorGUI.LabelField (new Rect (pos.x, pos.y, labelWidth, lineHeight), new GUIContent (displayName, tooltip));
		//reset the indent level
		int indentLevel = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;
		//disable the GUI
		GUI.enabled = false;
//		if (string.IsNullOrEmpty(outputPath.stringValue))
//			outputPath.stringValue = Application.dataPath;
		float outputFolderWidth = pos.width - labelWidth - folderButtonWidth - padding; 
		//create the outputfolder field
		EditorGUI.TextField (new Rect (pos.x + labelWidth, pos.y, outputFolderWidth, lineHeight), outputPath.stringValue);
		//renable the GUI
		GUI.enabled = true;
		float buttonXPos = pos.x + labelWidth + outputFolderWidth + padding; 
		if (GUI.Button(new Rect(buttonXPos, pos.y, folderButtonWidth, lineHeight), "...")) {
			string value = string.Empty;
			if (isFile == false) {
				//folder
				value = EditorUtility.OpenFolderPanel ("Select Output Folder", outputPath.stringValue, "");
			} else {
				//file
				value = EditorUtility.SaveFilePanel ("Save File", outputPath.stringValue, defaultFileName + "." + fileExtension, fileExtension);
			}
			if (string.IsNullOrEmpty (value) == false)
				outputPath.stringValue = value;
		}
		EditorGUI.indentLevel = indentLevel;
	}
}
