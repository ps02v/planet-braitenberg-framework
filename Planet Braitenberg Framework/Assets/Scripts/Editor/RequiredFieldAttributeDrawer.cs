using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer (typeof(RequiredFieldAttribute))]
public class RequiredFieldAttributeDrawer : PropertyDrawer {

	public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
	{
		if (prop.objectReferenceValue == null) {
			Color c = GUI.backgroundColor;
			GUI.backgroundColor = new Color (1f, 0.5f, 0.5f);
			EditorGUI.PropertyField (position, prop);
			GUI.backgroundColor = c;
		} else {
			EditorGUI.PropertyField (position, prop);
		}

	}
}
