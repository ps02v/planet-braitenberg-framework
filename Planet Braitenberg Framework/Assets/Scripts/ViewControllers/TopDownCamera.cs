using UnityEngine;
using System.Collections;
using UnityEditor;

[RequireComponent(typeof (Camera))]
public class TopDownCamera : MonoBehaviour
{
	[Tooltip("The move speed of the camera.")]
    public float moveSpeed = 1.0f;
	[Tooltip("Links the camera to a particular transform in the scene.")]
	public Transform targetTransform;
	[Tooltip("Links the camera to the transform of the primary vehicle in the scene.")]
	public bool linkToVehicle = false;

	internal float maxNearZoom = 5.0f; //the minimum orthographic size of the camera.
	internal float maxFarZoom = 40.0f; //the maximum orthographic size of the camera
//	internal const float minOrthoSizeLimit = 3.0f;
//	internal const float maxOrthoSizeLimit = 50.0f;

	private Camera _camera;
   

    void Awake()
    {
		this._camera = this.GetComponent<Camera> ();
    }

	void Start()
	{
		if (this.linkToVehicle) {
			//set the target transform to the vehicle - there should be only one vehicle with the Vehicle tag - all other vehicles should be tagged as secondary vehicles.
			this.targetTransform = GameObject.FindGameObjectWithTag (TagManager.Vehicle).transform;
		}
		if (targetTransform != null) {
			//parent the transform to the target transform
			this.transform.parent = targetTransform;
			//zero out x and y
			this.transform.localPosition = new Vector3(0, this.transform.localPosition.y, 0);
		}
	}

    // Update is called once per frame
    void Update()
    {
        //return if the camera is not enabled
        if (this._camera.enabled == false) return;
		//wasd keys move the camera
		if (targetTransform == null) {
			//calcuate the position
			float vertical = Input.GetAxis("Vertical") * moveSpeed;
			float horizontal = Input.GetAxis("Horizontal") * moveSpeed;
			transform.Translate(horizontal, 0, vertical, Space.World);
		}
		//use mouse wheel to zoom in and out
		//calculate the zoom
		float zoom = 0; 
		if (Input.GetAxis ("Mouse ScrollWheel") < 0) { //back
			zoom++;
		} 
		if (Input.GetAxis ("Mouse ScrollWheel") > 0) { //forward
			zoom--;
		}
        float orthoZoom = _camera.orthographicSize + zoom;
        if (orthoZoom < this.maxNearZoom)
        {
			//can't move closed to the ground than 5f
            orthoZoom = this.maxNearZoom;
        }
        else if (orthoZoom > this.maxFarZoom)
        {
			//can't move farther from the ground that 50f
            orthoZoom = this.maxFarZoom;
        }
        _camera.orthographicSize = orthoZoom;
    }
}

