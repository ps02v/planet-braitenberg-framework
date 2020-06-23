using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Vehicle))]
public class VehicleEditor : Editor {

	SerializedProperty motorTorque;
	SerializedProperty brakeTorque;
	SerializedProperty frontSteerAngle;
	SerializedProperty rearSteerAngle;
	SerializedProperty invertMotorTorque;
	SerializedProperty testMode;


	void OnEnable()
	{
		motorTorque = serializedObject.FindProperty ("motorTorque");
		brakeTorque = serializedObject.FindProperty ("brakeTorque");
		frontSteerAngle = serializedObject.FindProperty ("frontSteerAngle");
		rearSteerAngle = serializedObject.FindProperty ("rearSteerAngle");
		invertMotorTorque = serializedObject.FindProperty ("invertMotorTorque");
		testMode = serializedObject.FindProperty ("testMode");
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update ();
		DrawDefaultInspector();
		if (testMode.boolValue) {
			EditorGUI.indentLevel++;
			//EditorGUILayout.PropertyField (motorTorque);
			EditorGUILayout.Slider (motorTorque, 0, 300);
			EditorGUILayout.Slider (brakeTorque, 0, 300);
			EditorGUILayout.Slider (frontSteerAngle, -20, 20);
			EditorGUILayout.Slider (rearSteerAngle, -20, 20);
			EditorGUILayout.PropertyField (invertMotorTorque);
			EditorGUI.indentLevel--;
		}
		serializedObject.ApplyModifiedProperties();

	}
}
	