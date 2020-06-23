using UnityEngine;
using System.Collections;

public class ScreenCaptureScript : MonoBehaviour {

	public string outputFolder = "C:/ScreenRecordings/";
	public int frameRate = 25;
	public bool record = false;
	
	private int frameCount = 0;
	private bool initError = false;

	// Use this for initialization
	void Start () {
		try{
			if (record)
			{
				Time.captureFramerate = this.frameRate;
				if (System.IO.Directory.Exists(this.outputFolder))
				{
					System.IO.Directory.Delete(this.outputFolder, true);
				}
				System.IO.Directory.CreateDirectory(this.outputFolder);
			}
		}
		catch (System.Exception e) {
			this.initError = true;
			Debug.LogError(e);
		}  
	}
	
	// Update is called once per frame
	void Update () {
		//if there was an initialization error then return
		if (this.initError) return;
		if (record)
		{
			string name = string.Format("{0}/{1:D04}.png", this.outputFolder, this.frameCount);
			ScreenCapture.CaptureScreenshot(name);
			this.frameCount++;
			if (this.frameCount % this.frameRate == 0)
			{
				Debug.Log ("Captured " + this.frameCount / this.frameRate + " seconds.");
			}
		}
	}

	void OnApplicationQuit()
	{
		Time.captureFramerate = 0;
	}
}