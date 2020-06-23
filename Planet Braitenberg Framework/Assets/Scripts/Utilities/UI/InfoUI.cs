using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoUI : MonoBehaviour {

	[Tooltip("The vehicle whose information will be displayed in the UI.")]
	public Vehicle targetVehicle;

	[Tooltip("The visibility of the heading UI element.")]
	public bool headingVisible = true;

	[Tooltip("The visibility of the motor torque UI element.")]
	public bool motorTorqueVisible = true;

	[Tooltip("The visibility of the reversing UI element.")]
	public bool reversingVisible = true;

	[Tooltip("The visibility of the speed UI element.")]
	public bool speedVisible = true;

	[Tooltip("The visibility of the front steer angle UI element.")]
	public bool frontSteerVisible = true;

	[Tooltip("The visibility of the rear steer angle UI element.")]
	public bool rearSteerVisible = true;

	[Tooltip("The visibility of the drag factor UI element.")]
	public bool dragFactorVisible = true;

	[Tooltip("The visibility of the left eye output UI element.")]
	public bool leftEyeOutputVisible = true;

	[Tooltip("The visibility of the right eye output UI element.")]
	public bool rightEyeOutputVisible = true;

	[Tooltip("The visibility of the distance travelled UI element.")]
	public bool distanceTravelledVisible = true;

	[Tooltip("The visibility of the time UI element.")]
	public bool timeVisible = true;

	private VehicleBehaviourBase vehicleBehaviour;
	private InfoUIElement headingUI;
	private InfoUIElement motorTorqueUI;
	private InfoUIElement reversingUI;
	private InfoUIElement speedUI;
	private InfoUIElement frontSteerUI;
	private InfoUIElement rearSteerUI;
	private InfoUIElement dragFactorUI;
	private InfoUIElement leftEyeOutputUI;
	private InfoUIElement rightEyeOutputUI;
	private InfoUIElement distanceUI;
	private GameObject timeUI;


	void Awake()
	{
		//get a reference to the primary vehicle in the scene
		if (this.targetVehicle == null) {
			this.targetVehicle = GameObject.FindGameObjectWithTag (TagManager.Vehicle).GetComponent<Vehicle> ();
		}
		this.vehicleBehaviour = this.targetVehicle.GetComponent<VehicleBehaviourBase> ();
		//get a list of all the child objects of this game object
		List<GameObject> li = new List<GameObject> ();
		this.gameObject.GetChildObjects (li);
		//get a reference to UI elements of 
		foreach (GameObject go in li) {
			switch (go.name) {
			case "Heading":
				this.headingUI = go.GetComponent<InfoUIElement>();
				break;
			case "MotorTorque":
				this.motorTorqueUI = go.GetComponent<InfoUIElement>();
				break;
			case "Reversing":
				this.reversingUI = go.GetComponent<InfoUIElement>();
				break;
			case "Speed":
				this.speedUI = go.GetComponent<InfoUIElement>();
				break;
			case "FrontSteer":
				this.frontSteerUI = go.GetComponent<InfoUIElement>();
				break;
			case "RearSteer":
				this.rearSteerUI = go.GetComponent<InfoUIElement>();
				break;
			case "DragFactor":
				this.dragFactorUI = go.GetComponent<InfoUIElement>();
				break;
			case "LeftEye":
				this.leftEyeOutputUI = go.GetComponent<InfoUIElement>();
				break;
			case "RightEye":
				this.rightEyeOutputUI = go.GetComponent<InfoUIElement>();
				break;
			case "Distance":
				this.distanceUI = go.GetComponent<InfoUIElement>();
				break;
			case "Time":
				this.timeUI = go;
				break;
			}
		}
	}

	void Start()
	{
		this.ProcessUIElementVisibility ();
	}

	void ProcessUIElementVisibility()
	{
		headingUI.gameObject.SetActive (this.headingVisible);
		motorTorqueUI.gameObject.SetActive (this.motorTorqueVisible);
		reversingUI.gameObject.SetActive (this.reversingVisible);
		speedUI.gameObject.SetActive (this.speedVisible);
		frontSteerUI.gameObject.SetActive (this.frontSteerVisible);
		rearSteerUI.gameObject.SetActive (this.rearSteerVisible);
		dragFactorUI.gameObject.SetActive (this.dragFactorVisible);
		leftEyeOutputUI.gameObject.SetActive (this.leftEyeOutputVisible);
		rightEyeOutputUI.gameObject.SetActive (this.rightEyeOutputVisible);
		distanceUI.gameObject.SetActive (this.distanceTravelledVisible);
		timeUI.SetActive (this.timeVisible);
	}

	void ToggleVisibilityOfAllUIElements(bool toggle)
	{
		headingUI.gameObject.SetActive (toggle);
		motorTorqueUI.gameObject.SetActive (toggle);
		reversingUI.gameObject.SetActive (toggle);
		speedUI.gameObject.SetActive (toggle);
		frontSteerUI.gameObject.SetActive (toggle);
		rearSteerUI.gameObject.SetActive (toggle);
		dragFactorUI.gameObject.SetActive (toggle);
		leftEyeOutputUI.gameObject.SetActive (toggle);
		rightEyeOutputUI.gameObject.SetActive (toggle);
		distanceUI.gameObject.SetActive (toggle);
		timeUI.SetActive (toggle);
	}
		
	void FixedUpdate () {
		//update to UI element display text
		this.headingUI.value = this.targetVehicle.heading;
		this.motorTorqueUI.value = this.targetVehicle.motorTorque;
		this.reversingUI.value = System.Convert.ToInt32 (this.targetVehicle.invertMotorTorque) - 1;
		this.speedUI.value = this.targetVehicle.speed;
		this.frontSteerUI.value = this.targetVehicle.frontSteerAngle;
		this.rearSteerUI.value = this.targetVehicle.rearSteerAngle;
		this.dragFactorUI.value = this.targetVehicle.dragFactor;
		//only get left eye information if this is a binocular vehicle; i.e., a vehicle with two eyes
		if (typeof(BinocularVehicleBehaviourBase).IsInstanceOfType(this.vehicleBehaviour)) {
			this.leftEyeOutputUI.value = ((BinocularVehicleBehaviourBase)this.vehicleBehaviour).leftEyeOutput;
			this.rightEyeOutputUI.value = ((BinocularVehicleBehaviourBase)this.vehicleBehaviour).rightEyeOutput;
		}
		this.distanceUI.value = this.targetVehicle.distanceTravelled;
		//time has its own update method, so no need to update here
	}
}
