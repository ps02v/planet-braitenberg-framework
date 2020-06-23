using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

[System.Serializable]
public class XYDataList {

	private List<XYDataItem> _DataList = new List<XYDataItem> ();

	public void Add(System.Object x, System.Object y)
	{
		this._DataList.Add (new XYDataItem (x, y));
	}

	public void Save(string path)
	{
		StringBuilder sb = new StringBuilder ();
		int counter = 1;
		foreach (XYDataItem dItem in this._DataList) {
			//Debug.Log ("Printing " + counter);
			sb.AppendLine (dItem.ToString ());
			counter++;
		}
		File.WriteAllText (path, sb.ToString ());
	}

	public void Clear()
	{
		this._DataList.Clear ();
	}
}
