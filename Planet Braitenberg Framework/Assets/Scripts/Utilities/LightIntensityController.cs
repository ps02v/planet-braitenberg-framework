using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Controller))]
public class LightIntensityController : MonoBehaviour {

	[Header("Light configuration for the top down camera.")]
	public LightConfiguration topDownCameraLightConfig;
	[Space()]
	[Header("Light configuration for the vehicle camera.")]
	public LightConfiguration vehicleCameraLightConfig;
	[Space()]
	[Header("Light configuration for the first person camera.")]
	public LightConfiguration firstPersonCameraLightConfig;
	[Space()]
	[Header("Light configuration for the oribit camera.")]
	public LightConfiguration orbitCameraLightConfig;

	private Controller controller;
	private Light directionalLight;

	void Awake()
	{
		//get a reference to the controller.
		controller = this.GetComponent<Controller> ();
		//get a reference to the main directiona light.
		this.directionalLight = GameObject.FindGameObjectWithTag (TagManager.DirectionalLight).GetComponent<Light> ();
	}

	void Update()
	{
		//change the configuration of the main directional light based on the currently active camera
		if (controller.activeCamera == controller.topDownCamera) {
			directionalLight.intensity = this.topDownCameraLightConfig.intensity;
			directionalLight.color = this.topDownCameraLightConfig.color;
			directionalLight.transform.localRotation = Quaternion.Euler(this.topDownCameraLightConfig.rotation);
		}
		else if (controller.activeCamera == controller.firstPersonCamera)
		{
			directionalLight.intensity = this.firstPersonCameraLightConfig.intensity;
			directionalLight.color = this.firstPersonCameraLightConfig.color;
			directionalLight.transform.localRotation = Quaternion.Euler(this.firstPersonCameraLightConfig.rotation);
		}
		else if (controller.activeCamera == controller.vehicleCamera)
		{
			directionalLight.intensity = this.vehicleCameraLightConfig.intensity;
			directionalLight.color = this.vehicleCameraLightConfig.color;
			directionalLight.transform.localRotation = Quaternion.Euler(this.vehicleCameraLightConfig.rotation);
		}
		else if (controller.activeCamera == controller.orbCamera)
		{
			directionalLight.intensity = this.orbitCameraLightConfig.intensity;
			directionalLight.color = this.orbitCameraLightConfig.color;
			directionalLight.transform.localRotation = Quaternion.Euler(this.orbitCameraLightConfig.rotation);
		}
	}
}
