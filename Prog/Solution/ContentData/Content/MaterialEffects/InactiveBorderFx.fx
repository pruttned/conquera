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
    float2 Uv : TEXCOORD0;
};
	
struct VsOutput
{
    float4 Position : POSITION;
    float2 Uv : TEXCOORD0;
    float4 Color : COLOR0;
};	
	
VsOutput mainVS(VsInput input)
{
    VsOutput output;
    
    output.Position = mul(input.Position, gWorldViewProj);
    output.Uv = input.Uv;
    output.Color = float4(gColor, sin(gTime*3)*0.2+0.4);
    
    return output;
}

float4 mainPS(float2 uv: TEXCOORD0,  float4 color : COLOR0) : COLOR 
{
	return tex2D(gDiffuseMapSampler, uv) * color;
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
