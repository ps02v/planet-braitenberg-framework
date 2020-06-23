using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenCaptureRecorder : DataRecorder {

	[Tooltip("Specifies whether recording is enabled.")]
	public bool record = false;

	private int frameCount = 0;
	private float lastCaptureTime = 0f;
	private float captureInterval;

	internal override void OnStart()
	{
		if (record) {
			//set data capture rate to zero to ensure that data capture is invoked from update
			int tempDataCaptureRate = this.outputDetails.dataCaptureRate;
			this.outputDetails.dataCaptureRate = 0;
			base.OnStart ();
			if (this.isValid) {
				Time.captureFramerate = this.outputDetails.dataCaptureRate;
			}
			this.outputDetails.dataCaptureRate = tempDataCaptureRate;
			this.captureInterval = MyRoutines.ConvertRateToInterval (this.outputDetails.dataCaptureRate);
		} 
		else {
			this.recording = false;
		}
	}

	internal override void OnReset()
	{
		this.outputDetails = new DataOutputDetails (string.Empty, 1, Mathf.Infinity, false, string.Empty, string.Empty);
	}

	internal override void CaptureData ()
	{
		if (this.isValid == false )
			return;
		if (record == false) {
			//recording has been disabled
			recording = false;
		}
		if (Time.time < (this.lastCaptureTime + this.captureInterval))
			return;
		if (recording)
		{
			string name = string.Format("{0}/{1:D04}.png", this.outputDetails.outputPath, this.frameCount);
			ScreenCapture.CaptureScreenshot(name);
			this.frameCount++;
			this.lastCaptureTime = Time.time;
			if (this.frameCount % this.outputDetails.dataCaptureRate == 0)
			{
				Debug.Log ("Captured " + this.frameCount / outputDetails.dataCaptureRate + " seconds.");
			}
		}
	}

	void OnApplicationQuit()
	{
		Time.captureFramerate = 0;
	}


}
