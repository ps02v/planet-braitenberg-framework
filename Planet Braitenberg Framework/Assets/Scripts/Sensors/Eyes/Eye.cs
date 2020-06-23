using UnityEngine;
using System.Collections;
using System.Text;

public enum EyeLaterality {left, right, none};

public enum EyeMode {Average, Maximum, Minimum};

public class Eye : MonoBehaviour {

	[RequiredField(), Tooltip("Specifies the type of the eye.")]
	public EyeType eyeType;
	[Tooltip("Specifies whether the eye will process color information.")]
	public bool hasColorVision = false; 
	[Tooltip("Specifies whether the eye will rely on an imagined color.")]
	public bool useImaginedColor = false; 
	[Tooltip("The imagined color to use in situations where the vehicle relies on an imagined color.")]
	public Color imaginedColor = Color.black; 
	[Tooltip("Specifies that this is a complex eye with multiple retinal patches")]
	public bool isComplexEye; 
	//[Tooltip("Indicates the laterality of the eye - used for robots with left and right eyes.")]
	//public EyeLaterality laterality = EyeLaterality.left; 
	[Tooltip("Specifies the initial rotation of the eye.")]
	public Vector3 initialEyeRotation = new Vector3(0, 0, 0);  
	[Tooltip("Specifies the field of view of the eye. This is used to implement zoom functionality.")]
	public float fieldOfView = 60f;

	public EyeTest eyeTest;
	[Tooltip("Specifies whether the eye can see the skybox.")]
	public bool canSeeSkyBox = false; //
	[Tooltip("Specifies whether the eye is sensitive to ambient lighting in the scene.")]
	public bool canSeeAmbientLighting = false; 
	[Tooltip("Specifies whether the eye is sensitive to the main directional light in the scene.")]
	public bool canSeeDirectionalLight = false; 
	[HideInInspector]
	public LightConfiguration lightConfig;

	internal ColorInformation[,] retinalPatchesVisualInfo; //information about retinal patches
	internal const string defaultTagName = "Eye";
	internal Light directionalLight;
	internal EyeMode eyeDisplayMode = EyeMode.Average; //specifies how the eye will render on the relevant eye UI - this does not affect visual processing.

	private Camera _camera; //the camera of the eye
	private Vehicle vehicle; //the vehicle that the eye is associated with
	internal int retinalRows = 1; //assume that a complex eye will have 1 retinal row
	internal int retinalColumns = 5; //assume that a complex eye will have 5 retinal columns
	private int retinaWidth = 200; //thw width of the retina in pixels
	private int retinaHeight = 200; //the height of the retina in pixels
	private string _tag;
	private Retina retina; //the script that provides additional functionality for the camera
	private float lastEyeTestTime = 0f; //the time of the last eye test

	void Awake () {
		
		//get a reference to the vehicle component
		vehicle = GetComponent<Vehicle> ();
		if (vehicle == null) {
			vehicle = this.GetComponentInParent<Vehicle> ();
		}
		//get a reference to the directional light
		this.directionalLight = GameObject.FindGameObjectWithTag (TagManager.DirectionalLight).GetComponent<Light> ();
		if (this.eyeTest.enabled) {
			this.eyeTest.Initialize ();
		}
		//get a reference to the retina
		if (this.eyeType == null) {
			Debug.LogError ("Null reference for eye type.");
			return;
		}
		foreach (Retina r in this.GetComponentsInChildren<Retina>()) {
			if (r.eyeType == this.eyeType) {
				this.retina = r;
				break;
			}
		}
		//log error if retina with the appropriate type cannot be found
		if (this.retina == null) {
			Debug.LogError ("Could not find retina for eye. Are you missing an EyeType reference?");
			return;
		}
		//get a reference to the Camera component.
		_camera = retina.GetComponent<Camera>();
		_camera.enabled = false;
		// the retina of a complex eye is divided into one row of five columns
		if (this.isComplexEye) {
			this.retinalRows = 1;
			this.retinalColumns = 5;
		}
		else { // a simple eye simply has a single retinal patch
			this.retinalRows = 1;
			this.retinalColumns = 1;
		}
		this.retinalPatchesVisualInfo = new ColorInformation[this.retinalColumns, this.retinalRows]; //initialize the retinal patches array with the number of columns and number of rows
		this.SetEyeRotation(this.initialEyeRotation);
		this._tag = this.EyeTag; // this ensures we cannot change the tag during game play
	}

//	void Start() {
////		this.retinalPatchesVisualInfo = new ColorInformation[this.retinalColumns, this.retinalRows]; //initialize the retinal patches array with the number of columns and number of rows
////		this.SetEyeRotation(this.initialEyeRotation);
////		this._tag = this.EyeTag; // this ensures we cannot change the tag during game play
//	}

