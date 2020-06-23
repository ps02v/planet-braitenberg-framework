using UnityEngine;
using System.Collections;

public enum ViewType
{
	Orbit,
	FirstPerson,
	TopDown,
	Vehicle
};

public class Controller : MonoBehaviour {



	[Tooltip("The target vehicle in the scene.")]
	public Vehicle primaryVehicle;
	[Tooltip("Specifies whether the vehicle's eye camera's are displayed.")]
	public bool showEyeCameras = true;
	[Tooltip("Specifies whether the vehicle's trail is visible.")]
	public bool showTrail = true;
	[Tooltip("Specifies whether the vehicle's eye cameras will show processed information.")]
	public bool showProcessedInformation = false;
	[Tooltip("The type of view to display on startup.")]
	public ViewType cameraView = ViewType.Orbit;
	[Tooltip("Specifies whether the Instruction UI will be displayed on startup.")]
	public bool showInstructionUI = true;
	[Tooltip("Specifies whether the Information UI will be displayed on startup.")]
	public bool showInfoUI = true;

	internal GameObject orbitCameraRig; //The orbit camera rig in the scene.
	internal GameObject firstPersonController; //The first person controller in the scene

	internal GameObject instructionScreen; //The Instruction UI canvas.
	internal GameObject infoUI; //The Info UI canvas.
	internal GameObject eyeViewUI; //The Eye View UI canvas.
	internal Camera activeCamera; //the active camera
	internal Camera leftEyeCamera; //the camera for the left eye
	internal Camera rightEyeCamera; //the camera for the right eye
	internal Retina leftEyeRetina; //the script for the left eye
	internal Retina rightEyeRetina; //the script for the right eye
	internal TrailRenderer[] trails; //the trails for the target vehicle

	internal Camera orbCamera; //the orbit camera
	internal Camera firstPersonCamera; //the first person camera
	internal Camera vehicleCamera; //The primary vehicle's onboard camera.
	internal Camera topDownCamera; //The top down camera for the scene.

	internal GameObject vehicleGameObject;

	void Awake()
	{
		this.orbitCameraRig = GameObject.FindGameObjectWithTag (TagManager.OrbitCamera);
		this.firstPersonController = GameObject.FindGameObjectWithTag (TagManager.FirstPersonCamera);
		this.topDownCamera = GameObject.FindGameObjectWithTag (TagManager.TopDownCamera).GetComponent<Camera> ();
		this.instructionScreen = GameObject.FindGameObjectWithTag (TagManager.InstrustionUI);
		this.infoUI = GameObject.FindGameObjectWithTag (TagManager.InfoUI);
		this.eyeViewUI = GameObject.FindGameObjectWithTag (TagManager.EyeUI);
		orbCamera = this.orbitCameraRig.GetComponentInChildren<Camera> ();
		firstPersonCamera = this.firstPersonController.GetComponentInChildren<Camera> ();
		if (this.primaryVehicle == null) {
			//find the first vehicle in the scene
			this.vehicleGameObject = GameObject.FindGameObjectWithTag (TagManager.Vehicle);
			//set the vehicle as the primary vehicle
			this.primaryVehicle = this.vehicleGameObject.GetComponent<Vehicle> ();
		} else {
			this.vehicleGameObject = this.primaryVehicle.gameObject;
		}
		//ensure that the primary vehicle is tagged as Vehicle
		this.vehicleGameObject.tag = TagManager.Vehicle;
		//get a reference to the primary vehicle's camera
		this.vehicleCamera = primaryVehicle.gameObject.GetChildObjectByName("Vehicle Camera").GetComponent<Camera>();
		//ensure that only the primary vehicle is tagged with the Vehicle tag
		//all other vehicles are secondary vehicles
		//there can be only one primary vehicle in the scene
		foreach (GameObject go in GameObject.FindGameObjectsWithTag(TagManager.Vehicle)) {
			if ((go != this.vehicleGameObject) && (go.tag == TagManager.Vehicle)) {
				go.tag = TagManager.SecondaryVehicle;
			}
		}
	}

	internal virtual void Start()
	{
		//set the initial view
		this.InitializeViews();
		//get a reference to the primary vehicle's eye cameras
		leftEyeCamera = primaryVehicle.gameObject.GetChildObjectByName("LeftEyeCamera").GetComponent<Camera>();
		rightEyeCamera = primaryVehicle.gameObject.GetChildObjectByName("RightEyeCamera").GetComponent<Camera>();
		//get a reference to the vehicle's retinas
		leftEyeRetina = leftEyeCamera.GetComponent<Retina>();
		rightEyeRetina = rightEyeCamera.GetComponent<Retina>();
		trails = GameObject.FindObjectsOfType<TrailRenderer>();
		foreach (TrailRenderer trail in trails)
		{
			//trail.enabled = true;
			trail.enabled = this.showTrail;
		}
		//determine whether the eye view UI is rendered
		this.eyeViewUI.SetActive (this.showEyeCameras);
		//determine whether the eye view UI will show the average eye color
		leftEyeRetina.showUniform = this.showProcessedInformation;
		rightEyeRetina.showUniform = this.showProcessedInformation;
		//determine whether the instruction screen is visible
		instructionScreen.SetActive (this.showInstructionUI);
		//determine whether the info UI is visible. The info UI has priority over the instruction screen.
		if (this.showInfoUI == true) {
			instructionScreen.SetActive (false);
			this.infoUI.SetActive (true);
			this.showInstructionUI = false;
		} else {
			this.infoUI.SetActive (false);
		}
	}

