float4x4 gWorldViewProj : WorldViewProjection;
float4x4 gWorld : World;
float gTime : Time;
float3 gEyePosition : EyePosition;

float4x4 gLightViewProj : ScenePassCameraViewProjection
	<string ScenePass = "ShadowPass";>;

texture2D gDiffuseMap;
sampler2D gDiffuseMapSampler = sampler_state 
	{
	  Texture = <gDiffuseMap>;
	  MinFilter = Linear;
	  MagFilter = Linear;
	  MipFilter = Linear;
	  AddressU = Wrap;
	  AddressV = Wrap;
	};
	
texture2D gShadowMap : RenderTargetMap
	<string TargetName = "ShadowMap";>;
sampler2D gShadowMapSampler = sampler_state 
	{
	  Texture = <gShadowMap>;
	  MinFilter = linear;
	  MagFilter = linear;
	  MipFilter = linear;
	  AddressU = Clamp;
	  AddressV = Clamp;
	};	
	
struct VsOutput
{
    float4 pos : POSITION;
    float2 uv : TEXCOORD0;
    float4 worldPos : TEXCOORD1;
};	
	
VsOutput mainVS(float4 pos: POSITION, float4 normal: NORMAL, float2 uv: TEXCOORD0)
{
	VsOutput output = (VsOutput)0;
//	pos.y -= 0.01;
	output.pos = mul(pos, gWorldViewProj);
	//output.uv = mul(pos, gWorld).xy / 6;
	output.uv = uv;
	output.worldPos = mul(pos, gWorld);

	return output;
}

float4 mainPS(float2 uv: TEXCOORD0, float4 posWorld : TEXCOORD1) : COLOR 
{
   float4 lightingPosition = mul(posWorld, gLightViewProj);
    
  
    // Find the position in the shadow map for this pixel
    float2 shadowTexCoord = 0.5 * lightingPosition.xy / 
                            lightingPosition.w + float2( 0.5, 0.5 );
    shadowTexCoord.y = 1.0f - shadowTexCoord.y;
    
    float4 shadowColor  = float4(tex2D(gShadowMapSampler, shadowTexCoord).rgb, 1);
	return tex2D(gDiffuseMapSampler, uv) * shadowColor;
}



float4 mainPS2(float2 uv: TEXCOORD0, float4 posWorld : TEXCOORD1) : COLOR 
{
	return float4(0,0,0,1);
}

technique Default
{
	pass p0 
	<
		bool IsTransparent=false;
		string MainTexture = "gDiffuseMap";  
	>
	{
		AlphaBlendEnable = false;
		AlphaTestEnable = false;
		ZEnable = true;
		ZWriteEnable = true;
		CullMode = CCW;
	
		VertexShader = compile vs_2_0 mainVS();
		PixelShader = compile ps_2_0 mainPS();
	}
}

technique WaterReflectionPass
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
		CullMode = None;
	
		VertexShader = compile vs_2_0 mainVS();
		PixelShader = compile ps_2_0 mainPS2();
	}
}