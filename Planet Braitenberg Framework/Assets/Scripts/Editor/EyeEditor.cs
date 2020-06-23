using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Eye))]
public class EyeEditor : Editor {

	SerializedProperty canSeeDirectionalLight;
	SerializedProperty lightConfig;

	void OnEnable()
	{
		canSeeDirectionalLight = serializedObject.FindProperty ("canSeeDirectionalLight");
		lightConfig = serializedObject.FindProperty ("lightConfig");
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update ();
		DrawDefaultInspector();
		if (canSeeDirectionalLight.boolValue) {
			EditorGUI.indentLevel++;
			EditorGUILayout.PropertyField (lightConfig);
			EditorGUI.indentLevel--;
		}
		serializedObject.ApplyModifiedProperties();

	}
}
	