using UnityEngine;
using System.Collections;

[RequireComponent (typeof (GrayscaleEffect))]
public class Retina : MonoBehaviour {

	//public Eye eye;
	[Tooltip("The type of eye the retina belongs to."), RequiredField()]
	public EyeType eyeType;

	internal bool showUniform = false;

	private Light directionalLight;
	private Light vehicleSpotLight;
	private GrayscaleEffect grayScaleEffect;
	public Material simpleRetinaMaterial;
	public Material complexRetinaMaterial;
	private Color ambientLighting;
	private Vehicle vehicle;
	private Color _directionalLightOriginalColor;
	private Vector3 _directionalLightOriginalRotation;
	private float _directionalLightOriginalIntensity;
	private bool vehicleSpotLightOriginalSetting;
	private Eye eye;

	private Camera _camera;

	void Awake()
	{
		//get a reference to the vehicle
		vehicle = this.GetComponentInParent<Vehicle> ();
		//get a reference to the vehicle spotlight
		GameObject lightObj = vehicle.gameObject.GetChildObjectByName ("Vehicle Spotlight");
		//get a reference to the light component of the vehicle spotlight
		this.vehicleSpotLight = lightObj.GetComponent<Light> ();
		//get a reference to the main directional light
		directionalLight = GameObject.FindGameObjectWithTag(TagManager.DirectionalLight).GetComponent<Light>();
		//get a reference to the GrayScaleEffect component
		grayScaleEffect = GetComponent<GrayscaleEffect>();
		//get a reference to the camera component
		_camera = GetComponent<Camera> ();
		//get a reference to the eye that contains the retina.
		if (this.eyeType == null) {
			Debug.LogError ("Null reference for retina eye type.");
			return;
		}
		foreach (Eye e in this.GetComponentsInParent<Eye>(true)) {
			if (e.eyeType == this.eyeType) {
				this.eye = e;
				break;
			}
		}
		//log error if eye with the appropriate type cannot be found
		if (this.eye == null) {
			Debug.LogError ("Could not find eye for retina. Are you missing an EyeType reference?");
		}
		this.ambientLighting = RenderSettings.ambientLight;
		this.vehicleSpotLightOriginalSetting = this.vehicle.vehicleSpotlight;
		complexRetinaMaterial = new Material (Shader.Find ("Custom/ComplexEye"));
		simpleRetinaMaterial = new Material (Shader.Find ("Custom/AverageBrightness"));
	}
	
//	void Start () 
//	{
////		this.ambientLighting = RenderSettings.ambientLight;
////		this.vehicleSpotLightOriginalSetting = this.vehicle.vehicleSpotlight;
////		complexRetinaMaterial = new Material (Shader.Find ("Custom/ComplexEye"));
////		simpleRetinaMaterial = new Material (Shader.Find ("Custom/AverageBrightness"));
//	}

	void Update () {
		//disable the grey scale effect if this is a color eye
		grayScaleEffect.enabled = !this.eye.hasColorVision;
	}
	
	void OnPreCull ()
	{//called before the camera starts rendering the scene
		//disbale the directional light if the eye cannot see it
		if (this.eye.canSeeDirectionalLight == false) {
			directionalLight.enabled = false;
		} else {
			//configure the directional light with the specific properties
			directionalLight.enabled = true;
			_directionalLightOriginalIntensity =  directionalLight.intensity;
			_directionalLightOriginalColor = directionalLight.color;
			_directionalLightOriginalRotation = directionalLight.transform.rotation.eulerAngles;
			directionalLight.intensity = this.eye.lightConfig.intensity;
			directionalLight.color = this.eye.lightConfig.color;
			//directionalLight.transform.localEulerAngles = this.eye.directionalLightRotation;
			this.directionalLight.transform.localRotation = Quaternion.Euler(this.eye.lightConfig.rotation);
		}
		//the vehicle should never see the spotlight
		vehicleSpotLight.enabled = false;
		if (this.eye.canSeeAmbientLighting == false) RenderSettings.ambientLight = Color.black;
	}
	
