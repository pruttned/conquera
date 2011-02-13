float4x4 gWorldViewProj : WorldViewProjection;
float4x4 gWorld : World;
float gTime : Time;

struct VsInput
{
    float4 Position : POSITION0;
    float3 Color : Normal;
};
	
struct VsOutput
{
    float4 Position : POSITION;
    float4 Color : COLOR0;
};	
	
VsOutput mainVS(VsInput input)
{
    VsOutput output;
    
    output.Position = mul(input.Position, gWorldViewProj);
    output.Color = float4(input.Color-(sin(gTime*2)*0.1), 0.7f);
    
    return output;
}

float4 mainPS(float2 uv: TEXCOORD0,  float4 color : COLOR0) : COLOR 
{
	return color;
}

technique Default
{
	pass p0 
	<
		bool IsTransparent=true;
	>
	{
		AlphaTestEnable = false;
		AlphaBlendEnable = true;
		ZEnable = true;
		ZWriteEnable = false;
		
		BlendOp = add;
		DestBlend = INVSRCALPHA;
		SrcBlend = SRCALPHA;
		
		CullMode = None;
		
		VertexShader = compile vs_2_0 mainVS();
		PixelShader = compile ps_2_0 mainPS();
	}
}
