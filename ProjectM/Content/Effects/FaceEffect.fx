#if OPENGL
    #define VS_SHADERMODEL vs_3_0
    #define PS_SHADERMODEL ps_3_0
#else
    #define VS_SHADERMODEL vs_4_0
    #define PS_SHADERMODEL ps_4_0
#endif

matrix View;
matrix Projection;

Texture2D Texture;
sampler2D TextureSampler = sampler_state
{
    Texture = <Texture>;
    AddressU = Wrap;
    AddressV = Wrap;
    MinFilter = Point;
    MagFilter = Point;
    MipFilter = Point;
};

struct VertexInput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
};

struct InstanceInput
{
    float4 Row1 : TEXCOORD1;
    float4 Row2 : TEXCOORD2;
    float4 Row3 : TEXCOORD3;
    float4 Row4 : TEXCOORD4;
    
    float2 TexCoord : TEXCOORD5;
};

struct VertexOutput
{
    float4 Position : SV_Position;
    float2 TexCoord : TEXCOORD0;
};

VertexOutput MainVS(in VertexInput vertex, in InstanceInput instance)
{
    VertexOutput output = (VertexOutput)0;

    matrix World = matrix(
        instance.Row1,
        instance.Row2,
        instance.Row3,
        instance.Row4
    );
    
    float4 worldPosition = mul(vertex.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
    
    output.TexCoord = vertex.TexCoord + instance.TexCoord;
    
    return output;
}

float4 MainPS(VertexOutput input) : COLOR0
{
    return tex2D(TextureSampler, input.TexCoord);
}

technique MainTechnique
{
    pass Pass1
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
}