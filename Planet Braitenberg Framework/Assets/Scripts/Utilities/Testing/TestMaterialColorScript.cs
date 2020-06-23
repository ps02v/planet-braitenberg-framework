using UnityEngine;
using System.Collections;
using UnityEditor;

public class TestMaterialColorScript : MonoBehaviour {

	private Renderer _renderer;

	[Tooltip("The red component of the main material's color."), HideInInspector()]
	public float redComponent;
	[Tooltip("The green component of the main material's color."), HideInInspector()]
	public float greenComponent;
	[Tooltip("The blue component of the main material's color."), HideInInspector()]
	public float blueComponent;
	[Tooltip("The grayscale component of the main material's color."), HideInInspector()]
	public float grayComponent;

	void Awake () {
		_renderer = this.GetComponent<Renderer> ();
	}

	void Update () {
		this.redComponent = this._renderer.material.color.r;
		this.blueComponent = this._renderer.material.color.b;
		this.greenComponent = this._renderer.material.color.g;
		this.grayComponent = this._renderer.material.color.grayscale;
	}
}

[CustomEditor(typeof(TestMaterialColorScript))]
public class TestMaterialScriptEditor : Editor
{
	public override void OnInspectorGUI()
	{
		TestMaterialColorScript targetScript = (TestMaterialColorScript)target;
		EditorGUILayout.LabelField("Red Component", targetScript.redComponent.ToString ());
		EditorGUILayout.LabelField("Green Component", targetScript.greenComponent.ToString ());
		EditorGUILayout.LabelField("Blue Component", targetScript.blueComponent.ToString ());
		EditorGUILayout.LabelField("Gray Component", targetScript.grayComponent.ToString ());
	}
}