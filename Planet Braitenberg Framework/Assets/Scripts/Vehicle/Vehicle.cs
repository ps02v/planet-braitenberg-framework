using UnityEngine;
using System.Collections;
using UnityEditor;

[DisallowMultipleComponent, RequireComponent(typeof(Rigidbody))]
//public class Vehicle : MonoBehaviour, ISerializationCallbackReceiver {
public class Vehicle : MonoBehaviour {

	[Tooltip("The vehicle's center of mass.")]
	public Vector3 centerOfMass = new Vector3 (0f, -1f, -0.25f);
	[Tooltip("The interval between successive invocations of the vehicle's behaviour module."), Range(0.1f, 10f)]
	public float behaviourUpdateInterval = 2f;
	[Tooltip("Disables the vehicle's motor.")]
	public bool disableMotor = false;
	[Tooltip("The speed at which the vehicle will report it's status as stationary."), Range(0f, 1f)]
	public float stationaryThreshold = 0.1f;
	[Tooltip("The number of RPMs at which the wheels will no longer be deemed to be rotating."), Range(0f, 5f)]
	public float wheelColliderStopThreshold = 0.1f;
	[Tooltip("Implements an autostop capability. Applies brake torque whenever motor torque is zero.")]
	public bool implementAutoStop = false;
	[Tooltip("The amount of brake torque to apply when autostop is enabled.")]
	public float autoStopBrakeTorque = 50f;
	[Range(1f, 40.0f), Tooltip("The rate at which the vehicle's wheels will rotate to the target steering angle (in degrees per second).")]
	public float steerSpeed = 10f;
	[Tooltip("Specifies that the vehicle's wheels will rotate to the target steering angle immediately.")]
	public bool instantSteer = false;

	[Tooltip("Indicates whether the vehicle spotlight with be turned on.")]
	public bool vehicleSpotlight = false;
	[Tooltip("Transfer function for vehicle retinaa.")]
	public ScaledCurve visualProcessingFunction = new ScaledCurve (false, Color.red, 1, 1, 1, 1, "visual");
	[Tooltip("Transfer function for the vehicle's motor.")]
	public ScaledCurve visuoMotorFunction = new ScaledCurve (false, Color.green, 300, 1, 300, 1, "motor");
	[Tooltip("Transfer function for the vehicle's steering.")]
	public ScaledCurve rotationFunction = new ScaledCurve (false, Color.yellow, 20, 1, 20, 1, "rotation");
	[Tooltip("Transfer function for the vehicle's drag.")]
	public ScaledCurve vehicleDragFunction = new ScaledCurve (false, Color.blue, 10, 300, 10, 300, "drag");
	[Tooltip("Places the vehicle in test mode. If activated, the vehicle will ignore input from the behaviour module.")]
	public bool testMode = false;

	[HideInInspector(), Tooltip("The motor torque to be applied to the vehicle.")]
	public float motorTorque;
	[HideInInspector(), Tooltip("The brake torque to be applied to the vehicle.")]
	public float brakeTorque = 0f;
	[HideInInspector(), Tooltip("The steering angle of the vehicle's front wheels.")]
	public float frontSteerAngle;
	[HideInInspector(), Tooltip("The steering angle of the vehicle's rear wheels.")]
	public float rearSteerAngle;
	[HideInInspector(), Tooltip("Specifies whether the vehicle's motor torque is inverted.")]
	public bool invertMotorTorque = false;

	internal float rotationDelta;
	internal float dragFactor;
	internal float speed;
	internal float heading;
	internal float distanceTravelled;
	internal Vector3 startPosition;
	internal Transform[] wheels; //the transform of the object that has the wheel collider attached
	internal WheelCollider[] colls; //the actual collider components of the wheel

	private Transform[] wGraphics; //the transform components of the tire models
	private VehicleBehaviourBase vehicleBehaviour; //the behaviour component for the vehicle
	private Rigidbody rigidBody;
	private Vector3 lastPosition; //the last known position of the vehicle
	private bool previousInvertMotorTorque; //the value of inverMotorTorque from the previous update

