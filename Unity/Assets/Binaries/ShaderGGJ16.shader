Shader "GGJ16/Alpha" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	_ColorEmissive("Emissive Color", Color) = (1,1,1,1)
	_EmissiveTex("Emissive Texture", 2D) = "black" {}
}

SubShader {
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	LOD 200

CGPROGRAM
#pragma surface surf Lambert alpha:fade

sampler2D _MainTex;
fixed4 _Color;

sampler2D _EmissiveTex;
fixed4 _ColorEmissive;

struct Input {
	float2 uv_MainTex;
};

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
	o.Albedo = c.rgb;
	fixed4 e = tex2D(_EmissiveTex, IN.uv_MainTex) * _ColorEmissive * 4;
	o.Emission = e * e.a;
	o.Alpha = c.a;
}
ENDCG
}

Fallback "Legacy Shaders/Transparent/VertexLit"
}
