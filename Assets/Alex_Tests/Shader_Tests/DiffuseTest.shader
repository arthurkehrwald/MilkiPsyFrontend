Shader "Custom/DiffuseTest"
{
    Properties
    {
        _Color("Color", Color) = (1,0,0,1)
    }

    SubShader
    {
        Tags { "Lightmode" = "ForwardBase" }
        LOD 100

        Pass 
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "UnityLightingCommon.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
            float4 vertex : SV_POSITION;
            float3 worldNormal : TEXCOORD0;
            };

            float4 _Color;
            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                float3 worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldNormal = worldNormal;
                return o;
            }
            float4 frag(v2f i) : SV_Target
            {
                float3 normalDirection = normalize(i.worldNormal);

                float nl = max(0.0, dot(normalDirection, _WorldSpaceLightPos0.xyz));
                float4 diffuseTerm = nl * _Color * _LightColor0;

                return diffuseTerm;
            }
            ENDCG
        }
        
    }
    
}