	void Awake () {
		//get the wheel collider transforms
		wheels = new Transform[4];
		wheels[0] = this.gameObject.GetChildObjectByName("FL_Collider").transform;
		wheels[1] = this.gameObject.GetChildObjectByName("FR_Collider").transform;
		wheels[2] = this.gameObject.GetChildObjectByName("BL_Collider").transform;
		wheels[3] = this.gameObject.GetChildObjectByName("BR_Collider").transform;
		//get the wheel model transforms
		wGraphics = new Transform[4];
		wGraphics[0] = this.gameObject.GetChildObjectByName("FL_Tire").transform;
		wGraphics[1] = this.gameObject.GetChildObjectByName("FR_Tire").transform;
		wGraphics[2] = this.gameObject.GetChildObjectByName("BL_Tire").transform;
		wGraphics[3] = this.gameObject.GetChildObjectByName("BR_Tire").transform;
		//get the wheel colliders
		colls = new WheelCollider[4];
		for (int i = 0; i < 4; i++) {
			colls[i] = wheels[i].GetComponent<WheelCollider>();
		}
		//get a reference to the rigibody component component
		this.rigidBody = this.GetComponent<Rigidbody>();
		//set the center of mass of the vehicle
		this.rigidBody.centerOfMass = this.centerOfMass;
		//get a reference to the vehicle behaviour
		this.vehicleBehaviour = GetComponent<VehicleBehaviourBase>();
		//set the last known position of the vehicle
		this.lastPosition = this.transform.position;
		//set the start position of the vehicle
		this.startPosition = this.transform.position;
		//get a reference to the vehicle spotlight
		GameObject spotLight = this.gameObject.GetChildObjectByName ("Vehicle Spotlight");
		//toggle the visibility of the vehicle spotlight
		if (this.vehicleSpotlight) {
			spotLight.GetComponent<Light> ().enabled = true;
		} else {
			spotLight.GetComponent<Light> ().enabled = false;
		}
	}

	void Start()
	{
		//execute the vehicle behaviour model with the specified behaviour update frequency
		InvokeRepeating ("ExecuteBehaviour", 0.0f, this.behaviourUpdateInterval);
	}

	void ComputeHeading()
	{
		//update the vehicle's heading
		Vector3 v;
		if (invertMotorTorque) {
			v = this.transform.forward * -1;
		} else {
			v = this.transform.forward;
		}
		this.heading = Quaternion.LookRotation(v).eulerAngles.y;
	}

	void ComputeSpeed()
	{
		//update the vehicle's speed
		this.speed = (transform.position - lastPosition).magnitude / Time.fixedDeltaTime;
	}

	internal float GetSpeed()
	{
		//returns the vehicle's current speed
		return this.speed;
	}

	void ExecuteBehaviour()
	{
		//execute the vehicle's behaviour component
		this.vehicleBehaviour.Execute();
	}

	internal bool IsStopped(bool applyToFrontWheels)
	{
		//indicates whether the relevant wheel colliders have stopped moving
		//if applyToFrontWheels is false, then we determine whether the back wheel colliders are rotating
		//else we determine whether the front wheel colliders are rotating
		return this.GetWheelInfo ().IsStopped (applyToFrontWheels);
	}

	internal bool IsStopped()
	{
		return IsStopped (this.invertMotorTorque);
	}

	internal bool IsStationary()
	{
		//indicates whether the vehicle is stationary
		float s = this.GetSpeed();
		s = Mathf.Abs (s);
		if (s <= this.stationaryThreshold)
			return true;
		else
			return false;
	}

	void FixedUpdate () {
		if (testMode == false) {
			//get locomotion information from the vehicle behaviour component
			motorTorque = vehicleBehaviour.motorTorque;
			rotationDelta = vehicleBehaviour.rotationDelta;
			frontSteerAngle = vehicleBehaviour.frontSteerAngle;
			rearSteerAngle = vehicleBehaviour.rearSteerAngle;
			invertMotorTorque = vehicleBehaviour.invertMotorTorque;
			brakeTorque = vehicleBehaviour.brakeTorque;
		}
		if (this.implementAutoStop && this.IsStationary () == false && motorTorque == 0f) {
			//autostop functionality is enabled, but the vehicle is moving
			brakeTorque = this.autoStopBrakeTorque;
		} else {
			brakeTorque = vehicleBehaviour.brakeTorque;
		}

		bool continueBraking = false;
		if (this.previousInvertMotorTorque != this.invertMotorTorque)
		{
			//the vehicle has changed direction
			if (this.implementAutoStop && this.IsStationary () == false) {
				//we want to implement the autostop capability to stop the vehicle skidding
				//if the vehicle was moving forward then previousInvertMotorTorque will be false and invertMotorTorque will be true
				//the motortorque will be applied to back wheels based on previousMotorTorque, so that is where the brake torque needs to be applied
				motorTorque = 0f;
				brakeTorque = this.autoStopBrakeTorque;
				continueBraking = true;
			} else {
				brakeTorque = vehicleBehaviour.brakeTorque;
			}
		}

		//apply any drag to the vehicle
		dragFactor = this.EvaluateDragFactor (Mathf.Abs (this.motorTorque));
		this.rigidBody.drag = this.dragFactor;

		if (invertMotorTorque && (motorTorque > 0)) {
			//if the vehicle is reversing and motor torque is positive then negate the motor torque
			motorTorque *= -1;
		}
		//if the motor is disabled then set motor torque to zero
		if (disableMotor) {
			motorTorque = 0;
		} 
		//turn the wheels
		if (invertMotorTorque)
		{
			//if the vehicle is reversing apply the motor torque to the front wheels
			colls[0].motorTorque = motorTorque;
			colls[0].brakeTorque = brakeTorque;
			colls[1].motorTorque = motorTorque;
			colls[1].brakeTorque = brakeTorque;
			colls[2].motorTorque = 0;
			colls[2].brakeTorque = 0;
			colls[3].motorTorque = 0;
			colls[3].brakeTorque = 0;

		}
		else{
			//else apply the motor torque to the back wheels
			colls[0].motorTorque = 0;
			colls[0].brakeTorque = 0;
			colls[1].motorTorque = 0;
			colls[1].brakeTorque = 0;
			colls[2].motorTorque = motorTorque;
			colls[2].brakeTorque = brakeTorque;
			colls[3].motorTorque = motorTorque;
			colls[3].brakeTorque = brakeTorque;
		}
		if (continueBraking == false)
			this.previousInvertMotorTorque = this.invertMotorTorque; //only implemented once the vehicle has come to a stop or autostop is disabled.
		this.ChangeSteeringAngle();
		this.RotateWheels();
		this.ComputeHeading ();
		this.ComputeSpeed ();
		//record the last known position of the vehicle
		lastPosition = transform.position;
		//record the distance of the vehicle relative to its start position
		this.distanceTravelled = Vector3.Distance (this.transform.position, this.startPosition);
	}

