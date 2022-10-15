Shader "XCode/Sample0"
{
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
				o.uv0.xy = i.uv0.xy;
				o.clipPos = positionCS;
				return o;
			}

			half4 frag ( v2f i  ) : SV_Target
			{
				half3 intput = half3(i.uv0.xy, 0.0h);
				half3 _Color = intput;
				half _Alpha = 0.5h;
				return half4(_Color, _Alpha);
			}
			ENDHLSL
		}
	}
}