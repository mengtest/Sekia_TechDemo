Shader "XCode/Sample2"
{
	Properties
	{
		_BaseMap("BaseMap", 2D) = "white" {}
		_BaseMap2("BaseMap2", 2D) = "white" {}
	}

	SubShader
	{
		Tags { "RenderPipeline"="UniversalPipeline" "Queue"="Transparent" }
		
		Pass
		{
			Name "Forward"
			Tags { "LightMode"="UniversalForward" }
			Blend SrcAlpha OneMinusSrcAlpha
			ColorMask RGB
			ZWrite Off

			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

			CBUFFER_START(UnityPerMaterial)
				float4 _BaseMap_ST;
			CBUFFER_END
			TEXTURE2D(_BaseMap); SAMPLER(sampler_BaseMap);
			TEXTURE2D(_BaseMap2); SAMPLER(sampler_BaseMap2);

			struct a2v
			{
				float3 positionOS 					: POSITION;
				float2 uv0 							: TEXCOORD0;
			};

			struct v2f
			{
				float4 clipPos 						: SV_POSITION;
				float4 uv0 							: TEXCOORD0;
			};

			v2f vert ( a2v i )
			{
				v2f o = (v2f)0;
				float3 positionWS = TransformObjectToWorld(i.positionOS);
				float4 positionCS = TransformWorldToHClip(positionWS);
				o.uv0.xy = i.uv0.xy * _BaseMap_ST.xy + _BaseMap_ST.zw;
				o.clipPos = positionCS;
				return o;
			}

			half4 frag ( v2f i  ) : SV_Target
			{
				float2 UV_0 = i.uv0.xy;
				half4 _BaseMapValue = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, UV_0);
				_BaseMapValue *= SAMPLE_TEXTURE2D(_BaseMap2, sampler_BaseMap2, UV_0 * 1.0);
				half3 _Color = _BaseMapValue.rgb; 
				half _Alpha = _BaseMapValue.a;
				return half4(_Color, _Alpha);
			}
			ENDHLSL
		}
	}
}