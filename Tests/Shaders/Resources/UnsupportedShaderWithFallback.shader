// Copyright (c) 2026 Koji Hasegawa.
// This software is released under the MIT License.

// Test fixture: same unsupported-SubShader trick as UnsupportedShader, but with a
// Fallback declaration. Unity then silently renders the fallback shader and reports
// Shader.isSupported == true, so the material scan cannot flag it by design; only
// the shader fallback warning log (when the Unity version emits one) can reveal it.
Shader "TestHelper/Tests/UnsupportedShaderWithFallback"
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
    Fallback "Legacy Shaders/Diffuse"
}
