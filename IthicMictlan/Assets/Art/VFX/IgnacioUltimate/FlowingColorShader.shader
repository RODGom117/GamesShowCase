// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/FlowingWaterShader"
{
    Properties
    {
        _Color ("Main Color", Color) = (1, 1, 1, 1)
        _Speed ("Flow Speed", Range (0, 10)) = 1
        _WaveFrequency ("Wave Frequency", Range (0, 10)) = 1
        _WaveAmplitude ("Wave Amplitude", Range (0, 1)) = 0.1
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        CGPROGRAM
        #pragma surface surf Lambert vertex:vert

        struct Input
        {
            float3 worldPos;
            float3 viewDir;
            float4 color : COLOR;
        };

        fixed4 _Color;
        float _Speed;
        float _WaveFrequency;
        float _WaveAmplitude;

        void vert(inout appdata_full v, out Input o)
        {
            // Calculate the flow offset based on time and speed
            float flowOffset = _Speed * _Time.y;

            // Create a flowing function using sin and cos
            float flow = _WaveAmplitude * sin(flowOffset + _WaveFrequency * v.vertex.x) * cos(flowOffset + _WaveFrequency * v.vertex.y);

            // Displace the vertex along its normal based on the flowing function
            v.vertex.xyz += v.normal * flow;

            o.color = _Color;
            o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
            o.viewDir = UnityObjectToViewPos(v.vertex);
        }


        void surf(Input IN, inout SurfaceOutput o)
        {
            // Apply the color to the output
            o.Albedo = _Color.rgb;
            o.Alpha = _Color.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
