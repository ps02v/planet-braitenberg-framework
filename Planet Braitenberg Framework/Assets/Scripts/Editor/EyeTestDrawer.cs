using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(EyeTest))]
public class EyeTestDrawer : PropertyDrawer {
	const int folderButtonWidth = 25;

	public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
	{
		SerializedProperty enabled = prop.FindPropertyRelative ("enabled");
		SerializedProperty outputFolder = prop.FindPropertyRelative ("outputFolder");
		int lines = 0;
		if (enabled.boolValue == false)
			lines = 1;
		else
			lines = 5;
		if (string.IsNullOrEmpty (outputFolder.stringValue) == false && enabled.boolValue == true)
			lines += 2;
		return MyRoutines.GetGUIPropertyHeight (lines);
	}


	public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
	{
		SerializedProperty enabled = prop.FindPropertyRelative ("enabled");
		SerializedProperty outputFolder = prop.FindPropertyRelative ("outputFolder");
		SerializedProperty rate = prop.FindPropertyRelative ("rate");
		SerializedProperty captureProcessedImage = prop.FindPropertyRelative ("captureProcessedImage");
		SerializedProperty stopTime = prop.FindPropertyRelative ("stopTime");
		float lineHeight = MyRoutines.GetEditorGUIStandardLineHeight ();

		EditorGUI.PropertyField(new Rect(pos.x, pos.y, pos.width, lineHeight), enabled, new GUIContent ("Eye Test", enabled.tooltip));

		if (enabled.boolValue) {
			EditorGUI.indentLevel++;
			string tooltip = MyRoutines.GetTooltip (typeof(EyeTest), "outputFolder");

			float ypos = MyRoutines.GetEditorGUINextControlYPos(pos.y);
			EditorGUI.PropertyField (new Rect (pos.x, ypos, pos.width, lineHeight), captureProcessedImage);
			ypos = MyRoutines.GetEditorGUINextControlYPos(ypos);
			Rect controlRect = new Rect (pos.x, ypos, pos.width, lineHeight); //lineheight
			//MyRoutines.CreateScreenRecordingControl (outputFolder.displayName, tooltip, controlRect, outputFolder, rate, "EyeTestData");
			MyRoutines.CreateOutputPathControl(outputFolder.displayName, tooltip, controlRect, outputFolder, false, string.Empty, string.Empty);

			if (string.IsNullOrEmpty (outputFolder.stringValue) == false) {
				ypos = MyRoutines.GetEditorGUINextControlYPos (ypos);
				controlRect = new Rect (pos.x, ypos, pos.width, lineHeight * 2);
				EditorGUI.HelpBox (controlRect, "The contents of the selected folder will be overwritten.", MessageType.Warning);
				ypos = MyRoutines.GetEditorGUINextControlYPos (ypos) + lineHeight;
			} else {
				ypos = MyRoutines.GetEditorGUINextControlYPos(ypos);
			}

			EditorGUI.PropertyField(new Rect(pos.x, ypos, pos.width, lineHeight), rate);
			ypos = MyRoutines.GetEditorGUINextControlYPos(ypos);
			EditorGUI.PropertyField(new Rect(pos.x, ypos, pos.width, lineHeight), stopTime);
			if (stopTime.floatValue <= 0)
				stopTime.floatValue = Mathf.Infinity;
			EditorGUI.indentLevel--;
		}
	}
}
