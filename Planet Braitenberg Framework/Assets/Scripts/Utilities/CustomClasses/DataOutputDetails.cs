using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DataOutputDetails {

	[Tooltip("The path to which data will be saved.")]
	public string outputPath; 

	[Tooltip("The data capture rate (data captures per second). If this is less than zero, the data capture routine will be called every frame.")]
	public int dataCaptureRate = 1; 

	[Tooltip("The number of seconds that the data recorder will run for (a value of Infinity indicates no stop time).")]
	public float stopTime = 30.0F; 

	public bool isFile = true;

	public string defaultFileName = "SimulationData";

	public string fileExtension = "txt";

	public string rateFieldLabel = "Data Capture Rate";

	public DataOutputDetails(string outputPath, int dataCaptureRate, float stopTime, bool isFile, string defaultFileName, string fileExtension)
	{
		this.outputPath = outputPath;
		this.dataCaptureRate = dataCaptureRate;
		this.stopTime = stopTime;
		this.isFile = isFile;
		this.defaultFileName = defaultFileName;
		this.fileExtension = fileExtension;
	}

	public DataOutputDetails(string outputPath, int dataCaptureRate, float stopTime, bool isFile, string defaultFileName, string fileExtension, string rateFieldLabel)
		: this (outputPath, dataCaptureRate, stopTime, isFile, defaultFileName, fileExtension)
	{
		this.rateFieldLabel = rateFieldLabel;
	}

	public DataOutputDetails(){
	}
}
