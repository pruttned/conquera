texture2D gScreenMap;
sampler2D gScreenMapSampler = sampler_state 
	{
	  Texture = <gScreenMap>;
	  MinFilter = linear;
	  MagFilter = linear;
	  MipFilter = linear;
	  AddressU = Clamp;
	  AddressV = Clamp;
	};
	

float4 mainPS(float2 pos : POSITION, float2 uv : TEXCOORD0) : COLOR 
{
	return saturate((tex2D(gScreenMapSampler, uv) - 0.51) / (1 - 0.51));
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
		ZEnable = false;
		ZWriteEnable = false;
	
		PixelShader = compile ps_2_0 mainPS();
	}
}
