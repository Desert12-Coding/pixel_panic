// Create a new shader (Right-click in Project > Create > Shader > Unlit Shader)
Shader "Custom/LavaFlow"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NoiseTex ("Noise Texture", 2D) = "gray" {}
        _Speed ("Flow Speed", Range(0,1)) = 0.5
        _Distortion ("Distortion", Range(0,1)) = 0.2
        _Color1 ("Color 1", Color) = (1,0.3,0,1)
        _Color2 ("Color 2", Color) = (1,0.6,0,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
            
            sampler2D _MainTex;
            sampler2D _NoiseTex;
            float4 _MainTex_ST;
            float _Speed;
            float _Distortion;
            fixed4 _Color1;
            fixed4 _Color2;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // Animate UVs
                float2 uv = i.uv;
                uv.x += _Time.y * _Speed * 0.1;
                uv.y += _Time.y * _Speed * 0.05;
                
                // Sample noise texture
                float noise = tex2D(_NoiseTex, uv).r;
                
                // Create flowing effect
                float2 distortion = float2(noise * _Distortion, noise * _Distortion * 0.5);
                float flow = sin(_Time.y * _Speed + i.uv.x * 5) * 0.5 + 0.5;
                
                // Color blending
                fixed4 col = lerp(_Color1, _Color2, flow + noise * 0.3);
                
                return col;
            }
            ENDCG
        }
    }
}