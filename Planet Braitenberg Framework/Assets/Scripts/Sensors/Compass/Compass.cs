using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CompassHeading{
	North,
	NorthEast,
	East,
	SouthEast,
	South,
	SouthWest,
	West,
	NorthWest,
	Unknown
};

[RequireComponent(typeof(Vehicle)), DisallowMultipleComponent]
public class Compass : MonoBehaviour {

	[Range(0, 5)]
	public int tolerance = 2;

	private Vehicle vehicle;

	void Awake () {
		vehicle = GetComponent<Vehicle> ();
	}


	public float GetHeading()
	{
		return this.vehicle.heading;
	}

	public int GetHeadingAsInt()
	{
		return Mathf.RoundToInt (this.GetHeading ());
	}

	public CompassHeading GetCompassHeading()
	{
		float value = this.GetHeading ();
		int[] values = new int[]{ 0, 45, 90, 135, 180, 225, 270, 315, 360};
		foreach (int v in values) {
			if (IsInRange (v, value)) {
				return ReturnCompassHeadingFromAngle (v);
			}
		}
		return CompassHeading.Unknown;
	}

	private CompassHeading ReturnCompassHeadingFromAngle(int angle)
	{
		switch (angle) {
		case 0 | 360:
			return CompassHeading.North;
		case 45:
			return CompassHeading.NorthEast;
		case 90:
			return CompassHeading.East;
		case 135:
			return CompassHeading.SouthEast;
		case 180:
			return CompassHeading.South;
		case 225:
			return CompassHeading.SouthWest;
		case 270:
			return CompassHeading.West;
		case 315:
			return CompassHeading.NorthWest;
		default:
			return CompassHeading.Unknown;
		}
	}

	private bool IsInRange(int comparator, float value)
	{
		int min = comparator - tolerance;
		int max = comparator + tolerance;
		return (value >= min) & (value <= max);
	}


}
