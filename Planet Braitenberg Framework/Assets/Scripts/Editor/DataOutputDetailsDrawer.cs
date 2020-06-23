using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(DataOutputDetails))]
public class DataOutputDetailsDrawer : PropertyDrawer {

	public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
	{
		SerializedProperty outputPath = prop.FindPropertyRelative ("outputPath");
		int lines = 3;
		if (string.IsNullOrEmpty (outputPath.stringValue) == false)
			lines += 2;
		return MyRoutines.GetGUIPropertyHeight (lines);
	}


	public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
	{
		SerializedProperty outputPath = prop.FindPropertyRelative ("outputPath");
		SerializedProperty dataCaptureRate = prop.FindPropertyRelative ("dataCaptureRate");
		SerializedProperty stopTime = prop.FindPropertyRelative ("stopTime");
		float lineHeight = MyRoutines.GetEditorGUIStandardLineHeight ();
		DataOutputDetails outputDetails = fieldInfo.GetValue (prop.serializedObject.targetObject) as DataOutputDetails;
		bool isFile = outputDetails.isFile;
		string fileExtension = outputDetails.fileExtension;
		string defaultFileName = outputDetails.defaultFileName;
		string tooltip = MyRoutines.GetTooltip (typeof(DataOutputDetails), "outputPath");
		//create the output path control
		MyRoutines.CreateOutputPathControl (outputPath.displayName, tooltip, pos, outputPath, isFile, fileExtension, defaultFileName);
		float ypos;
		if (string.IsNullOrEmpty (outputPath.stringValue) == false) {
			ypos = MyRoutines.GetEditorGUINextControlYPos (pos.y);
			Rect controlRect = new Rect (pos.x, ypos, pos.width, lineHeight * 2);
			EditorGUI.HelpBox (controlRect, "The contents of the selected folder will be overwritten.", MessageType.Warning);
			ypos = MyRoutines.GetEditorGUINextControlYPos (ypos) + lineHeight;
		} else {
			ypos = MyRoutines.GetEditorGUINextControlYPos(pos.y);
		}
		//create the datacapture rate field
		tooltip = MyRoutines.GetTooltip (typeof(DataOutputDetails), "dataCaptureRate");
		string rateDisplayName = outputDetails.rateFieldLabel;
		EditorGUI.PropertyField (new Rect (pos.x, ypos, pos.width, lineHeight), dataCaptureRate, new GUIContent(rateDisplayName, tooltip));
		//create the stopTime field
		ypos = MyRoutines.GetEditorGUINextControlYPos(ypos);
		EditorGUI.PropertyField (new Rect (pos.x, ypos, pos.width, lineHeight), stopTime);
		if (stopTime.floatValue <= 0)
			stopTime.floatValue = Mathf.Infinity;
		}
}
