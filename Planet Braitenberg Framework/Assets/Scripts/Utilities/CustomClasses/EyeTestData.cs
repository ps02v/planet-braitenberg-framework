using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

internal class EyeTestData {
	//the EyeTestData class is used to generate information for testing purposes
	internal string tag;
	internal Texture2D mainTexture;
	internal Texture2D processedTexture;
	internal bool isColor;
	internal bool isComplex;
	internal float time;
	internal ColorInformation[,] RetinalPatchInfo;

	internal EyeTestData(string tag, Texture2D mainTexture, Texture2D processedTexture, bool isColor, bool isComplex, ColorInformation[,] retinalPatchInfo)
	{
		this.tag = tag;
		this.mainTexture = mainTexture;
		this.processedTexture = processedTexture;
		this.time = Time.time;
		this.isColor = isColor;
		this.isComplex = isComplex;
		this.RetinalPatchInfo = retinalPatchInfo;
	}

	public override string ToString ()
	{
		StringBuilder sb = new StringBuilder ();
		sb.AppendLine ("Tag: " + this.tag);
		sb.AppendLine ("Time: " + this.time);
		sb.AppendLine ("Is Color: " + this.isColor.ToString());
		sb.AppendLine ("Is Complex: " + this.isComplex.ToString());
		for (int i = 0; i < this.RetinalPatchInfo.GetLength(0); i++)
		{
			for (int j = 0; j < this.RetinalPatchInfo.GetLength(1); j++)
			{
				sb.AppendLine ();
				this.RetinalPatchInfo [i, j].GetDescription (sb);
			}
		}
		return sb.ToString();
	}

}