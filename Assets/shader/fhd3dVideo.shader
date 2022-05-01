Shader "fhd3dVideo" {
Properties {
    [Gamma] _Exposure ("Exposure", Range(0, 8)) = 1.0
    _PixelDivider ("over under pixel offset", Int) = 45
    [NoScaleOffset] _MainTex ("Texture", 2D) = "white" {}
    [Enum(over under, 0, side by side, 1)] _ImageType("3D Type", Float) = 0
    _Cutoff ("Alpha Cutoff", Range(0,1)) = 0.5
}

SubShader {
    Tags { "RenderType"="Cutout" }
    LOD 100

    Pass {

        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag

        #include "UnityCG.cginc"

        struct appdata
        {
            float4 vertex : POSITION;
            float2 uv : TEXCOORD0;
            UNITY_VERTEX_INPUT_INSTANCE_ID
        };

        struct v2f
        {
            float2 uv : TEXCOORD0;
            float4 vertex : SV_POSITION;
            float4 layout3DScaleAndOffset : TEXCOORD2;
            UNITY_VERTEX_OUTPUT_STEREO
        };

        sampler2D _MainTex;
        float4 _MainTex_ST;
        //half _Exposure;
        int _ImageType;
        fixed _Cutoff;

        v2f vert (appdata v)
        {
            v2f o;
            UNITY_SETUP_INSTANCE_ID(v);
            UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
            o.vertex = UnityObjectToClipPos(v.vertex);
            o.uv = TRANSFORM_TEX(v.uv, _MainTex);
            // Calculate constant scale and offset for 3D layouts Over-Under 3D layout

            if (_ImageType == 0) // Over-Under 3D layout
                o.layout3DScaleAndOffset = float4(0, 1-unity_StereoEyeIndex,1,0.5);
            else // Side-by-Side 3D layout
                o.layout3DScaleAndOffset = float4(unity_StereoEyeIndex,0,0.5,1);

            
            return o;
        }

        fixed4 frag (v2f i) : SV_Target
        {
            // sample the texture
            float2 coords;
            if (_ImageType == 0) // Over-Under 3D layout
                coords = float2(i.uv.x, (i.uv.y + i.layout3DScaleAndOffset.y) * 0.5);
            else // Side-by-Side 3D layout
                coords = float2((i.uv.x + i.layout3DScaleAndOffset.x) * 0.5, i.uv.y);
            fixed4 col = tex2D(_MainTex,coords );
            clip(col.a - _Cutoff);
            return col;
        }
        ENDCG
    }
}

Fallback Off

}