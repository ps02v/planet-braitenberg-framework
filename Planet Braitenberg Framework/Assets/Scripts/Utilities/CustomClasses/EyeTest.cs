using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class EyeTest {

	[Tooltip("Outputs eye-related data to a custom folder.")]
	public bool enabled = false;

	[Tooltip("The folder to which data will be saved.")]
	public string outputFolder; //the folder to which data will be saved

	[Tooltip("The data capture rate (number of captures per second).")]
	public float rate = 1.0F; //the rate at which data will be saved

	[Tooltip("Specifies whether the processed image texture will be included in the eye test.")]
	public bool captureProcessedImage = false; //the rate at which data will be saved

	[Tooltip("The number of seconds that the data recorder will run for (a value of Infinity indicates no stop time).")]
	public float stopTime = Mathf.Infinity; 

	internal bool initializationError = false;

	private int counter = 0;
	private float timeInterval = 0.0F;

	internal void Initialize () {
//		if (string.IsNullOrEmpty(this.outputFolder))
//			this.outputFolder = System.IO.Path.Combine(Application.dataPath, "EyeTestData");
		this.initializationError = false;
		try{
			if (System.IO.Directory.Exists(this.outputFolder))
			{
				System.IO.Directory.Delete(this.outputFolder, true);
			}
			System.IO.Directory.CreateDirectory(this.outputFolder);
		}
		catch (System.Exception e) {
			this.initializationError = true;
			Debug.LogError (e);
		}  
	}

	internal void WriteTestData(EyeTestData data)
	{
		//Debug.Log ("writing eye test data:" + data.Tag);
		//if there was an initialization error then return
		if (this.initializationError) return;
		// if the relevant amount of time has not passed then return
		if (this.timeInterval > Time.time) 
		{
			return;
		}
		this.counter++;
		// encode the texture into a PNG
		byte[] bytes = data.mainTexture.EncodeToPNG();
		//write the texture to a file
		System.IO.File.WriteAllBytes(System.IO.Path.Combine(this.outputFolder, data.tag + "_" + data.time + ".png"), bytes);
		if (data.processedTexture != null) {
			byte[] bytes2 = data.processedTexture.EncodeToPNG();
			//write the texture to a file
			System.IO.File.WriteAllBytes(System.IO.Path.Combine(this.outputFolder, data.tag + "_" + data.time + "_processed.png"), bytes2);
		}
		//write the eye data to a file
		System.IO.File.WriteAllText(System.IO.Path.Combine(this.outputFolder, data.tag + "_" + data.time + ".txt"), data.ToString());
		//update the time interval
		this.timeInterval = Time.time + this.rate;
	}

}
