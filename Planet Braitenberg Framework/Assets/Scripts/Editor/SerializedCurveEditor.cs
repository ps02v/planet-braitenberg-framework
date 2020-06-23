using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SerializedCurve))]
public class SerializedCurveEditor : Editor {

	public override void OnInspectorGUI()
	{
		//serializedObject.Update();
		SerializedCurve curve = (SerializedCurve)this.target;
		this.DrawHeader ();
		EditorGUILayout.LabelField ("Curve Type", curve.curveType);
		EditorGUILayout.CurveField ("Curve", curve.curve.ToAnimationCurve());
		EditorGUILayout.HelpBox ("Curve data is read only.", MessageType.Info);
		//serializedObject.ApplyModifiedProperties();
	}
}