	internal string EyeTag
	{
		get {
			return this.eyeType.name;
		}
	}

	void Update()
	{
		//set the field of view
		this._camera.fieldOfView = this.fieldOfView;
		//set whether the eye can see the sky
		if (this.canSeeSkyBox) {
			this._camera.clearFlags = CameraClearFlags.Skybox;
		} else {
			this._camera.clearFlags = CameraClearFlags.SolidColor;
			this._camera.backgroundColor = Color.black;
		}

	}

	private bool RecordEyeTestData()
	{
		//only record eye data if:
		//1. the eye test is enabled
		//2. there was no error while initializing the eyetest
		//3. the appropriate amount of time has lapsed since the last data serialization
		//4. the stop time has not been reached
		return (this.eyeTest.enabled == true) && (this.eyeTest.initializationError == false) &&
		(Time.time >= (MyRoutines.ConvertRateToInterval (this.eyeTest.rate) + this.lastEyeTestTime) &&
		(Time.time <= this.eyeTest.stopTime));
	}

	internal void SetEyeRotation(Vector3 rotation)
	{//changes the rotation of the camera representing the eye
		this._camera.transform.localEulerAngles = Vector3.zero;
		this._camera.transform.Rotate(rotation);
	}

	internal void RotateEyeLeft(float angle)
	{
		this.SetEyeRotation (angle * Vector3.down); 
	}

	internal void RotateEyeRight(float angle)
	{
		this.SetEyeRotation (angle * Vector3.up); 
	}
		
	internal void RotateEyeUp(float angle)
	{
		this.SetEyeRotation (angle * Vector3.left); 
	}

	internal void RotateEyeDown(float angle)
	{
		this.SetEyeRotation (angle * Vector3.right); 
	}

	internal ColorInformation GetEntireEyeInformation()
	{//returns color information for the entire eye - used when we are dealing with a simple eye
		float graySum = 0, redSum = 0, blueSum = 0, greenSum = 0;
		float grayMax = 0, redMax = 0, blueMax = 0, greenMax = 0;
		float grayMin = 0, redMin = 0, blueMin = 0, greenMin = 0;
		ColorInformation info = new ColorInformation (0, 0);
		for (int i = 0; i < this.retinalColumns; i++)
		{
			for (int j = 0; j < this.retinalRows; j++)
			{
				ColorInformation patchInfo = this.retinalPatchesVisualInfo [i, j];
				//get maximum color information
				if (patchInfo.maxBrightness > grayMax) grayMax = patchInfo.maxBrightness;
				if (patchInfo.maxRed > redMax) redMax = patchInfo.maxRed;
				if (patchInfo.maxGreen > greenMax) greenMax = patchInfo.maxGreen;
				if (patchInfo.maxBlue > blueMax) blueMax = patchInfo.maxBlue;
	
				//get minimum color information
				if (patchInfo.minBrightness < grayMin) grayMin = patchInfo.minBrightness;
				if (patchInfo.minRed < redMin) redMin = patchInfo.minRed;
				if (patchInfo.minGreen < greenMin) greenMin = patchInfo.minGreen;
				if (patchInfo.minBlue < blueMin) blueMin = patchInfo.minBlue;

				//get total color information
				graySum += patchInfo.averageBrightness;
				redSum += patchInfo.averageRed;
				greenSum += patchInfo.averageGreen;
				blueSum += patchInfo.averageBlue;
			}
		}
		//set maximum color information
		info.maxBrightness = grayMax;
		info.maxRed = redMax;
		info.maxGreen = greenMax;
		info.maxBlue = blueMax;
		//set minimum color information
		info.minBrightness = grayMin;
		info.minRed = redMin;
		info.minGreen = greenMin;
		info.minBlue = blueMin;
		//set average color information
		info.averageBrightness = graySum / (this.retinalColumns * this.retinalRows);
		info.averageRed = redSum / (this.retinalColumns * this.retinalRows);
		info.averageGreen = greenSum / (this.retinalColumns * this.retinalRows);
		info.averageBlue = blueSum / (this.retinalColumns * this.retinalRows);
		return info;
	}
		
