using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PauseSimulation : MonoBehaviour {

	public float pauseTime = 20.0f;

	void Update () {
		if (Time.time >= this.pauseTime) {
			EditorApplication.isPaused = true;
		}
	}
}
