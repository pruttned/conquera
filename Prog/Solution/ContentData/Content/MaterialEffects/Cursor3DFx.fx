float4x4 gWorldViewProj : WorldViewProjection;
float3 gColor;

struct VsOutput
{
    float4 pos : POSITION;
};	
	
VsOutput mainVS(float4 pos: POSITION)
{
    VsOutput output = (VsOutput)0;
	output.pos = mul(pos, gWorldViewProj);

	return output;
}

float4 mainPS() : COLOR 
{
	return float4(gColor, 1); 
}

technique Default
{
	pass p0 
	<
		bool IsTransparent=false;
	>
	{
	
		AlphaBlendEnable = false;
		AlphaTestEnable = false;
		ZEnable = true;
		ZWriteEnable = true;
		
		VertexShader = compile vs_2_0 mainVS();
		PixelShader = compile ps_2_0 mainPS();
	}
}
