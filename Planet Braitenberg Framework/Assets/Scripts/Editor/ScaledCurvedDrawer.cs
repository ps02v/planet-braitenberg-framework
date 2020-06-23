using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer (typeof(ScaledCurve))]
public class ScaledCurvedDrawer : PropertyDrawer {



	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		return MyRoutines.GetGUIPropertyHeight (2);
	}

	public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
	{
		SerializedProperty showScale = prop.FindPropertyRelative ("showScale");
		SerializedProperty curveColor = prop.FindPropertyRelative ("curveColor");
		SerializedProperty yScale = prop.FindPropertyRelative ("yScale");
		SerializedProperty xScale = prop.FindPropertyRelative ("xScale");
		SerializedProperty yMaxScale = prop.FindPropertyRelative ("yMaxScale");
		SerializedProperty xMaxScale = prop.FindPropertyRelative ("xMaxScale");
		SerializedProperty curve = prop.FindPropertyRelative ("curve");
		SerializedProperty curveType = prop.FindPropertyRelative ("curveType");

		float padding = MyRoutines.GetEditorGUIControlSpacing();
		float lineHeight = MyRoutines.GetEditorGUIStandardLineHeight ();
		float yPos = pos.y;
		float curveWidth = pos.width;



		string tooltip = MyRoutines.GetTooltip (prop.serializedObject.targetObject.GetType(), prop.name);
		GUIContent gContent = new GUIContent (label.text, tooltip); //prop.tooltip doesn't work

		//draw the slider if required
		if (showScale.boolValue) {
			curveWidth = 75;
			EditorGUI.IntSlider (new Rect (pos.x, yPos, pos.width - curveWidth, lineHeight), yScale, 0, yMaxScale.intValue, gContent);
		} 

		//draw curve
		if (showScale.boolValue) {
			int indent = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;
			EditorGUI.CurveField (new Rect (pos.x + (pos.width - curveWidth) + padding, yPos, curveWidth - padding, lineHeight), curve, curveColor.colorValue, new Rect (0, 0, xScale.intValue, yScale.intValue), GUIContent.none);
			EditorGUI.indentLevel = indent;
		} else {
			EditorGUI.CurveField (new Rect (pos.x, yPos, curveWidth - padding, lineHeight), curve, curveColor.colorValue, new Rect (0, 0, xMaxScale.intValue, yScale.intValue), gContent);
		}


		//buttons
		yPos = MyRoutines.GetEditorGUINextControlYPos(yPos);

		float buttonWidth = (pos.width / 3) - padding;
		GUIContent buttonGUIContent = new GUIContent ("Reset", "Reset the curve to its default setting.");
		if (GUI.Button (new Rect (pos.x, yPos, buttonWidth, lineHeight), buttonGUIContent)) {
			//reset the animation curve
			//get a reference to the curve
			ScaledCurve c = (ScaledCurve)fieldInfo.GetValue(prop.serializedObject.targetObject);
			//reset the curve
			curve.animationCurveValue = c.ResetCurve ();
		}
		buttonGUIContent = new GUIContent ("Save", "Saves curve data to an asset file.");
		if (GUI.Button(new Rect(pos.x + buttonWidth + padding, yPos, buttonWidth, lineHeight), buttonGUIContent))
		{
			string path = EditorUtility.SaveFilePanelInProject ("Save Curve", "CurveData", "asset", "Please enter a file name to save the curve data to.");
			if (string.IsNullOrEmpty (path) == false) {
				//save curve data as scriptable object instance
				SerializedCurve asset = ScriptableObject.CreateInstance<SerializedCurve> ();
				asset.curve = new SerializableCurve (curve.animationCurveValue);
				asset.curveType = curveType.stringValue;
				AssetDatabase.CreateAsset (asset, path);
				AssetDatabase.SaveAssets ();
				AssetDatabase.Refresh ();
			}
		}
		buttonGUIContent = new GUIContent ("Load", "Loads curve data from an asset file.");
		if (GUI.Button(new Rect(pos.x + ((buttonWidth + padding) * 2), yPos, buttonWidth, lineHeight), buttonGUIContent))
		{
			string path = EditorUtility.OpenFilePanel ("Load Curve", Application.dataPath, "asset");
			if (string.IsNullOrEmpty (path) == false) {
				//load curve data from scriptable object
				path = path.Replace(Application.dataPath, "Assets");
				SerializedCurve asset = AssetDatabase.LoadAssetAtPath<SerializedCurve>(path);
				if (asset.curveType == curveType.stringValue) {
					//correct type - replace curve with serialized curve data
					curve.animationCurveValue = asset.curve.ToAnimationCurve();
				}
				else
				{
					EditorUtility.DisplayDialog ("Error", "Saved curve data is of the wrong type.", "OK");
				}
			}
		}

	}
}