	protected void ChangeSteeringAngle()
	{
		//adjust the steering angle of the wheels
		if (this.instantSteer) {
			colls [WheelInfo.FrontLeft].steerAngle = frontSteerAngle;
			colls [WheelInfo.FrontRight].steerAngle = frontSteerAngle;
			colls [WheelInfo.BackLeft].steerAngle = rearSteerAngle;
			colls [WheelInfo.BackRight].steerAngle = rearSteerAngle;
			return;
		}

		float frontTurnSpeed = 0f;
		float rearTurnSpeed = 0f;
		float diffFrontSteerAngle = 0f;
		float diffRearSteerAngle = 0f;

		if (frontSteerAngle > colls [WheelInfo.FrontLeft].steerAngle)
			diffFrontSteerAngle = frontSteerAngle - colls [WheelInfo.FrontLeft].steerAngle;
		else
			diffFrontSteerAngle = colls [WheelInfo.FrontLeft].steerAngle - frontSteerAngle;
		if (rearSteerAngle > colls [WheelInfo.BackLeft].steerAngle)
			diffRearSteerAngle = rearSteerAngle - colls [WheelInfo.BackLeft].steerAngle;
		else
			diffRearSteerAngle = colls [WheelInfo.BackLeft].steerAngle - rearSteerAngle;

		if (diffFrontSteerAngle > 0f)
			frontTurnSpeed = Mathf.Min (diffFrontSteerAngle, this.steerSpeed * Time.fixedDeltaTime);
		else if (diffFrontSteerAngle < 0f)
			frontTurnSpeed = Mathf.Min (diffFrontSteerAngle, -this.steerSpeed * Time.fixedDeltaTime);
		if (diffRearSteerAngle > 0f)
			rearTurnSpeed = Mathf.Min (diffRearSteerAngle, this.steerSpeed * Time.fixedDeltaTime);
		else if (diffRearSteerAngle < 0f)
			rearTurnSpeed = Mathf.Min (diffRearSteerAngle, -this.steerSpeed * Time.fixedDeltaTime);
		colls [WheelInfo.FrontLeft].steerAngle += (frontTurnSpeed * Mathf.Sign(frontSteerAngle - colls [WheelInfo.FrontLeft].steerAngle));
		colls [WheelInfo.FrontRight].steerAngle += (frontTurnSpeed * Mathf.Sign (frontSteerAngle - colls [WheelInfo.FrontRight].steerAngle));
		colls [WheelInfo.BackLeft].steerAngle += (rearTurnSpeed * Mathf.Sign (rearSteerAngle - colls [WheelInfo.BackLeft].steerAngle));
		colls [WheelInfo.BackRight].steerAngle += (rearTurnSpeed * Mathf.Sign (rearSteerAngle - colls [WheelInfo.BackRight].steerAngle));
	}
		
	internal WheelInfo GetWheelInfo()
	{
		//returns information about the current status of the vehicle's wheels
		return new WheelInfo (this, this.wheelColliderStopThreshold);
	}
		
	protected void RotateWheels()
	{
		//turn the wheels
		Quaternion q;
		Vector3 p;
		for (int i = 0; i < 4; i++) {
			this.colls[i].GetWorldPose(out p, out q);
			wGraphics[i].position = p;
			wGraphics[i].rotation = q;
		}
	}

	internal float EvaluateEyeBrightness(float input)
	{
		//evaluates eye output (y) relative to the input value (x)
		return this.visualProcessingFunction.curve.Evaluate(input);
	}
	
	internal float EvaluateMotorTorque(float input)
	{
		//evaluates motor torque (y) relative to the input value (x)
		return this.visuoMotorFunction.curve.Evaluate(input);
	}
	
	internal float EvaluateRotation(float input)
	{
		//evaluates steering rotation (y) relative to the input value (x)
		return this.rotationFunction.curve.Evaluate(input);
	}

	internal float EvaluateDragFactor(float input)
	{
		//evaluates drag (y) relative to the input value (x)
		return this.vehicleDragFunction.curve.Evaluate(input);
	}
	
}