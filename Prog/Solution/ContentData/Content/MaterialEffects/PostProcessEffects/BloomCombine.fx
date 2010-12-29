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

texture2D gBluredBrightnessMap;
sampler2D gBluredBrightnessMapSampler = sampler_state 
	{
	  Texture = <gBluredBrightnessMap>;
	  MinFilter = Linear;
	  MagFilter = Linear;
	  MipFilter = Linear;
	  //AddressU = Clamp;
	  //AddressV = Clamp;
	};

	
	
float4 mainPS(float2 pos : POSITION, float2 uv : TEXCOORD0) : COLOR 
{
    float4 bloom = tex2D(gBluredBrightnessMapSampler, uv) * 1.1 + tex2D(gScreenMapSampler, uv)*0.00001;
    float4 base = tex2D(gScreenMapSampler, uv) ;//* 1.0;
    
    return lerp(base, bloom, 0.4);
    //base *= (1 - saturate(bloom));
    
    //return base + bloom;
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



