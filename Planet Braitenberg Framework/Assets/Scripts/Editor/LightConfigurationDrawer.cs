using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(LightConfiguration))]
public class LightConfigurationDrawer : PropertyDrawer {

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		int lines = 0;
		if (EditorGUIUtility.wideMode == false)
			lines = 5;
		else
			lines = 4;
		return MyRoutines.GetGUIPropertyHeight (lines);
	}

	public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
	{
		SerializedProperty color = prop.FindPropertyRelative ("color");
		SerializedProperty rotation = prop.FindPropertyRelative ("rotation");
		SerializedProperty intensity = prop.FindPropertyRelative ("intensity");
		float lineHeight = MyRoutines.GetEditorGUIStandardLineHeight ();
		float yPos = pos.y;
		//color
		EditorGUI.PropertyField (new Rect(pos.x, yPos, pos.width, lineHeight), color, new GUIContent (color.displayName, color.tooltip));
		//intensity
		yPos = MyRoutines.GetEditorGUINextControlYPos(yPos);
		EditorGUI.PropertyField (new Rect(pos.x, yPos, pos.width, lineHeight), intensity, new GUIContent (intensity.displayName, intensity.tooltip));
		//rotation
		yPos =MyRoutines.GetEditorGUINextControlYPos(yPos);
		EditorGUI.PropertyField (new Rect(pos.x, yPos, pos.width, lineHeight), rotation, new GUIContent (rotation.displayName, rotation.tooltip));
		yPos = MyRoutines.GetEditorGUINextControlYPos(yPos);
		if (EditorGUIUtility.wideMode == false)
			yPos = MyRoutines.GetEditorGUINextControlYPos(yPos);
		GUIContent buttonGUIContent = new GUIContent ("Same As Existing Directional Light", "Copies the settings of the scenes's main directional light.");
		if (GUI.Button(new Rect(pos.x, yPos, pos.width, lineHeight), buttonGUIContent))
			{
				Light light = GameObject.FindGameObjectWithTag (TagManager.DirectionalLight).GetComponent<Light> ();
				color.colorValue = light.color;
				rotation.vector3Value = light.transform.localEulerAngles;
				//rotation.vector3Value = light.transform.rotation.eulerAngles;
				intensity.floatValue = light.intensity;
			}
	}

}