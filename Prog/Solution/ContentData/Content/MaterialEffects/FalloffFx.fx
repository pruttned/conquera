float4x4 gWorldViewProj : WorldViewProjection;
float4x4 gWorld : World;
float3 gEyePosition : EyePosition;
	
struct VsOutput
{
    float4 pos : POSITION;
    float4 posWorld : TEXCOORD0;
    float3 normal : TEXCOORD1;
};	
	
VsOutput mainVS(float4 pos: POSITION, float4 normal: NORMAL)
{
	VsOutput output = (VsOutput)0;
	output.posWorld = mul(pos, gWorld);
	output.normal = normal;
	output.pos = mul(pos, gWorldViewProj);
	return output;

}

float4 mainPS(float4 posWorld : TEXCOORD0, float3 normal : TEXCOORD1) : COLOR 
{
	float3 eyeDir = normalize(gEyePosition - posWorld.xyz);   
	float4 outColor = float4(0.8, 0.8, 1.0, 1) * saturate(1-dot(eyeDir,normalize(normal)) + 0.3);
	//outColor.a = 0.5;
	//outColor.a += 0.2;
	return outColor;
}

technique Default
{
	pass p0 
	<
		bool IsTransparent=true;  
	>
	{
		AlphaBlendEnable = true;
		AlphaTestEnable = true;
	
		BlendOp = add;
		DestBlend = INVSRCALPHA;
		SrcBlend = SRCALPHA;
	
		ZEnable = true;
		ZWriteEnable = true;
		
		VertexShader = compile vs_2_0 mainVS();
		PixelShader = compile ps_2_0 mainPS();
	}
}
