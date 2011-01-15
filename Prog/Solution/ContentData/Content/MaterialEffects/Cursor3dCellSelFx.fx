float4x4 gWorldViewProj : WorldViewProjection;
float4x4 gWorld : World;
float gTime : Time;

float3 gColor;

texture2D gDiffuseMap;
sampler2D gDiffuseMapSampler = sampler_state 
	{
	  Texture = <gDiffuseMap>;
	  MinFilter = Linear;
	  MagFilter = Linear;
	  MipFilter = Linear;
	  AddressU = Wrap;
	  AddressV = CLAMP;
	};
	
struct VsInput
{
    float4 Position : POSITION0;
};
	
struct VsOutput
{
    float4 Position : POSITION;
    float4 Color : COLOR0;
};	
	
VsOutput mainVS(VsInput input)
{
    VsOutput output;
    
    input.Position.z*=0.5f;
    output.Position = mul(input.Position, gWorldViewProj);
    output.Color = float4(gColor, sin(gTime*2)*0.3+0.5);
    
    return output;
}

float4 mainPS(float4 color : COLOR0) : COLOR 
{
	return  color;
}



technique Default
{
	pass p0 
	<
		bool IsTransparent=true;
		string MainTexture = "gDiffuseMap";  
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
