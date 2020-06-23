using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(DropMarker))]
public class DropMarkerEditor : Editor {

	SerializedProperty useRate;
	SerializedProperty rate;
	SerializedProperty times;
	SerializedProperty targetTransform;
	SerializedProperty dropMarkerPrefab;

	void OnEnable()
	{
		useRate = serializedObject.FindProperty ("useRate");
		rate = serializedObject.FindProperty ("rate");
		times = serializedObject.FindProperty ("times");
		targetTransform = serializedObject.FindProperty ("targetTransform");
		dropMarkerPrefab = serializedObject.FindProperty ("dropMarkerPrefab");
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update ();
		EditorGUILayout.PropertyField (targetTransform);
		EditorGUILayout.PropertyField (dropMarkerPrefab);
		EditorGUILayout.PropertyField (useRate);
		if (useRate.boolValue) {
			//if using rate then display rate field
			EditorGUILayout.PropertyField (rate);
		} else {
			//display list of times
			EditorGUILayout.PropertyField (times, true);
		}
		serializedObject.ApplyModifiedProperties();
	}
}
	