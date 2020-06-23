using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LightConfiguration {

	[Tooltip("The color of the directional light.")]
	public Color color;
	[Tooltip("The rotation of the directional light.")]
	public Vector3 rotation;
	[Tooltip("The intensity of the directional light.")]
	public float intensity;
}
