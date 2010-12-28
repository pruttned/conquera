texture2D gScreenMap;
sampler2D gScreenMapSampler = sampler_state 
	{
	  Texture = <gScreenMap>;
	  MinFilter = Point;
	  MagFilter = Point;
	  MipFilter = Point;
	  //AddressU = Clamp;
	  //AddressV = Clamp;
	};
	
float4 mainPS(float2 pos : POSITION, float2 uv : TEXCOORD0) : COLOR 
{
	return  1 - tex2D(gScreenMapSampler, uv);
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