	private void InitializeViews()
	{
		this.orbitCameraRig.SetActive (true);
		this.firstPersonController.SetActive (false);
		orbCamera.enabled = false;
		vehicleCamera.enabled = false;
		topDownCamera.enabled = false;
		firstPersonCamera.enabled = false;
		//set the active camera
		switch (this.cameraView) 
		{
		case ViewType.Orbit:
			activeCamera = orbCamera;
			//this.orbitCameraRig.SetActive (true);
			break;
		case ViewType.FirstPerson:
			activeCamera = firstPersonCamera;
			this.firstPersonController.SetActive (true);
			break;
		case ViewType.TopDown:
			activeCamera = topDownCamera;
			break;
		case ViewType.Vehicle:
			activeCamera = vehicleCamera;
			break;
		default:
			activeCamera = orbCamera;
			break;
		}
		activeCamera.enabled = true;
	}
		
	internal virtual void Update()
	{
		//process requests to change the view
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			this.cameraView = ViewType.Orbit;
		}
		if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			this.cameraView = ViewType.Vehicle;
		}
		if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			this.cameraView = ViewType.TopDown;
		}
		if (Input.GetKeyDown(KeyCode.Alpha4))
		{
			this.cameraView = ViewType.FirstPerson;
		}
		//set the active view
		switch (this.cameraView) {
		case ViewType.Orbit:
			activeCamera = orbCamera;
			break;
		case ViewType.TopDown:
			activeCamera = topDownCamera;
			break;
		case ViewType.Vehicle:
			activeCamera = vehicleCamera;
			break;
		default: 
			activeCamera = firstPersonCamera;
			break;
		}
		//disable inactive cameras
		if (activeCamera != this.orbCamera)
		{
			this.orbCamera.enabled = false;
		}
		if (activeCamera != this.firstPersonCamera)
		{
			this.firstPersonCamera.enabled = false;
		}
		if (activeCamera != this.vehicleCamera)
		{
			this.vehicleCamera.enabled = false;
		}
		if (activeCamera != this.topDownCamera)
		{
			this.topDownCamera.enabled = false;
		}
		//ensure the active camera is enabled
		activeCamera.enabled = true;

		//enable or disable the first person controller and orbit camera rigs based on the active camera
		//NO, DON'T DO THIS BECAUSE IT PREVENTS THE ORBIT CAMERA POSITION FROM UPDATING AS THE VEHICLE MOVES
		//IT IS ENOUGH JUST TO DISABLE THE CAMERA
//		this.orbitCameraRig.SetActive (activeCamera == orbCamera);
		this.firstPersonController.SetActive (activeCamera == firstPersonCamera);

		//toggle the display of eye cameras
		if (Input.GetKeyDown (KeyCode.V)) {
			this.showEyeCameras = !this.showEyeCameras;
		}
		this.eyeViewUI.SetActive (this.showEyeCameras);
		this.leftEyeCamera.enabled = this.showEyeCameras;
		this.rightEyeCamera.enabled = this.showEyeCameras;
		//toggle the display of the trail renderer
		if (Input.GetKeyDown(KeyCode.T))
		{
			this.showTrail = !this.showTrail;
		}
		if (this.showTrail)
		{
			foreach (TrailRenderer trail in trails)
			{
				trail.enabled = true;
			}
		}
		else
		{
			foreach (TrailRenderer trail in trails)
			{
				trail.enabled = false;
			}
		}
		//toggle whether eye cameras show average brightness
		if (Input.GetKeyDown(KeyCode.U))
		{
			this.showProcessedInformation = !this.showProcessedInformation;
		}
		leftEyeRetina.showUniform = this.showProcessedInformation;
		rightEyeRetina.showUniform = this.showProcessedInformation;
		//toggle the display of the Instruction UI
		if (Input.GetKeyDown(KeyCode.H))
		{
			this.showInfoUI = false;
			this.showInstructionUI = !this.showInstructionUI;
		}
		this.instructionScreen.SetActive (this.showInstructionUI);
		if (Input.GetKeyDown(KeyCode.I))
		{
			this.showInstructionUI = false;
			this.showInfoUI = !this.showInfoUI;
		}
		this.infoUI.SetActive (this.showInfoUI);
		if (showInfoUI)
			this.showInstructionUI = false;
	}
}

