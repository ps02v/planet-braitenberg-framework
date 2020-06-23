Shader "Custom/ComplexEye" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Patch1Color ("Patch 1 Color", Color) = (1,1,1,1)
		_Patch2Color ("Patch 2 Color", Color) = (1,1,1,1)
		_Patch3Color ("Patch 3 Color", Color) = (1,1,1,1)
		_Patch4Color ("Patch 4 Color", Color) = (1,1,1,1)
		_Patch5Color ("Patch 5 Color", Color) = (1,1,1,1)
	}
SubShader {
	Pass {
		ZTest Always Cull Off ZWrite Off
		Fog { Mode off }
				
CGPROGRAM
#pragma vertex vert_img
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest 
#include "UnityCG.cginc"

uniform sampler2D _MainTex;
fixed4 _Patch1Color;
fixed4 _Patch2Color;
fixed4 _Patch3Color;
fixed4 _Patch4Color;
fixed4 _Patch5Color;

fixed4 frag (v2f_img i) : SV_Target
{
	fixed4 output = tex2D(_MainTex, i.uv);
	if(i.uv.x <= 0.20){
		output.rgb = _Patch1Color.rgb;
		return output;
	};
	if(i.uv.x <= 0.40){
		output.rgb = _Patch2Color.rgb;
		return output;
	};
	if(i.uv.x <= 0.6){
		output.rgb = _Patch3Color.rgb;
		return output;
	};
	if(i.uv.x <= 0.8){
		output.rgb = _Patch4Color.rgb;
		return output;
	};
	output.rgb = _Patch5Color.rgb;
	return output;
}
ENDCG

	}
}

Fallback off

}


