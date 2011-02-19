//////////////////////////////////////////////////////////////////////
//  Copyright (C) 2010 by Conquera Team
//  Part of the Conquera Project
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 2 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
////////////////////////////////////////////////////////////////////////

float4x4 gWorldViewProj : WorldViewProjection;
float4x4 gWorld : World;
float3 gEyePosition : EyePosition;
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
	AddressU  = Mirror;
    AddressV  = Mirror;
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

	output.viewVec = reflect(float4(0, -1, 0, 0), mul(pos.xyz, gWorld) - gEyePosition);

    //output.reflectionMapSamplingPos = mul(pos, mul(gWorld, gReflectionViewProj));
  //  output.reflectionMapSamplingPos = mul(pos, mul(gWorld, gReflectionViewProj));
    
  //  output.reflectionMapSamplingPos.xy = uv;
	
	

	return output;
}

float4 mainPS(float4 reflectionMapUv : TEXCOORD0, float2 uvNoise : TEXCOORD1, float2 uvNoise2 : TEXCOORD2, float3 viewVec : TEXCOORD3,
    float3 posWorld : TEXCOORD4, float2 bwUvNoise : TEXCOORD5) : COLOR 
{


//	float3 skyBoxUvw = float3(viewVec.x, viewVec.y *0.4 , viewVec.z) * tex2D(gNoiseMapSampler, uvNoise);
//	float4 waterColor = lerp(float4(0.1, 0.144, 0.295, 0.85), texCUBE(gEnvMapSampler, skyBoxUvw), 0.5);
	//float4 waterColor = float4(0.1, 0.144, 0.295, 0.85);//lerp(float4(0.1, 0.144, 0.295, 0.85), texCUBE(gEnvMapSampler, skyBoxUvw), 0.5);
	
	
	float4 lightPos = float4(6,3,6,1);

	//float3 normal = float3(0,1,0) + (2 * (tex2D(gNoiseMapSampler, uvNoise) * tex2D(gNoiseMapSampler, uvNoise2)) - 1)/3;

	float3 normal = normalize(0.5 * (tex2D(gNoiseMapSampler, uvNoise) + tex2D(gNoiseMapSampler, uvNoise2)));


	float3 skyBoxUvw = float3(viewVec.x, viewVec.y *0.4 , viewVec.z) * normal;
	float4 waterColor = float4(0.4, 0.55, 0.4, 1);
	waterColor = lerp(waterColor, texCUBE(gEnvMapSampler, skyBoxUvw), 0.5);


//	waterColor = (float4(0.1, 0.144, 0.295, 1)+0.3)*3 ;
    float diffuseLightingFactor = saturate(dot(normalize(float3(1,1,0.3)), normal));
	waterColor *= diffuseLightingFactor; 

//waterColor = float4(0.4, 0.55, 0.4, 1);
	waterColor *= (diffuseLightingFactor)  + float4(0.4, 0.55, 0.4, 1) ; 
	
//	return tex2D(gDiffuseMapSampler, uvNoise) * tex2D(gDiffuseMapSampler, uvNoise2);   


	//float4 reflectionMapUv2 = reflectionMapUv+ 0.5f * ((tex2D(gNoiseMapSampler, uvNoise) * tex2D(gNoiseMapSampler, uvNoise2))*2.0f - 1.0f); 
	float4 reflectionMapUv2 = reflectionMapUv+  0.5f * (tex2D(gNoiseMapSampler, uvNoise) * 2 - 1);

    float4 reflColor = tex2D(gReflectionMapSampler, 
		float2(-0.5f*reflectionMapUv2.x/reflectionMapUv2.w + 0.5f, 
		-0.5f*reflectionMapUv2.y/reflectionMapUv2.w + 0.5f));   


//	reflectionMapUv +=  0.5f * (tex2D(gNoiseMapSampler, uvNoise) * 2 - 1);

    float4 reflColor2 = tex2D(gReflectionMapSampler, 
		float2(-0.5f*reflectionMapUv.x/reflectionMapUv.w + 0.5f, 
		-0.5f*reflectionMapUv.y/reflectionMapUv.w + 0.5f));   

	//reflColor *=  reflColor2;
	
  
float3    lightVecW = float3(2.6f, -1.0f, -1.5f);
    
//	float3 sunlight = 1.5f * pow(saturate(dot(R, lightVecW)), 250) * float4(1.0f, 0.8f, 0.4f, 1.0f);
    
    //return lerp (reflColor, float4(0.5f, 0.79f, 0.75f, 1.0f), 0.7);
    
    //float4 color = lerp (reflColor, waterColor, 0.8);
    
    
    
    float3 reflectionVector = -reflect(normalize(float3(0,1,0)), normal);
	float specular = 1.5 * pow(1.01 * dot(normalize(reflectionVector), normalize(viewVec)), 250);

//    float3 reflectionVector2 = -reflect(normalize(float3(1,1,-1)), normal);
//	float specular2 = 1.5 * pow(1.01 * dot(normalize(reflectionVector2), normalize(viewVec)), 100);


//float4 deepWater = tex2D(gBwNoiseMapSampler, bwUvNoise+normal/10) + float4(0.1, 0.15, 0.3, 1);
skyBoxUvw.y *= 0.3;
//float4 deepWater = texCUBE(gEnvMapSampler2, skyBoxUvw)/2 * (tex2D(gBwNoiseMapSampler, bwUvNoise+normal/10) + float4(0.01, 0.45, 0.1, 1));


float4 deepWater = /*tex2D(gBottomMapSampler, posWorld.xz / 5)/5 * */tex2D(gBwNoiseMapSampler, bwUvNoise+normal/10)/5 + float4(0.01, 0.28, 0.2, 1);
//return posColor;

//    return lerp (reflColor, deepWater * waterColor /*+ specular*/ , 0.94);
    return lerp (reflColor, deepWater * waterColor /*+ specular*/ , 0.8);
    
    //return lerp(waterColor, reflColor) ;

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