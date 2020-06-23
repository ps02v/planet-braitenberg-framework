using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;



public class LoomingLightBehaviour : PhotoTaxicBehaviour
{
   
	[Range(0.1f, 10.0f), Tooltip("Specifies the rate of retinal processing (in seconds).")]
    public float lookRate = 4f;
	[Range(1, 10), Tooltip("The capacity of the eye's memory buffers. With a look interval of 1, a capacity of 4 means that the will eye remember information from the previous 4 seconds.")]
	public int eyeMemoryCapacity = 4; 

	internal bool isActivated = false; //indicates whether the vehicle has been activated
    
	private Queue<ColorInformation[,]> eyeMemoryLeft;
	private Queue<ColorInformation[,]> eyeMemoryRight;
	private int counter = 0;

	// Use this for initialization
	internal override void Start()
    {
        base.Start();
		this.isActivated = false;


		//disable the vehicle motor
        this.vehicle.disableMotor = true;
		//initialize the vehicle's eye memory
		eyeMemoryLeft = new Queue<ColorInformation[,]> (this.eyeMemoryCapacity);
		eyeMemoryRight = new Queue<ColorInformation[,]> (this.eyeMemoryCapacity);
		//initialize the vehicle's eyes
		this.InitEye (this.leftEye);
		this.InitEye (this.rightEye);
		this.InvokeRepeating("ProcessScene", 0, MyRoutines.ConvertRateToInterval(this.lookRate));
	}

	private void InitEye(Eye eye)
	{
		//the vehicle should have complex eyes
		eye.isComplexEye = true;
		//no need for the vehicles to process color information
		eye.hasColorVision = false;
	}

	private struct EyeInformation
	{
		internal float patch1;
		internal float patch2;
		internal float patch3;
		internal float patch4;
		internal float patch5;
	}
		
	private void ProcessScene()
	{
		//execute visual processing
		counter++;
		this.leftEye.Execute ();
		this.rightEye.Execute ();
		//check whether the memory buffers are at capacity and adjust their contents accordingly
		this.CheckMemoryBuffers (this.eyeMemoryLeft);
		this.CheckMemoryBuffers (this.eyeMemoryRight);
		//add the new data to the beginning of the buffers
		this.eyeMemoryLeft.Enqueue((ColorInformation[,])this.leftEye.retinalPatchesVisualInfo.Clone());
		this.eyeMemoryRight.Enqueue((ColorInformation[,])this.rightEye.retinalPatchesVisualInfo.Clone());
		if (eyeMemoryLeft.Count != this.eyeMemoryCapacity)
			return; //wait until the memory buffer is full before processing anything
		//process the contents of the eye memory buffers - check whether the conditions for alooming light are met
		if (ProcessEyeMemory(false, this.eyeMemoryRight) & ProcessEyeMemory(true, this.eyeMemoryLeft))
		{
			//the condition has been met
			//enable the vehicle's motor
			this.vehicle.disableMotor = false;
			//record the fact that the vehicle has been activated
			this.isActivated = true;
			CancelInvoke ("ProcessScene");
		}
	}

	private void CheckMemoryBuffers(Queue<ColorInformation[,]> mBuffer)
	{
		if (mBuffer.Count == this.eyeMemoryCapacity) {
			//the buffer is at capacity
			//remove the oldest element
			mBuffer.Dequeue();
		}
	}


	private bool ProcessEyeMemory(bool isLeftEye, Queue<ColorInformation[,]> mBuffer)
	{
		//first check whether luminosity is the same or increasing in the three most medial retinal patches.
		if ((this.CheckPatchIsIncreasing (isLeftEye, mBuffer, 1) == false) || (this.CheckPatchIsIncreasing (isLeftEye, mBuffer, 2) == false) 
			|| (this.CheckPatchIsIncreasing (isLeftEye, mBuffer, 3) == false)) {
			//failed the first test
			return false;
		}
		//now check that at each time slot, the brightness values increase from medial to lateral - the most medial patches should always be the brightest
		return this.CheckMedialToLateralIncrease(isLeftEye, mBuffer);
	}

