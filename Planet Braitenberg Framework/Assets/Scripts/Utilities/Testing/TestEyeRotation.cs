using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TestEyeRotation : MonoBehaviour {

	public Vector3 rotation;
	public Eye eye;

}

[CustomEditor(typeof(TestEyeRotation))]
public class EyeRotationEditor : Editor
{
	public override void OnInspectorGUI()
	{
		TestEyeRotation targetScript = (TestEyeRotation)target;
		DrawDefaultInspector();
		if (GUILayout.Button ("Rotate")) {
			targetScript.eye.SetEyeRotation (targetScript.rotation);
		}
		if (GUILayout.Button ("Up")) {
			targetScript.eye.RotateEyeUp (20);
		}
		if (GUILayout.Button ("Down")) {
			targetScript.eye.RotateEyeDown (20);
		}
		if (GUILayout.Button ("Left")) {
			targetScript.eye.RotateEyeLeft (20);
		}
		if (GUILayout.Button ("Right")) {
			targetScript.eye.RotateEyeRight (20);
		}

	}
}
