using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoUIElement : MonoBehaviour {

	public Color colorIfNegative = new Color (1.0f, 0.5f, 0f, 1.0f);
	public bool isBool = false; //specifies that the value should be interpreted as a boolean value
	public string prefix;
	public int decimalPlaces = 3;

	internal float value;

	private Text textElement;
	private Color defaultColor;

	void Awake()
	{
		this.textElement = GetComponent<Text> ();
		this.defaultColor = textElement.color;
	}

	void Update () {
		if (value < 0) {
			this.textElement.color = this.colorIfNegative;
		} 
		else 
		{
			this.textElement.color = this.defaultColor;
		}
		if (this.isBool) 
		{
			if (value < 0) 
			{
				this.textElement.text = this.prefix + "False";
			} else 
			{
				this.textElement.text = this.prefix + "True";
			}
		}
		else 
		{
			this.textElement.text = prefix + MyRoutines.Round(value, this.decimalPlaces).ToString ();
		}
	}
}
