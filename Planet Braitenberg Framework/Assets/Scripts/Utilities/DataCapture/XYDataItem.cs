using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class XYDataItem {

	internal System.Object x;
	internal System.Object y;

	internal XYDataItem(System.Object x, System.Object y)
	{
		this.x = x;
		this.y = y;
	}

	public override string ToString ()
	{
		return x.ToString () + "," + y.ToString ();
	}

}
