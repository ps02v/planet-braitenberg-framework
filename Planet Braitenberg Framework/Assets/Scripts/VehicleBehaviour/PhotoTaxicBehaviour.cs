using UnityEngine;
using System.Collections;

public enum PhotoTaxicResponseType {Photophilia, Photophobia}

public class PhotoTaxicBehaviour : BinocularVehicleBehaviourBase
{
	[Tooltip("Indicates whether the vehicle is attracted (philia) or repulsed (phobia) by light.")]
	public PhotoTaxicResponseType responseType = PhotoTaxicResponseType.Photophilia;
	
	internal override void Execute ()
	{
		base.Execute();
		//nothing to do for photophilia - the phototaxic behaviour component simply implements the base behaviour.
		if (this.responseType == PhotoTaxicResponseType.Photophobia) {
			//for a photophobic vehicle we simply need to invert the steer angle.
			frontSteerAngle *= -1;
		}
	}
}
