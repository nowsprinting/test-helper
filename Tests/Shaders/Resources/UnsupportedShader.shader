// Copyright (c) 2026 Koji Hasegawa.
// This software is released under the MIT License.

// Test fixture: a shader that compiles without error but is not supported at runtime
// (Shader.isSupported == false) on any platform other than d3d11/d3d9, because it
// declares only_renderers for platforms this project's CI/dev machines never run on.
Shader "TestHelper/Tests/UnsupportedShader"
{
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma only_renderers d3d11 d3d9
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return fixed4(1, 0, 1, 1);
            }
            ENDCG
        }
    }
}
