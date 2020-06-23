using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "CurveData", menuName = "Vehicles/CurveData")]
public class SerializedCurve : ScriptableObject {

	public string curveType;
	public SerializableCurve curve;

}