	internal void Execute () {
		if (useImaginedColor)
		{//set all retinal patches to the imagined color
			for (int i = 0; i < this.retinalColumns; i++)
			{
				for (int j = 0; j < this.retinalRows; j++)
				{
					this.retinalPatchesVisualInfo [i, j] = new ColorInformation(this.imaginedColor, 0, 0);
				}
			}
			return;
		}
		//record whether the camera should show a uniform color in a temporary variable
		bool tempShowAverage = retina.showUniform;
		//store the camera's target tecture in a temporary variable
		RenderTexture tempRT = _camera.targetTexture;
		//make sure the camera renders the actual scene rather than a uniform color
		retina.showUniform = false;
		//create a new texture to recieve the camera output
		Texture2D tex = new Texture2D(retinaWidth, retinaHeight, TextureFormat.ARGB32, false);
		Texture2D tex2 = null;
		//Texture2D tex = new Texture2D(retinaWidth, retinaHeight, TextureFormat.Alpha8, false);
		//initialise and render the rendertexture that the camera will render to
	 	//RenderTexture rt = new RenderTexture(retinaWidth, retinaHeight, 24, RenderTextureFormat.ARGBHalf);
		RenderTexture rt = new RenderTexture(retinaWidth, retinaHeight, 24);
		//set the targettexture of the camera to the rendertexture
	 	_camera.targetTexture = rt;
		//render the camera
		_camera.Render();
		//set the rendertexture to be the active rendertexture
	 	RenderTexture.active = rt;
	 	//read the pixels from the active render texture
		tex.ReadPixels(new Rect(0, 0, retinaWidth, retinaHeight), 0, 0);
		tex.Apply ();
		if (this.RecordEyeTestData () && this.eyeTest.captureProcessedImage) {
			//need to get secondary texture
			retina.showUniform = true;
			_camera.Render ();
			tex2 = new Texture2D(retinaWidth, retinaHeight, TextureFormat.ARGB32, false);
			tex2.ReadPixels(new Rect(0, 0, retinaWidth, retinaHeight), 0, 0);
			tex2.Apply ();
		}
	 	//reset the camera
		_camera.targetTexture = tempRT;
	 	//destroy the temporary render texture
	 	RenderTexture.active = null;
	 	DestroyImmediate(rt);
	 	//process the texture to extract color information
		this.ProcessRetina (tex, tex2);
		//destroy the texture
	 	DestroyImmediate(tex);
		//reset the EyeCamera.showUniform field
		retina.showUniform = tempShowAverage;
	}
		
	protected virtual void ProcessRetina(Texture2D tex, Texture2D tex2)
	{
		int x, y, width, height;
		//width represents the width of each retinal patch
		width = (int)(this.retinaWidth / this.retinalColumns);
		//height represents the height of each retinal patch
		height = (int)(this.retinaHeight / this.retinalRows);
		//loop through the retinal patches
		for (int i = 0; i < this.retinalColumns; i++)
			{
				for (int j = 0; j < this.retinalRows; j++)
				{
					//calculate the x and y positions of the patch
					x = i * width;
					y = j * height;
					this.retinalPatchesVisualInfo [i, j] = this.GetColorInformationFromPatch (tex, x, y, width, height, i, j);
				}
			}
		if (this.RecordEyeTestData())
		{
			EyeTestData eyeData = new EyeTestData (this._tag, tex, tex2, this.hasColorVision, this.isComplexEye, this.retinalPatchesVisualInfo);
			this.eyeTest.WriteTestData(eyeData);
			this.lastEyeTestTime = Time.time;
		}
	}


	//returns color information from the specified retinal patch
	private ColorInformation GetColorInformationFromPatch(Texture2D tex, int x, int y, int width, int height, int patchColumn, int patchRow)
	{
		ColorInformation info = new ColorInformation(patchRow, patchColumn);
		Color[] pix = tex.GetPixels(x, y, width, height);
		float graySum = 0, redSum = 0, blueSum = 0, greenSum = 0;
		float grayMax = 0, redMax = 0, blueMax = 0, greenMax = 0;
		float grayMin = 0, redMin = 0, blueMin = 0, greenMin = 0;
		for (int i = 0; i < pix.Length; i++)
		{
			//get maximum color information
			if (pix[i].grayscale > grayMax) grayMax = pix[i].grayscale;
			if (pix[i].r > redMax) redMax = pix[i].r;
			if (pix[i].g > greenMax) greenMax = pix[i].g;
			if (pix[i].b > blueMax) blueMax = pix[i].b;

			//get minimum color information
			if (pix[i].grayscale < grayMin) grayMin = pix[i].grayscale;
			if (pix[i].r < redMin) redMin = pix[i].r;
			if (pix[i].g < greenMin) greenMin = pix[i].g;
			if (pix[i].b < blueMin) blueMin = pix[i].b;

			//get total color information
			graySum += pix[i].grayscale;
			redSum += pix[i].r;
			greenSum += pix[i].g;
			blueSum += pix[i].b;
		}
		//set maximum color information
		info.maxBrightness = grayMax;
		info.maxRed = redMax;
		info.maxGreen = greenMax;
		info.maxBlue = blueMax;
		//set minimum color information
		info.minBrightness = grayMin;
		info.minRed = redMin;
		info.minGreen = greenMin;
		info.minBlue = blueMin;
		//set average color information
		info.averageBrightness = graySum / pix.Length;
		info.averageRed = redSum / pix.Length;
		info.averageGreen = greenSum / pix.Length;
		info.averageBlue = blueSum / pix.Length;
		return info;
	}
}

