using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScaledCurve : MonoBehaviour {

	public ScaledCurve FirstScaledCurve = new ScaledCurve (true, Color.red, 1, 1, 1, 1, "visual");

	public ScaledCurve SecondScaledCurve;

	public ScaledCurve ThirdScaledCurve = new ScaledCurve(false, Color.yellow, 1, 1, 1, 1, "motor");

}
