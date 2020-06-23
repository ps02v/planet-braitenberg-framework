using UnityEngine;
using System.Collections;

public class Territory : MonoBehaviour {

	internal bool penetrated = false; //indicates whether the territory has been penetrated by a foreign object

	void OnTriggerEnter(Collider other)
	{
		if (other.GetComponent<Collider>().tag == TagManager.ForeignObject)
		{
			//if the object entering the territory is the foreign object then disable it
			other.GetComponent<ForeignObject>().isActive = false;
		}
	}
	
	void OnTriggerStay(Collider other)
	{
		//detect whether the foreign object is within the territory
		if (other.GetComponent<Collider>().tag == TagManager.ForeignObject)
		{
			this.penetrated = true;
		}
	}
	
	void OnTriggerExit(Collider other)
	{
		//detect when the foreign object has exited the territory
		if (other.GetComponent<Collider>().tag == TagManager.ForeignObject)
		{
			this.penetrated = false;
			other.gameObject.GetComponent<Rigidbody>().drag = 0.0f;
		}
	}
	


}