//a class to hold information about the color of retinal patches
internal class ColorInformation
{
	internal float averageBrightness = 0f;
	internal float averageRed = 0f;
	internal float averageGreen = 0f;
	internal float averageBlue = 0f;

	internal float maxBrightness = 0f;
	internal float maxRed = 0f;
	internal float maxGreen = 0f;
	internal float maxBlue = 0f;

	internal float minBrightness = 0f;
	internal float minRed = 0f;
	internal float minGreen = 0f;
	internal float minBlue = 0f;

	internal int patchRow = 0;
	internal int patchColumn = 0;

	internal Color GetAverageColor(){
		//returns the average color of the retinal patch
		return new Color (this.averageRed, this.averageGreen, this.averageBlue, 1.0f);
	}

	internal Color GetAverageGreyscale(){
		//returns the avaerage brightness of the retinal patch
		return new Color (this.averageBrightness, this.averageBrightness, this.averageBrightness, 1.0f);
	}

	internal Color GetMaxColor(){
		//returns the maximum color of the retinal patch
		return new Color (this.maxRed, this.maxGreen, this.maxBlue, 1.0f);
	}

	internal Color GetMaxGreyscale(){
		//returns the maximum brightness of the retinal patch
		return new Color (this.maxBrightness, this.maxBrightness, this.maxBrightness, 1.0f);
	}

	internal Color GetMinColor(){
		//returns the minimum color of the retinal patch
		return new Color (this.minRed, this.minGreen, this.minBlue, 1.0f);
	}

	internal Color GetMinGreyscale(){
		//returns the minimum brightness of the retinal patch
		return new Color (this.minBrightness, this.minBrightness, this.minBrightness, 1.0f);
	}

	internal ColorInformation(int patchRow, int patchColumn)
	{
		this.patchRow = patchRow;
		this.patchColumn = patchColumn;
	}

	internal ColorInformation(Color imaginedColor, int patchRow, int patchColumn): this(patchRow, patchColumn)
	{//instantiate an instance of the ColorInformation class with an imagined color
		this.averageBrightness = imaginedColor.grayscale;
		this.averageRed = imaginedColor.r;
		this.averageGreen = imaginedColor.g;
		this.averageBlue = imaginedColor.b;

		this.maxBrightness = imaginedColor.grayscale;
		this.maxRed = imaginedColor.r;
		this.maxGreen = imaginedColor.g;
		this.maxBlue = imaginedColor.b;

		this.minBrightness = imaginedColor.grayscale;
		this.minRed = imaginedColor.r;
		this.minGreen = imaginedColor.g;
		this.minBlue = imaginedColor.b;
	}

	public void GetDescription(StringBuilder sb)
	{
		//generate descriptive information for the retinal patch
		sb.AppendLine ("Patch Column: " + this.patchColumn.ToString());
		sb.AppendLine ("Patch Row: " + this.patchRow.ToString());
		//output average color information for patch
		sb.AppendLine ("Average Brightness: " + this.averageBrightness.ToString());
		sb.AppendLine ("Average Red: " + this.averageRed.ToString());
		sb.AppendLine ("Average Green: " + this.averageGreen.ToString());
		sb.AppendLine ("Average Blue: " + this.averageBlue.ToString());
		//output maximum color information for patch
		sb.AppendLine ("Max Brightness: " + this.maxBrightness.ToString());
		sb.AppendLine ("Max Red: " + this.maxRed.ToString());
		sb.AppendLine ("Max Green: " + this.maxGreen.ToString());
		sb.AppendLine ("Max Blue: " + this.maxBlue.ToString());
		//output minimum color information for patch
		sb.AppendLine ("Min Brightness: " + this.minBrightness.ToString());
		sb.AppendLine ("Min Red: " + this.minRed.ToString());
		sb.AppendLine ("Min Green: " + this.minGreen.ToString());
		sb.AppendLine ("Min Blue: " + this.minBlue.ToString());
	}

}

