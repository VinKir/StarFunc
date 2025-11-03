Shader "Custom/GridShader"
{
    Properties
    {
        _LineColor ("Line Color", Color) = (1, 1, 1, 1)
        _BackgroundColor ("Background Color", Color) = (0.52, 0.52, 0.52, 1) // #858585
        _Scale ("Grid Cell Size", Float) = 1
        _LineThickness ("Line Thickness", Float) = 0.02
        _MajorThickness ("Major Line Thickness", Float) = 0.04
        _AxisThickness ("Axis Thickness", Float) = 0.06
        _MajorInterval ("Major Interval", Float) = 10
    }

    SubShader
    {
        Tags {"Queue"="Transparent" "RenderType"="Transparent"}
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float2 worldPos : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float4 _LineColor;
            float4 _BackgroundColor;
            float _Scale;
            float _LineThickness;
            float _MajorThickness;
            float _AxisThickness;
            float _MajorInterval;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xy;
                return o;
            }

            float isMultiple(float value, float interval)
            {
                float n = round(value);
                float r = fmod(abs(n), interval);
                return step(r, 0.001);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 coord = i.worldPos / _Scale;

                float distX = abs(frac(coord.x + 0.5) - 0.5);
                float distY = abs(frac(coord.y + 0.5) - 0.5);

                float baseLineX = step(distX, _LineThickness);
                float baseLineY = step(distY, _LineThickness);

                float isMajorX = isMultiple(coord.x, _MajorInterval);
                float isMajorY = isMultiple(coord.y, _MajorInterval);

                float majorLineX = step(distX, _MajorThickness) * isMajorX;
                float majorLineY = step(distY, _MajorThickness) * isMajorY;

                float lineX = max(baseLineX, majorLineX);
                float lineY = max(baseLineY, majorLineY);

                float lineMask = max(lineX, lineY);

                float axisMaskX = step(abs(coord.x), _AxisThickness);
                float axisMaskY = step(abs(coord.y), _AxisThickness);
                float axisMask = max(axisMaskX, axisMaskY);

                float3 color = lerp(_BackgroundColor.rgb, _LineColor.rgb, lineMask);
                color = lerp(color, _LineColor.rgb, axisMask);

                return float4(color, 1);
            }
            ENDCG
        }
    }
}
