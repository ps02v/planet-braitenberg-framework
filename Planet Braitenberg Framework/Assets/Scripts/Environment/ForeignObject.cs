using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class ForeignObject : MonoBehaviour {

	[Tooltip("Indicates whether the foreign object is activated.")]
	public bool isActive = false;

	[RequiredFieldAttribute, Tooltip("The territory that the foreign object will attempt to invade.")]
	public Territory territory;

	private Rigidbody _rigidbody;

	void Awake()
	{
		this._rigidbody = this.GetComponent<Rigidbody> ();
	}

    /// <summary>
    /// 
    /// </summary>
	void FixedUpdate () {
		//move the foreign object into the vehicle's territory
		if (isActive)
		{
			Ray dir = new Ray(transform.position, (territory.transform.position - transform.position).normalized);
			float dist = Vector3.Distance(territory.transform.position, transform.position);
			if(this.GetComponent<Rigidbody>())
			{
				Vector3 direction = new Vector3(dir.direction.x, 0, dir.direction.z);
				this._rigidbody.AddForce ((direction) * (0.5f * dist));
			}
		}
	}
}
