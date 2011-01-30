float4x4 gWorldViewProj : WorldViewProjection;
float4x4 gWorld : World;
float3 gEyePosition : EyePosition; //cameraposition
float gTime : Time;
float4x4 gReflectionViewProj;

texture2D gNoiseMap;
sampler2D gNoiseMapSampler = sampler_state 
	{
	  Texture = <gNoiseMap>;
	  MinFilter = Linear;
	  MagFilter = Linear;
	  MipFilter = Linear;
	  AddressU = Wrap;
	  AddressV = Wrap;
	};


texture2D gReflectionMap : RenderTargetMap
	<string TargetName = "ReflectionMap";>;
sampler2D gReflectionMapSampler = sampler_state 
	{
	  Texture = <gReflectionMap>;
	  MinFilter = Linear;
	  MagFilter = Linear;
	  MipFilter = Linear;
	AddressU  = CLAMP;
    AddressV  = CLAMP;
	};

	
	texture2D gBwNoiseMap;
sampler2D gBwNoiseMapSampler = sampler_state 
	{
	  Texture = <gBwNoiseMap>;
	  MinFilter = Linear;
	  MagFilter = Linear;
	  MipFilter = Linear;
	AddressU  = Wrap;
    AddressV  = Wrap;
	};
	
textureCUBE gEnvMap;
samplerCUBE gEnvMapSampler = sampler_state 
	{
	  Texture = <gEnvMap>;
	  MinFilter = Linear;
	  MagFilter = Linear;
	  MipFilter = Linear;
	  AddressU = Wrap;
	  AddressV = Wrap;
	  //AddressW = Wrap;
	};
	


	
struct VsOutput
{
    float4 pos : POSITION;
    float4 reflectionMapUv : TEXCOORD0;
    float2 uvNoise : TEXCOORD1;
    float2 uvNoise2 : TEXCOORD2;
    float3 viewVec : TEXCOORD3;
    float3 posWorld : TEXCOORD4;
    float2 bwUvNoise : TEXCOORD5;
};	

VsOutput mainVS(float4 pos: POSITION, float4 normal: NORMAL, float2 uv: TEXCOORD0)
{
	VsOutput output = (VsOutput)0;
	//pos.y -= 0.01;
	//output.pos = mul(float4(pos.xyz, 1.0f), gWorldViewProj);
	//output.pos = mul(pos, gWorldViewProj);

	//output.reflectionMapSamplingPos = pos;
	
	output.posWorld = mul(pos, gWorld);
	
	
	output.pos = output.reflectionMapUv = mul(float4(pos.xyz, 1.0f), gWorldViewProj);
	
	uv = output.posWorld *0.1;

	
	
	//outVS.pos = outVS.uv = mul(float4(posL, 1.0f), gWorldViewProj);

	output.bwUvNoise = uv * 2;
	output.uvNoise = uv + float2(-gTime * 0.008, -gTime * 0.03);
	output.uvNoise2 = uv * 2 + float2(gTime * 0.05, gTime * 0.007);

	output.viewVec = reflect(float4(0, 0, -1, 0), mul(pos.xyz, gWorld) - gEyePosition);

    //output.reflectionMapSamplingPos = mul(pos, mul(gWorld, gReflectionViewProj));
  //  output.reflectionMapSamplingPos = mul(pos, mul(gWorld, gReflectionViewProj));
    
  //  output.reflectionMapSamplingPos.xy = uv;
	
	

	return output;
}

float4 mainPS(float4 reflectionMapUv : TEXCOORD0, float2 uvNoise : TEXCOORD1, float2 uvNoise2 : TEXCOORD2, float3 viewVec : TEXCOORD3,
    float3 posWorld : TEXCOORD4, float2 bwUvNoise : TEXCOORD5) : COLOR 
{
//return float4(0,0,0,0);

return  texCUBE(gEnvMapSampler, normalize(viewVec)) * float4(1,0.2,0.2, 1);

//	return tex2D(gBwNoiseMapSampler, uvNoise)* float4(1,0.2,0.2, 1); // * tex2D(gBwNoiseMapSampler, uvNoise2)* float4(0.8,0.7,0.1, 1);
	
	float3 noise = 2 * ((tex2D(gNoiseMapSampler, uvNoise)+tex2D(gNoiseMapSampler, uvNoise2))*0.5) - 1;


   noise.xz *= 0.15;
   // Make sure the normal always points upwards
   // note that Ogres y axis is vertical (RM Z axis is vertical)
   noise.y = 0.3 * abs(noise.y) ;
   // Offset the surface normal with the bump

   float3 normal = normalize(float3(0,0,1) + noise);

   // Find the reflection vector
   float3 normView = normalize(viewVec);
   float3 reflVec = reflect(normView, normal);
   // Ogre has z flipped for cubemaps
   reflVec.z = -reflVec.z;
   float4 refl = texCUBE(gEnvMapSampler, reflVec);


   
   	float4 reflectionMapUv2 = reflectionMapUv +  float4(noise, 0);

    float4 reflColor = tex2D(gReflectionMapSampler, 
		float2(-0.5f*reflectionMapUv2.x/reflectionMapUv2.w + 0.5f, 
		-0.5f*reflectionMapUv2.y/reflectionMapUv2.w + 0.5f));   

	refl*=0.9;
//	refl = (refl * reflColor);//*0.5f;
   

   //return lerp(float4(0.4, 0.55, 0.4, 1), refl, 0.3);
   
   
   
   return float4(0.0, 0.0, 0.0, 1);
	
	
	



}

technique Default
{
	pass p0 
	<
		bool IsTransparent=false;
		string MainTexture = "gEnvMap";  
	>
	{
		AlphaBlendEnable = false;
		AlphaTestEnable = false;
	
		VertexShader = compile vs_2_0 mainVS();
		PixelShader = compile ps_2_0 mainPS();
	}
}