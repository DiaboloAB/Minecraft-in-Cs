#if OPENGL
    #define VS_SHADERMODEL vs_3_0
    #define PS_SHADERMODEL ps_3_0
#else
    #define VS_SHADERMODEL vs_4_0
    #define PS_SHADERMODEL ps_4_0
#endif

// Matrix uniforms
matrix View;
matrix Projection;

// Vertex shader input structures
struct VertexInput
{
    float4 Position : POSITION0;
    float4 Color : COLOR0;
};

struct InstanceInput
{
    float4 Row1 : TEXCOORD1;
    float4 Row2 : TEXCOORD2;
    float4 Row3 : TEXCOORD3;
    float4 Row4 : TEXCOORD4;
    float4 Color : COLOR1;
};

// Vertex shader output structure
struct VertexOutput
{
    float4 Position : SV_Position;
    float4 Color : COLOR0;
};

// Vertex shader
VertexOutput MainVS(in VertexInput vertex, in InstanceInput instance)
{
    VertexOutput output = (VertexOutput)0;

    // Reconstruct world matrix from instance data
    matrix World = matrix(
        instance.Row1,
        instance.Row2,
        instance.Row3,
        instance.Row4
    );
    
    // Transform position
    float4 worldPosition = mul(vertex.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
    
    // Combine vertex and instance colors
    output.Color = vertex.Color * instance.Color;
    
    return output;
}

// Pixel shader
float4 MainPS(VertexOutput input) : COLOR0
{
    return input.Color;
}

// Technique
technique MainTechnique
{
    pass Pass1
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
}
