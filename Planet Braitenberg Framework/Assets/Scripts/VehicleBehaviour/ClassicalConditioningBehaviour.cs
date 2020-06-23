using UnityEngine;
using System.Collections;

[AddComponentMenu("Vehicle/Classical Conditioning Behaviour")]
public class ClassicalConditioningBehaviour : BinocularVehicleBehaviourBase 
{
    public bool testMode = false;
    public float trainingCycles = 5;
    [Range(1.0f, 5.0f)]
    public float trainingStimulusDuration = 1.0f;
    [Range(1.0f, 5.0f)]
    public float testStimulusDuration = 2.0f;
    [Range(0.0f, 1.0f)]
    public float redChannelWeight = 1.0f;
    [Range(0.0f, 1.0f)]
    public float greenChannelWeight = 0.0f;
    [Range(0.0f, 1.0f)]
    public float blueChannelWeight = 0.0f;
    [Range(0.0f, 1.0f)]
    public float learningRate = 0.3f;

    public AnimationCurve activationDecayCurve = AnimationCurve.Linear(0, 1, 1, 0); 

    private float redChannelActivation;
    private float greenChannelActivation;
    private float blueChannelActivation;

    public float channelActivationThreshold = 10.0f;

    private float activationDecayInterval;
    
    [HideInInspector()]
    public bool beginProcessing = false;



	internal override void Start()
    {
        base.Start();
		this.leftEye.hasColorVision = true;
		this.leftEye.isComplexEye = false;
		this.rightEye.hasColorVision = true;
		this.rightEye.isComplexEye = false;
    }

    internal override void Execute()
	{
        if (beginProcessing == false) return;
		base.Execute();
        if (motorTorque > this.channelActivationThreshold)
        {
            this.activationDecayInterval = 0;
        }
        else
        {
            this.activationDecayInterval += Time.deltaTime;
        }
        this.activationDecayInterval = Mathf.Clamp(this.activationDecayInterval, 0.0f, 1.0f);
        float activationLevel = this.activationDecayCurve.Evaluate(this.activationDecayInterval);
        this.redChannelActivation = activationLevel;
        this.greenChannelActivation = activationLevel;
        this.blueChannelActivation = activationLevel;
		ColorInformation leftEyeInfo = this.leftEye.GetEntireEyeInformation ();
		ColorInformation rightEyeInfo = this.rightEye.GetEntireEyeInformation ();
		this.redChannelWeight += (this.redChannelActivation * ((leftEyeInfo.maxRed + rightEyeInfo.maxRed) / 2)) * this.learningRate * Time.deltaTime;
		this.greenChannelWeight += (this.greenChannelActivation * ((leftEyeInfo.maxGreen + rightEyeInfo.maxGreen) / 2)) * this.learningRate * Time.deltaTime;
		this.blueChannelWeight += (this.blueChannelActivation * ((leftEyeInfo.maxBlue + rightEyeInfo.maxBlue) / 2)) * this.learningRate * Time.deltaTime;
        this.redChannelWeight = Mathf.Clamp(this.redChannelWeight, 0.0f, 1.0f);
        this.greenChannelWeight = Mathf.Clamp(this.greenChannelWeight, 0.0f, 1.0f);
        this.blueChannelWeight = Mathf.Clamp(this.blueChannelWeight, 0.0f, 1.0f);
    }

	internal override float CalculateEyeOutput(Eye eye)
    {
        float red, green, blue;
		ColorInformation info = eye.GetEntireEyeInformation ();
		red = info.maxRed * this.redChannelWeight;
		green = info.maxGreen * this.greenChannelWeight;
		blue = info.maxBlue * this.blueChannelWeight;
        //now we calculate an average value
        float average = (red + green + blue) / 3;
        return this.vehicle.EvaluateEyeBrightness(average);
    }


}