	void OnPostRender() 
	{
		// reenable the directional light
		directionalLight.enabled = true;
		if (this.eye.canSeeDirectionalLight == true) {
			this.directionalLight.intensity = _directionalLightOriginalIntensity;
			this.directionalLight.color = _directionalLightOriginalColor;
			//this.directionalLight.transform.eulerAngles = _directionalLightOriginalRotation;
			this.directionalLight.transform.rotation = Quaternion.Euler(_directionalLightOriginalRotation);
			//this.directionalLight.transform.rotation = this._directionalLightOriginalRotation;
		}
		//renable spotlight
		if (this.vehicleSpotLightOriginalSetting) {
			vehicleSpotLight.enabled = true;
		}
		if (this.eye.canSeeAmbientLighting == false) RenderSettings.ambientLight = this.ambientLighting;
	}
	
	void OnRenderImage(RenderTexture src, RenderTexture dest) {
		if (this.showUniform) //showUniform will be false if the rendering routine is invoked by the Eye component.
		{
			if (eye.hasColorVision == false) {
				//if this not a color eye, then render as a grayscale image
				if (eye.isComplexEye) {
					switch (eye.eyeDisplayMode) {
					case EyeMode.Minimum: //render minimum greyscale information
						complexRetinaMaterial.SetColor ("_Patch1Color", eye.retinalPatchesVisualInfo [0, 0].GetMinGreyscale ());
						complexRetinaMaterial.SetColor ("_Patch2Color", eye.retinalPatchesVisualInfo [1, 0].GetMinGreyscale ());
						complexRetinaMaterial.SetColor ("_Patch3Color", eye.retinalPatchesVisualInfo [2, 0].GetMinGreyscale ());
						complexRetinaMaterial.SetColor ("_Patch4Color", eye.retinalPatchesVisualInfo [3, 0].GetMinGreyscale ());
						complexRetinaMaterial.SetColor ("_Patch5Color", eye.retinalPatchesVisualInfo [4, 0].GetMinGreyscale ());
						break;
					case EyeMode.Maximum: //render maximum greayscale information
						complexRetinaMaterial.SetColor ("_Patch1Color", eye.retinalPatchesVisualInfo [0, 0].GetMaxGreyscale ());
						complexRetinaMaterial.SetColor ("_Patch2Color", eye.retinalPatchesVisualInfo [1, 0].GetMaxGreyscale ());
						complexRetinaMaterial.SetColor ("_Patch3Color", eye.retinalPatchesVisualInfo [2, 0].GetMaxGreyscale ());
						complexRetinaMaterial.SetColor ("_Patch4Color", eye.retinalPatchesVisualInfo [3, 0].GetMaxGreyscale ());
						complexRetinaMaterial.SetColor ("_Patch5Color", eye.retinalPatchesVisualInfo [4, 0].GetMaxGreyscale ());
						break;
					default: //render average greyscale information
						complexRetinaMaterial.SetColor ("_Patch1Color", eye.retinalPatchesVisualInfo [0, 0].GetAverageGreyscale ());
						complexRetinaMaterial.SetColor ("_Patch2Color", eye.retinalPatchesVisualInfo [1, 0].GetAverageGreyscale ());
						complexRetinaMaterial.SetColor ("_Patch3Color", eye.retinalPatchesVisualInfo [2, 0].GetAverageGreyscale ());
						complexRetinaMaterial.SetColor ("_Patch4Color", eye.retinalPatchesVisualInfo [3, 0].GetAverageGreyscale ());
						complexRetinaMaterial.SetColor ("_Patch5Color", eye.retinalPatchesVisualInfo [4, 0].GetAverageGreyscale ());
						break;
					}
				} else { //the eye is a simple eye
					switch (eye.eyeDisplayMode) {
					case EyeMode.Minimum: //render minimum greyscale information
						simpleRetinaMaterial.SetColor ("_Color", eye.retinalPatchesVisualInfo [0, 0].GetMinGreyscale ());
						break;
					case EyeMode.Maximum: //render maximum greyscale information
						simpleRetinaMaterial.SetColor ("_Color", eye.retinalPatchesVisualInfo [0, 0].GetMaxGreyscale ());
						break;
					default: //render average greyscale information
						simpleRetinaMaterial.SetColor ("_Color", eye.retinalPatchesVisualInfo [0, 0].GetAverageGreyscale ());
						break;
					}
				}
			} else {
				//the eye can process color information
				if (eye.isComplexEye) {
					switch (eye.eyeDisplayMode) {
					case EyeMode.Minimum:
						complexRetinaMaterial.SetColor ("_Patch1Color", eye.retinalPatchesVisualInfo [0, 0].GetMinColor ());
						complexRetinaMaterial.SetColor ("_Patch2Color", eye.retinalPatchesVisualInfo [1, 0].GetMinColor ());
						complexRetinaMaterial.SetColor ("_Patch3Color", eye.retinalPatchesVisualInfo [2, 0].GetMinColor ());
						complexRetinaMaterial.SetColor ("_Patch4Color", eye.retinalPatchesVisualInfo [3, 0].GetMinColor ());
						complexRetinaMaterial.SetColor ("_Patch5Color", eye.retinalPatchesVisualInfo [4, 0].GetMinColor ());
						break;
					case EyeMode.Maximum:
						complexRetinaMaterial.SetColor ("_Patch1Color", eye.retinalPatchesVisualInfo [0, 0].GetMaxColor ());
						complexRetinaMaterial.SetColor ("_Patch2Color", eye.retinalPatchesVisualInfo [1, 0].GetMaxColor ());
						complexRetinaMaterial.SetColor ("_Patch3Color", eye.retinalPatchesVisualInfo [2, 0].GetMaxColor ());
						complexRetinaMaterial.SetColor ("_Patch4Color", eye.retinalPatchesVisualInfo [3, 0].GetMaxColor ());
						complexRetinaMaterial.SetColor ("_Patch5Color", eye.retinalPatchesVisualInfo [4, 0].GetMaxColor ());
						break;
					default:
						complexRetinaMaterial.SetColor ("_Patch1Color", eye.retinalPatchesVisualInfo [0, 0].GetAverageColor ());
						complexRetinaMaterial.SetColor ("_Patch2Color", eye.retinalPatchesVisualInfo [1, 0].GetAverageColor ());
						complexRetinaMaterial.SetColor ("_Patch3Color", eye.retinalPatchesVisualInfo [2, 0].GetAverageColor ());
						complexRetinaMaterial.SetColor ("_Patch4Color", eye.retinalPatchesVisualInfo [3, 0].GetAverageColor ());
						complexRetinaMaterial.SetColor ("_Patch5Color", eye.retinalPatchesVisualInfo [4, 0].GetAverageColor ());
						break;
					}
				} else {
					switch (eye.eyeDisplayMode) {
					case EyeMode.Minimum:
						simpleRetinaMaterial.SetColor ("_Color", eye.retinalPatchesVisualInfo [0, 0].GetMinColor ());
						break;
					case EyeMode.Maximum:
						simpleRetinaMaterial.SetColor ("_Color", eye.retinalPatchesVisualInfo [0, 0].GetMaxColor ());
						break;
					default:
						simpleRetinaMaterial.SetColor ("_Color", eye.retinalPatchesVisualInfo [0, 0].GetAverageColor ());	
						break;
					}
				}
			}
			if (eye.isComplexEye) {
				Graphics.Blit (src, dest, complexRetinaMaterial);
			} else {
				Graphics.Blit(src, dest, simpleRetinaMaterial);
			}
		}
		else
		{
			RenderTexture tempRT = _camera.targetTexture;
			_camera.targetTexture = null;
			Graphics.Blit(src, dest);
			_camera.targetTexture = tempRT;
		}
	}
	
}