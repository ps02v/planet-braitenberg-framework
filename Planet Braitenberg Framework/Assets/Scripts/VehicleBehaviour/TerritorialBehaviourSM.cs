using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class TerritorialBehaviourSM : TerritorialBehaviour 
{
	[Tooltip("The radius of the vehicle's home base within its territory.")]
    public float homeBaseRadius = 1.0f;
	[Tooltip("The amount of torque to apply to the vehicle when it is returning to its home base.")]
	public float returnToHomeTorque = 50f;

    internal bool territoryPenetrated;
    internal bool atHome;
	internal float distanceFromHome;
	internal float objectLocation = 0f;
//	internal float frontSteerAngleSM = 0;
//	internal float rearSteerAngleSM = 0;
//	internal float motorTorqueSM = 0;
	internal float relativeBearingToHome = 0;
	internal bool isAttacking = false;

	private float previousLocation = -1;
	private Animator animator;
	private AtHomeStateMachineBehaviour atHomeSmb;
	private ReturnToHomeStateMachineBehaviour returnHomeSmb;
	private ReversingStateMachineBehaviour reversingSmb;
	private FacingHomeStateMachineBehaviour facingHomeSmb;
	private OrientingStateMachineBehaviour orientingSmb;
	private AttackingStateMachineBehaviour attackingSmb;
	private MoveForwardStateMachineBehaviour moveForwardSmb;
	private MoveBackwardStateMachineBehaviour moveBackwardSmb;
		
	internal override void Start()
	{
		base.Start();
		animator = GetComponent<Animator> ();
		//set at home references
		atHomeSmb = animator.GetBehaviour<AtHomeStateMachineBehaviour> ();
		atHomeSmb.territorialBehaviour = this;
		atHomeSmb.territory = this.territory;
		//set return to home references
		returnHomeSmb = animator.GetBehaviour<ReturnToHomeStateMachineBehaviour> ();
		returnHomeSmb.territorialBehaviour = this;
		//set reversing references
		reversingSmb = animator.GetBehaviour<ReversingStateMachineBehaviour> ();
		reversingSmb.territorialBehaviour = this;
		//set facing home references
		facingHomeSmb = animator.GetBehaviour<FacingHomeStateMachineBehaviour> ();
		facingHomeSmb.territorialBehaviour = this;
		//set orienting references
		orientingSmb = animator.GetBehaviour<OrientingStateMachineBehaviour> ();
		orientingSmb.territorialBehaviour = this;
		//set attacking references
		attackingSmb = animator.GetBehaviour<AttackingStateMachineBehaviour> ();
		attackingSmb.territorialBehaviour = this;
		//set move forward references
		moveForwardSmb = animator.GetBehaviour<MoveForwardStateMachineBehaviour> ();
		moveForwardSmb.territorialBehaviour = this;
		//set move backward references
		moveBackwardSmb = animator.GetBehaviour<MoveBackwardStateMachineBehaviour> ();
		moveBackwardSmb.territorialBehaviour = this;
	}
		
	internal override void Update()
	{
		base.Update();
        this.territoryPenetrated = this.territory.penetrated;
        this.distanceFromHome = DistanceFromHome();
        this.relativeBearingToHome = this.GetRelativeBearingToHome();
        this.atHome = this.AtHome();
		//update animator parameters
		animator.SetBool(TerritorialSMUtility.ParameterNames.TerritoryPenetrated, this.territoryPenetrated);
		animator.SetBool(TerritorialSMUtility.ParameterNames.AtHome, this.atHome);
		animator.SetFloat(TerritorialSMUtility.ParameterNames.DistanceFromHome, this.distanceFromHome);
		animator.SetFloat(TerritorialSMUtility.ParameterNames.RelativeBearingToHome, this.relativeBearingToHome);
		//object location
		animator.SetFloat(TerritorialSMUtility.ParameterNames.ObjectLocation, this.objectLocation);
	}

	private bool AtHome()
	{
		//determines whether the vehicle is at the home base within its territory
		float distance = this.DistanceFromHome();
		if (distance > this.homeBaseRadius)
		{
			return false;
		}
		else
		{
			return true;
		}
	}

	private float DistanceFromHome()
	{
		//calculates the vehicles exact distance from the centre of its territory
		Vector3 vehiclePosition = this.vehicle.transform.position;
		float distance = Vector3.Distance(new Vector3(vehiclePosition.x, this.territory.transform.position.y, vehiclePosition.z), this.territory.transform.position);
		return distance;
	}

	private float GetRelativeBearingToHome()
	{
		return Vector3.Angle(this.territory.transform.position - this.transform.position, this.transform.forward);
	}
	
	private void GetObjectLocation()
	{
		bool flagHit = false;
		for (int i = 0; i < this.rSensor.numberOfRays; i++)
		{
			if (rSensor.rayCollision[i])
			{
				flagHit = true;
				float half = rSensor.numberOfRays / 2;
				//if (i > 5)
				if (i > half)
				{
					this.objectLocation = -1;
				}
				//else if ((i > 0) && (i <= 5))
				else if ((i > 0) && (i <= half))
				{
                    this.objectLocation = 1;
				}
				else
				{
                    this.objectLocation = 0;
				}
                previousLocation = this.objectLocation;
				break;
			}
		}
        if (!flagHit)
        {
            //could not resolve the location of the object
            //choose a random location
            this.objectLocation = previousLocation;
        }
		
	}

   
		
	internal override void Execute ()
	{
		if (territory.penetrated)
		{
			this.GetObjectLocation();
		}
        if (isAttacking)
        {
            base.Execute();
            //this.rearSteerAngle = 0f;
        }
//        else
//        {
//			//probably don't need these
//            this.motorTorque = this.motorTorqueSM;
//            this.frontSteerAngle = this.frontSteerAngleSM;
//            this.rearSteerAngle = this.rearSteerAngleSM;
//        }
	}
}
	
