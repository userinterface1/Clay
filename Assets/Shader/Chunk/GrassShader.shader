Shader "Custom/GrassShader"
{
// The properties block of the Unity shader. In this example this block is empty
    // because the output color is predefined in the fragment shader code.
    Properties
    { 
        _grassHeight("Grass Height", Float) = 0
    }

    // The SubShader block containing the Shader code.
    SubShader
    {
        // SubShader Tags define when and under which conditions a SubShader block or
        // a pass is executed.
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
Cull Off
            // The HLSL code block. Unity SRP uses the HLSL language.
            HLSLPROGRAM
#pragma target 4.0
            // This line defines the name of the vertex shader.
            #pragma vertex vert
#pragma geometry geom
            // This line defines the name of the fragment shader.
            #pragma fragment frag

            // The Core.hlsl file contains definitions of frequently used HLSL
            // macros and functions, and also contains #include references to other
            // HLSL files (for example, Common.hlsl, SpaceTransforms.hlsl, etc.).
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // The structure definition defines which variables it contains.
            // This example uses the Attributes structure as an input structure in
            // the vertex shader.
struct Attributes
{
                // The positionOS variable contains the vertex positions in object
                // space.
    float4 positionOS : POSITION;
    float3 normalOS : NORMAL;
};

struct Varyings
{
                // The positions in this struct must have the SV_POSITION semantic.
    float4 positionHCS : SV_POSITION;
    float3 positionWS : TEXCOORD0;
    float3 normalOS : TEXCOORD1;
};

float4 viewPosition;
float _grassHeight;

            // The vertex shader definition with properties defined in the Varyings
            // structure. The type of the vert function must match the type (struct)
            // that it returns.
Varyings vert(Attributes IN)
{
                // Declaring the output object (OUT) with the Varyings struct.
    Varyings OUT;
                // The TransformObjectToHClip function transforms vertex positions
                // from object space to homogenous clip space.
    OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);
    OUT.positionHCS = 0;
    OUT.normalOS = IN.normalOS;
    return OUT;
}

float random(float2 uv)
{
    return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453123);
}

[maxvertexcount(6)]
void geom(triangle Varyings input[3], inout TriangleStream<Varyings> OutputStream)
{     
    if (input[1].positionWS.y >= _grassHeight && length(viewPosition - input[1].positionWS) <= 50)
    {
        Varyings OUT;
        
        float3 a = input[0].positionWS;
        float3 b = input[1].positionWS;
        float3 c = input[2].positionWS;
        
        float t1 = random(a.xz);
        float t2 = (1 - t1) * random(b.xz);
        float t3 = (1 - (t1 + t2));
        
        float3 barycentric = (a * t1) + (b * t2) + (c * t3);
        
        float3 normal = input[0].normalOS;
        float3 up = float3(0, 1, 0);
        float3 left = cross(normal, up);
        float3 forward = cross(normal, left);
        
        left = normalize(left);
        forward = normalize(forward);
        
        float4x4 local2World = float4x4(
            left.x, normal.x, forward.x, barycentric.x,
            left.y, normal.y, forward.y, barycentric.y,
            left.z, normal.z, forward.z, barycentric.z,
            0, 0, 0, 1
        );
        
        if (dot(normal, up) > 0.6f)
        {
            OutputStream.RestartStrip();
            OUT.positionHCS = TransformWorldToHClip(mul(local2World, float4(0, 0.3f, 0, 1)));
            OUT.positionWS = 0;
            OUT.normalOS = 0;
            OutputStream.Append(OUT);
            OUT.positionHCS = TransformWorldToHClip(mul(local2World, float4(-0.02f, 0, 0, 1)));
            OutputStream.Append(OUT);
            OUT.positionHCS = TransformWorldToHClip(mul(local2World, float4(0.02f, 0, 0, 1)));
            OutputStream.Append(OUT);
        
            OutputStream.RestartStrip();
            OUT.positionHCS = TransformWorldToHClip(mul(local2World, float4(0, 0.3f, 0, 1)));
            OutputStream.Append(OUT);
            OUT.positionHCS = TransformWorldToHClip(mul(local2World, float4(0, 0, -0.02f, 1)));
            OutputStream.Append(OUT);
            OUT.positionHCS = TransformWorldToHClip(mul(local2World, float4(0, 0, 0.02f, 1)));
            OutputStream.Append(OUT);
        }      
    }
}

            // The fragment shader definition.
half4 frag() : SV_Target
{
                // Defining the color variable and returning it.
    half4 customColor = half4(0.5f, 3.0f, 0.5f, 1.0f);
    return customColor;
}
            ENDHLSL
        }
    }
}
