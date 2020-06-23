using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchOffLight : MonoBehaviour {

	[Tooltip("The velocity that is required to switch off the light.")]
	public float velocityThreshold = 3.0f; // the speed of the vehicle that is required to switch off the light
	[Tooltip("The material used to render the light in its off state.")]
	public Material lightOffMaterial;

	private Light _light;
	private Renderer _renderer;

	void Start () {
		this._light = GetComponent<Light> ();
		this._renderer = GetComponent<Renderer> ();
	}

	void OnCollisionEnter(Collision collision)
	{
		this.ProcessCollision (collision);
	}

	void ProcessCollision(Collision collision)
	{
		if (collision.relativeVelocity.magnitude >= this.velocityThreshold) {
			this._light.enabled = false;
			this._renderer.material = this.lightOffMaterial;
		}
	}

}
