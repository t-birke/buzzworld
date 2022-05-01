Shader "CircularBarRotatedMask" {
    Properties {
        _Frac ("Progress Bar Value", Range(0,1)) = 1.0
        [NoScaleOffset] _AlphaTex ("Alpha", 2D) = "White" {}
        _FillColor ("Fill Color", Color) = (1,1,1,1)
        _BackColor ("Back Color", Color) = (0,0,0,1)
        [Toggle(SMOOTHSTEP)] _Smoothstep ("Use Smoothstep", Float) = 0.0
    }

    SubShader {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane"}

        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma shader_feature SMOOTHSTEP 

            #include "UnityCG.cginc"

            // Direct3D compiled stats:
            // vertex shader:
            //   12 math
            // fragment shader:
            //   13 math, 1 texture w/o smoothstep
            //   18 math, 1 texture w/ smoothstep

            half _Frac;
            fixed4 _FillColor;
            fixed4 _BackColor;

            sampler2D _AlphaTex;

            struct v2f {
                float4 pos : SV_POSITION;
                float3 uvMask : TEXCOORD0;
            };

            v2f vert (appdata_img v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);

                // rescale default quad UVs to a -1 to +1 range so 0 is centered
                o.uvMask.xy = v.texcoord.xy * 2.0 - 1.0;

                // flip so bar is clockwise to be consistent with other examples
                o.uvMask.x = -o.uvMask.x;

                // calculate radian angle from progress bar value
                float angle = _Frac * (UNITY_PI * 2.0) - UNITY_PI;

                // get sine and cosine value for angle
                float sinX, cosX;
                sincos(angle, sinX, cosX);

                // construct 2D rotation matrix and rotate centered uvs to get angled mask
                float2x2 rotationMatrix = float2x2(cosX, -sinX, sinX, cosX);
                o.uvMask.z = mul(o.uvMask.xy, rotationMatrix).x;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                half diag = i.uvMask.z;
                half vert = i.uvMask.x;

                // sharpen masks with screen space derivates
                diag = i.uvMask.z / fwidth(i.uvMask.z);
                vert = i.uvMask.x / fwidth(i.uvMask.x);

                // "flip" the masks depending on progress bar value
                half barProgress = 0.0;
                if (_Frac < 0.5)
                    barProgress = max(diag, vert);
                else
                    barProgress = min(diag, vert);

                // mask bottom of progress bar when below 20%
                if (_Frac < 0.2 && i.uvMask.y < 0.0)
                    barProgress = 1.5;

            #if defined(SMOOTHSTEP)
                barProgress = smoothstep(0.25, 1.25, barProgress);
            #else
                barProgress = saturate(barProgress);
            #endif

                // lerp between colors
                fixed4 col = lerp(_FillColor, _BackColor, barProgress);

                fixed alpha = tex2D(_AlphaTex, i.uvMask.xy * 0.5 + 0.5).a;
                col.a *= alpha;

                return col;
            }
            ENDCG
        }
    }
}