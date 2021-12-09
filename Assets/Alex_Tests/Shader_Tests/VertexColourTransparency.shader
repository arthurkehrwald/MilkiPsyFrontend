// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/VertexColourTransparent"
{
	Properties{
		_Color("Tint", Color) = (0, 0, 0, 1)

		//
		//_Cubemap("Cubemap", Cube) = "" {}
		_RefractRatio("Reflect Ratio", Range(0, 1)) = 0.667
		_F0("F0", Range(0, 1)) = 0.5
	}

	SubShader{
		Tags { "RenderType" = "Transparent" "Queue" = "Transparent" 
		"LightMode" = "ForwardBase"}
		LOD 100

		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite off
					 
		/*Stencil {
				  Ref 10
				  ReadMask 10
				  Comp NotEqual
				  Pass Replace
			  }*/

		Pass {
			CGPROGRAM

			#include "UnityCG.cginc"

			//
			#include "Lighting.cginc"
			#include "AutoLight.cginc"

			#pragma vertex vert
			#pragma fragment frag

			#pragma multi_compile_fwdbase

			//
			samplerCUBE _Cubemap;
			float _RefractRatio;
			float _F0;

			fixed4 _Color;

			struct appdata {
				float4 vertex : POSITION;
				float4 color : COLOR;

				float3 normal : NORMAL;
			};

			struct v2f {
				float4 position : SV_POSITION;
				float4 color : COLOR;

				float3 worldNormal : TEXCOORD0;
				float3 worldPos : TEXCOORD1;
				SHADOW_COORDS(2)
			};

			v2f vert(appdata v) {
				v2f o;
				o.position = UnityObjectToClipPos(v.vertex);

				o.color = v.color;

				o.worldNormal = normalize(UnityObjectToWorldNormal(v.normal));
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				//TRANSFER_SHADOW(o);
				return o;
			}

			fixed4 frag(v2f i) : SV_TARGET {
				
				float4 col = i.color;
				col *= _Color;

				/*
				fixed3 albedo = i.color.rgb;
				fixed3 ambient = albedo * UNITY_LIGHTMODEL_AMBIENT.rgb;
				*/


				float3 worldView = UnityWorldSpaceViewDir(i.worldPos);
				/*
				float3 reflectDir = reflect(-worldView, i.worldNormal);
				float3 refractDir = refract(-normalize(worldView), normalize(i.worldNormal), _RefractRatio);
				fixed3 reflectCol = texCUBE(_Cubemap, reflectDir);
				fixed3 refractCol = texCUBE(_Cubemap, refractDir);
				*/


				float schlick = _F0 + (1 - _F0) * pow(1 - dot(worldView, i.worldNormal), 5);

				UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);

				//fixed3 col = ambient + lerp(reflectCol, refractCol, schlick) * atten;
				//return fixed4(col, 1);

				return schlick*col;
			}

			ENDCG
		}
	}
		FallBack "Diffuse"
}

