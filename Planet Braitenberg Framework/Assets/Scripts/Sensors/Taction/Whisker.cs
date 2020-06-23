using UnityEngine;
using System.Collections;

public class Whisker : MonoBehaviour {

    public Transform whisker;

    public bool activated = false;

    void OnCollisionEnter(Collision collision)
    {
        GameObject go = collision.contacts[0].thisCollider.gameObject;
        if (collision.gameObject.tag == "Wall" && go.transform == this.whisker)
        {
            activated = true;
        }
    }

	void OnCollisionStay(Collision collision)
	{
		GameObject go = collision.contacts[0].thisCollider.gameObject;
		if (collision.gameObject.tag == "Wall" && go.transform == this.whisker)
		{
			activated = true;
		}
	}

    void OnCollisionExit(Collision collision)
    {
        activated = false;

//        GameObject go = collision.contacts[0].thisCollider.gameObject;
//        if (collision.gameObject.tag == "Wall" && go.transform == this.whisker)
//        {
//            activated = false;
//        }
    }
}