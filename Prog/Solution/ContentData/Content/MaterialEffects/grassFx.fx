float4x4 gViewProj : ViewProjection;
float gTime : Time;


float4 gColorVariation : ColorVariation;

#define cWorldUp float3(0,0,1)

texture2D gDiffuseMap;
sampler2D gDiffuseMapSampler = sampler_state 
	{
	  Texture = <gDiffuseMap>;
	  MinFilter = Linear;
	  MagFilter = Linear;
	  MipFilter = Linear;
	  AddressU = Clamp;
	  AddressV = Clamp;
	};

struct VsOutput
{
    float4 pos : POSITION;
    float2 uv : TEXCOORD0;
};	
	
VsOutput mainVS(float4 pos: POSITION, float2 normalizedUv: TEXCOORD0)
{
    VsOutput output = (VsOutput)0;


    float3 eyeVector = gViewProj._m02_m12_m22;

    float3 side = normalize(cross(eyeVector, cWorldUp)) ;
   
    float2 centeredUv;
    centeredUv.x = normalizedUv.x - 0.5f;
    centeredUv.y =  normalizedUv.y;
    //centeredUv.y = 1 - normalizedUv.y;
	//centeredUv*=0.2;
	//centeredUv.y = 1 -centeredUv.y;
	
	float windInfluence = 1-normalizedUv.y;
	
	float shiftedTime = gTime + fmod(pos.y*10 + pos.x * 5, 1);
	
	pos.x += sin(shiftedTime)*windInfluence*0.1f;
	pos.y += cos(shiftedTime)*windInfluence*0.1f;
	
    output.pos = mul(float4(pos + (centeredUv.x * side - centeredUv.y * cWorldUp), 1), gViewProj);
   
    output.uv = normalizedUv;
   
    return output;
}

float4 mainPS(float2 uv: TEXCOORD0, float4 vColorVariation: COLOR, float2 colorIndex : TEXCOORD1) : COLOR 
{
	return tex2D(gDiffuseMapSampler, uv);
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
		AlphaTestEnable = true;
		ZEnable = true;
		ZWriteEnable = true;
		AlphaFunc = Greater; 
		AlphaRef = 0x000080;
		
		VertexShader = compile vs_2_0 mainVS();
		PixelShader = compile ps_2_0 mainPS();
	}
}
