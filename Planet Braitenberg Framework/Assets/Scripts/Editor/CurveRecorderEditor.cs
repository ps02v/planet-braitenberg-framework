using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(CurveRecorder))]
public class CurveRecorderEditor : Editor {

	public override void OnInspectorGUI()
	{
		CurveRecorder curveRec = (CurveRecorder)target;
		serializedObject.Update ();
		DrawDefaultInspector();
		if (GUILayout.Button ("Extract Data")) {
			curveRec.GetData ();
		}
		serializedObject.ApplyModifiedProperties();

	}
}
	