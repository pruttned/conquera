float4x4 gWorldViewProj : WorldViewProjection;
float4x4 gWorld : World;
float gTime: Time;

texture2D gDiffuseMap;
sampler2D gDiffuseMapSampler = sampler_state 
	{
	  Texture = <gDiffuseMap>;
	  MinFilter = Linear;
	  MagFilter = Linear;
	  MipFilter = Linear;
	  AddressU = WRAP;
	  AddressV = WRAP;
	};
	
sampler2D gDiffuseMapSamplerA = sampler_state 
	{
	  Texture = <gDiffuseMap>;
	  MinFilter = Linear;
	  MagFilter = Linear;
	  MipFilter = Linear;
	  AddressU = CLAMP;
	  AddressV = CLAMP;
	};	
	
struct VsInput
{
    float4 Position : POSITION0;
    float3 Normal : NORMAL0;
    float2 Uv : TEXCOORD0;
};
	
struct VsOutput
{
    float4 Position : POSITION;
    float2 Uv : TEXCOORD0;
    float3 Normal : TEXCOORD1;
	float2 Uv2 : TEXCOORD3;
	float2 UvA : TEXCOORD4;

};	
	
VsOutput mainVS(VsInput input)
{
    VsOutput output;
    
    output.Position = mul(input.Position, gWorldViewProj);

    output.Normal = normalize(mul(input.Normal, gWorld));

    float3 worlPos = mul(input.Position, gWorld);
    
    output.UvA = input.Uv;
    output.Uv = input.Uv;
    output.Uv2 = input.Uv;
	float x = gTime/10;


	float r =fmod(worlPos.x * worlPos.y, 1);

	output.Uv.x += x;
	output.Uv2.y = 1 - output.Uv2.y + 2*x ;
	output.Uv2.x -= x;
	output.Uv2.x = 1- output.Uv2.x;
    
    return output;
}

float4 mainPS(float2 uv: TEXCOORD0, float3 normal : TEXCOORD01, 
	float2 uv2 : TEXCOORD3, float2 uvA : TEXCOORD4) : COLOR 
{
return float4(0,0,0,0);
	float x = gTime;
	float4 color  = tex2D(gDiffuseMapSampler, uv) + tex2D(gDiffuseMapSampler, uv2);
	color *= float4(1,0.5,0.7,1);
 	color.a = tex2D(gDiffuseMapSamplerA, uvA).a * 0.5;
	return color;   
}

technique Default
{
	pass p0 
	<
		bool IsTransparent=true;
		string MainTexture = "gDiffuseMap";  
	>
	{
		AlphaTestEnable = true;
		AlphaBlendEnable = true;
		ZEnable = true;
		ZWriteEnable = false;
			
		BlendOp = add;

		DestBlend = INVSRCALPHA;
		SrcBlend = SRCALPHA;

		AlphaFunc = Greater; 
		AlphaRef = 0x000005;
		
		VertexShader = compile vs_2_0 mainVS();
		PixelShader = compile ps_2_0 mainPS();
	}
}

