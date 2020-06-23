using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceDeltaRecorder : DataRecorder {

	[RequiredFieldAttribute, Tooltip("The source object.")]
	public Transform source;
	[RequiredFieldAttribute, Tooltip("The target object.")]
	public Transform target;

	internal override void OnReset()
	{
		this.outputDetails = new DataOutputDetails (string.Empty, 1, 30f, true, "DistanceDeltaData", "txt");
	}

	internal override void CheckRecorderValidity()
	{
		base.CheckRecorderValidity ();
		if (this.source == null || this.target == null) {
			isValid = false;
		}
	}

	internal override void CaptureData ()
	{
		//save the distance between source and target
		float x = Time.time;
		float y = Vector3.Distance (source.position, target.position);
		this.dataList.Add (x, y);
	}
}
