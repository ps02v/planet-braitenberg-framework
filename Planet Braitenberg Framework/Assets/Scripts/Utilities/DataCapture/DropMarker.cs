using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropMarker : MonoBehaviour {

	[Tooltip("The transform from which drop markers will be deposited.")]
	public Transform targetTransform;
	[Tooltip("The drop marker prefab.")]
	public GameObject dropMarkerPrefab;
	[Tooltip("Specifies whether markers will be placed at a particular rate.")]
	public bool useRate;
	[Tooltip("The rate at which the drop markers will be placed (number of drop markers per second).")]
	public float rate = 1f;
	[Tooltip("A list of times at which drop markers will be deposited.")]
	public float[] times;


	private Queue<float> q;

	// Use this for initialization
	void Start () {
		if (useRate) {
			InvokeRepeating ("DepositMarker", 0, MyRoutines.ConvertRateToInterval (rate));
		} else {
			//use a list of times
			//sort the times in increasing order
			List<float> t = new List<float>(this.times);
			t.Sort ();
			foreach (float f in t) {
				Debug.Log (f.ToString ());
			}
			q = new Queue<float> (t);
		}
	}

	void DepositMarker()
	{
		//instantiate the drop marker
		Vector3 pos = new Vector3(this.targetTransform.position.x, 0.01f, this.targetTransform.position.z);
		Instantiate (this.dropMarkerPrefab, pos, targetTransform.rotation);
	}
		
	// Update is called once per frame
	void Update () {
		if (q == null || q.Count == 0) return;
		if (Time.time >= q.Peek ()) {
			this.DepositMarker ();
			q.Dequeue ();
		}
	}
}
