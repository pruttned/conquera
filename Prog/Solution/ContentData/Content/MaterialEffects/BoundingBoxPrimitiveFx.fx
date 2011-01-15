float4x4 gViewProj : ViewProjection;
float3 gColor;
float3 gScale;
float3 gPosition;

struct VsOutput
{
    float4 pos : POSITION;
};	
	
VsOutput mainVS(float4 pos: POSITION)
{
    VsOutput output = (VsOutput)0;
      output.pos = pos * float4(gScale, 1);
    output.pos += float4(gPosition, 0);
         
	output.pos = mul(output.pos, gViewProj);

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

