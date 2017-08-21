Shader "Custom/ForceField" 
{
	Properties
	{
		_RimColor("Rim Color", Color) = (0.26,0.19,0.16,0.0)
		_RimPower("Rim Power", Range(0.5,8.0)) = 3.0
		_RimEmission("Rim Emission", Color) = (0.26,0.19,0.16,0.0)
		_HitColor("Hit Color", Color) = (0.26,0.19,0.16,0.0)
		_HitEmission("Hit Emission", Color) = (0.26,0.19,0.16,0.0)
		_Radius("Max Radius", float) = 30
	}
	SubShader
	{
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
		CGPROGRAM
		#pragma surface surf NoLighting alpha
		#pragma target 3.0

		struct Input 
		{
			float3 viewDir;
		};
		float4 _RimColor;
		float _RimPower;
		float4 _RimEmission;
		float4 _HitColor;
		float4 _HitEmission;
		float _Radius;

		int _PointsLength;
		uniform float3 _Points[50];
		uniform float _PointsStrength[50];
		uniform float _PointsAngle[50];

		UNITY_INSTANCING_CBUFFER_START(Props)
		UNITY_INSTANCING_CBUFFER_END

		fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten)
		{
			fixed4 c;
			c.rgb = s.Albedo;
			c.a = s.Alpha;
			return c;
		}

		half GetFalloff(float3 normal)
		{
			half h = 0;

			for (int i = 0; i < _PointsLength; i++)
			{
				half angle = acos(dot(_Points[i].xyz, normal));

				angle = abs(angle - _PointsAngle[i]);

				angle = saturate(angle / _Radius);
								
				half hi = (1 - angle) * saturate(_PointsStrength[i]);
				
				h += hi;
			}

			return saturate(h);
		}

		void surf(Input IN, inout SurfaceOutput o)
		{
			half rim = pow(1 - saturate(dot(normalize(IN.viewDir), o.Normal)), _RimPower);
			half hits = GetFalloff(o.Normal);
			o.Albedo = lerp(_RimColor.rgb, _HitColor.rgb, hits);
			o.Alpha = rim * _RimColor.a + hits * _HitColor.a;
			o.Emission = lerp(_RimEmission.rgb, _HitEmission.rgb, hits);
		}
		ENDCG
	}
	Fallback "Diffuse"
}