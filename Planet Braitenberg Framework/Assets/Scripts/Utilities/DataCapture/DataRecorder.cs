using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DataRecorder : MonoBehaviour {

	public DataOutputDetails outputDetails;

	internal XYDataList dataList = new XYDataList();

	internal bool recording = true;
	internal bool initializationError = false;
	internal bool isValid = true;
	internal bool runInUpdate = false;

	void Reset()
	{
		this.OnReset ();
	}

	internal virtual void OnReset()
	{
		this.outputDetails = new DataOutputDetails ();
	}
		
	void Start () {
		this.OnStart ();
	}

	internal virtual void OnStart()
	{
		this.dataList.Clear ();
		recording = false;
		this.CheckRecorderValidity ();
		if (this.isValid == false)
			return;
		//if datacapture rate is greater than zero then call CaptureData routine with relevant interval
		if (this.outputDetails.dataCaptureRate > 0) {
			this.InvokeRepeating ("CaptureData", 0, MyRoutines.ConvertRateToInterval (this.outputDetails.dataCaptureRate));
			this.runInUpdate = false;
		}
		else
			runInUpdate = true;
		//set recording to true
		recording = true;
	}

	internal virtual void CheckRecorderValidity()
	{
		//implement validation checks
		//the output path should not be null
		this.isValid = true;
		if (string.IsNullOrEmpty (this.outputDetails.outputPath)) {
			Debug.LogError ("Output path not specified.");
			this.isValid = false;
		}
		if (this.outputDetails.isFile) {
			//if this is a file, the extension of the file in outputpath should match the file extension in outputdetails
			if (System.IO.Path.GetExtension (this.outputDetails.outputPath) != ("." + this.outputDetails.fileExtension)) {
				Debug.LogError ("File extension is invalid or no output file was specified.");
				this.isValid = false;
			}
				
		}
		//if we are storing data to a folder then create the target directory
		if (this.outputDetails.isFile == false) {
			if (MyRoutines.CreateDirectory (this.outputDetails.outputPath, false) == false) {
				Debug.LogError ("The directory could not be created.");
				this.isValid = false;
			}
		}
	}

	internal void SaveData ()
	{
		//only save data if this is a file
		if (this.outputDetails.isFile && this.recording && this.isValid)
			this.dataList.Save (this.outputDetails.outputPath);
		if (this.runInUpdate == false)
			CancelInvoke ("CaptureData");
	}

	abstract internal void CaptureData (); 
	
	// Update is called once per frame
	void Update () {
		//if the stoptime as been reached then save the data
		if (Time.time > this.outputDetails.stopTime) {
			this.SaveData ();
			recording = false;
		}
		//if recording is disabled or the component is invalid then return
		if (recording == false || this.isValid == false || this.runInUpdate == false)
			return;
		this.CaptureData ();

	}

	void OnApplicationQuit()
	{
		if (this.enabled)
			this.SaveData ();
	}
}
