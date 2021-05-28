Shader "Custom/FresnelShader" {
	Properties {
	 	_Shininess ("Shininess", Range (0.01, 3)) = 1
	 	_Fade ("Material Fade", Range (0.00, 1)) = 0

	 	_ShineColor ("Shine Color", Color) = (1,1,1,1) 
	 	_BaseColor ("Base Color Color", Color) = (1,1,1,1) 

		_MainTex ("Base (RGB)", 2D) = "white" {}

		_Bump ("Bump", 2D) = "bump" {}

	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Lambert

		sampler2D _MainTex;
		sampler2D _Bump;
		float _Shininess;
		float _Fade;
		fixed4 _ShineColor; 
		fixed4 _BaseColor; 

		struct Input {
			float2 uv_MainTex;
			float2 uv_Bump;
			float3 viewDir;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 mat = tex2D (_MainTex, IN.uv_MainTex);
			half4 base = _BaseColor;
			half4 c = _Fade * mat + (1 - _Fade) * base; 
			o.Normal = UnpackNormal(tex2D(_Bump, IN.uv_Bump));
			half factor = dot(normalize(IN.viewDir),o.Normal);
			o.Albedo = c.rgb + _ShineColor*(_Shininess-factor*_Shininess);
			o.Emission.rgb = _ShineColor*(_Shininess-factor*_Shininess);
			o.Alpha = c.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}