	private bool CheckPatchIsIncreasing(bool isLeftEye, Queue<ColorInformation[,]> mBuffer, int patchNumber)
	{
		//checks whether the luminosity in the specified patch is increasing or at least stable across time
		List<EyeInformation> eyeInfo = this.ExtractColorInformationFromQueue(mBuffer, isLeftEye);
		float[] resultArray = new float[eyeInfo.Count];
		int count = 0;
		foreach (EyeInformation e in eyeInfo) {
			switch (patchNumber) {
			case 1: //most medial
				resultArray [count] = e.patch1;
				break;
			case 2: 
				resultArray [count] = e.patch2;
				break;
			case 3: 
				resultArray [count] = e.patch3;
				break;
			case 4: 
				resultArray [count] = e.patch4;
				break;
			case 5: //most lateral
				resultArray [count] = e.patch5;
				break;
			}
			count++;
		}
		//by this point we have an array of values for the target patch across the duration of the eye memory buffer
		//we need to check that these are increasing in value, or at least not decreasing across time.
		float referenceValue = resultArray[0];
		bool detectedIncrease = false;
		int cnt = 0;
		foreach (float value in resultArray) {
			cnt++;
			if (value < referenceValue) {
				return false;
			}
			if (value > referenceValue) {
				detectedIncrease = true;
			}
		}
		//we know that there is no decrease in the values; however, we also need to ensure that the brightness is increasing over time
		if (detectedIncrease) {
			return true;
		} else {
			return false;
		}
	}

	private bool CheckMedialToLateralIncrease(bool isLeftEye, Queue<ColorInformation[,]> mBuffer)
	{
		float value1, value2, value3;
		List<EyeInformation> eyeInfo = this.ExtractColorInformationFromQueue(mBuffer, isLeftEye);
		foreach (EyeInformation e in eyeInfo) {
			value1 = e.patch1;
			value2 = e.patch2;
			value3 = e.patch3;
			if ((value1 < value2) || (value2 < value3)) {
				return false;
			}
		}
		return true;
	}

	private List<EyeInformation> ExtractColorInformationFromQueue(Queue<ColorInformation[,]> mBuffer, bool isLeftEye)
	{
		//mBuffer contains two-dimensional arrays that represent retinal patches
		//we are assuming that there is only one row of retinal patches
		//so we return an array of ColorInformation representing retinal columns
		List<EyeInformation> arr = new List<EyeInformation>();
		//create the array so that the oldest information is placed first i the array
		foreach (ColorInformation[,] c in mBuffer.ToArray()) {
			if (isLeftEye == false) {
				EyeInformation eyeInfo = new EyeInformation ();
				eyeInfo.patch1 = c[0,0].averageBrightness;
				eyeInfo.patch2 = c[1,0].averageBrightness;
				eyeInfo.patch3 = c[2,0].averageBrightness;
				eyeInfo.patch4 = c[3,0].averageBrightness;
				eyeInfo.patch5 = c[4,0].averageBrightness;
				arr.Add (eyeInfo);
			} else {
				EyeInformation eyeInfo = new EyeInformation ();
				eyeInfo.patch1 = c[4,0].averageBrightness;
				eyeInfo.patch2 = c[3,0].averageBrightness;
				eyeInfo.patch3 = c[2,0].averageBrightness;
				eyeInfo.patch4 = c[1,0].averageBrightness;
				eyeInfo.patch5 = c[0,0].averageBrightness;
				arr.Add (eyeInfo);
			}
		}
		//arr.Reverse ();
		return arr; //reverse the list, so that earlier items are now at the beginning of the list
	}

    internal override void Execute()
    {
        if (isActivated)
        {
            base.Execute();
        }
    }


   

    
}

