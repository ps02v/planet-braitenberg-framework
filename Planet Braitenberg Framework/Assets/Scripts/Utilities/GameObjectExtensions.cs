using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class GameObjectExtensions {

	public static void GetChildObjects(this GameObject o, List<GameObject> list)
	{
		foreach (Transform trans in o.transform)
		{
			trans.gameObject.GetChildObjects(list);
			list.Add(trans.gameObject);	
		}
	}
	
	public static void WalkChildObjects(this GameObject o, System.Action<GameObject> f)
	{
		f(o);
		int numChildren = o.transform.childCount;
		for (int i = 0; i < numChildren; i++)
		{
			o.transform.GetChild(i).gameObject.WalkChildObjects(f);
		}
	}
	
	public static GameObject GetChildObjectByName(this GameObject o, string name)
	{
		List<GameObject> children = new List<GameObject>();
		o.GetChildObjects(children);
		return children.Find(
			delegate(GameObject go)
			{
				return go.name == name;
			}
			);
			
	}


}
