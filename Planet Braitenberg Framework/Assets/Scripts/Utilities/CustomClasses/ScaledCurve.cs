using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ScaledCurve {

	public bool showScale = true;
	public int yScale = 1;
	public int xScale = 1;
	public int xMaxScale = 1;
	public int yMaxScale = 1;
	public Color curveColor = Color.green;
	public string curveType;
	public AnimationCurve curve = AnimationCurve.Linear (0, 0, 1, 1);

	public ScaledCurve()
	{

	}

	public AnimationCurve ResetCurve()
	{
		return AnimationCurve.Linear (0, 0, xScale, yScale);
	}

	public ScaledCurve(bool showScale, Color curveColor, int yScale, int xScale, int yMaxScale, int xMaxScale, string curveType)
	{
		this.showScale = showScale;
		this.curveColor = curveColor;
		this.yScale = yScale;
		this.xScale = xScale;
		this.yMaxScale = yMaxScale;
		this.xMaxScale = xMaxScale;
		this.curveType = curveType;
		curve = AnimationCurve.Linear (0, 0, xScale, yScale);
	}

}
