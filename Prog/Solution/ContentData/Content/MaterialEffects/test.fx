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
	
texture2D gNormalMap;
sampler2D gNormalMapSampler = sampler_state 
	{
	  Texture = <gNormalMap>;
	  MinFilter = Linear;
	  MagFilter = Linear;
	  MipFilter = Linear;
	  AddressU = Wrap;
	  AddressV = Wrap;
	};
	
float4 gLightDir = float4(1.0, 0.2, 0.3, 1.0); 
	
	
struct VsOutput
{
    float4 pos : POSITION;
    float2 uv : TEXCOORD0;
    float4 posWorld : TEXCOORD1;
    float4 normal : TEXCOORD2;
};	
	
VsOutput mainVS(float4 pos: POSITION, float4 normal: NORMAL, float2 uv: TEXCOORD0)
{
	VsOutput output = (VsOutput)0;
	output.pos = mul(pos, gWorldViewProj);
	output.uv = uv;
	output.posWorld = mul(pos, gWorld);
	output.normal = normalize(normal);

	return output;

}

float4 mainPS(float2 uv: TEXCOORD0, float4 posWorld : TEXCOORD1, float4 normal : TEXCOORD2) : COLOR 
{
float sinTime = sin(gTime);

float4 lightPos = float4(gEyePosition ,1.0);

float distanceToLight = length(lightPos - posWorld);
float lightPower =  saturate(100 / distanceToLight);


	//float4 normal = tex2D(gNormalMapSampler, uv);
	//normal = normalize(normal);
   
   normal = normalize(normal * tex2D(gNormalMapSampler, uv));
   
    float diffuseLightingFactor = saturate(dot(normalize(lightPos - posWorld), normal)) * lightPower;
    
    
	float4 diffColor = tex2D(gDiffuseMapSampler, uv);
   
  return float4((diffColor * diffuseLightingFactor).xyz, 1);
  //return float4(diffColor.xyz, 0.8);
}

technique Default
{
	pass p0 
	<
		bool IsTransparent=false;
		string MainTexture = "gDiffuseMap";  
	>
	{
//		AlphaBlendEnable = true;
//		AlphaTestEnable = true;
//		ZEnable = true;
//		ZWriteEnable = false;
		
//		BlendOp = Add;
//		DestBlend = INVSRCALPHA;
//		SrcBlend = SRCALPHA;

		AlphaBlendEnable = false;
		AlphaTestEnable = false;
		ZEnable = true;
		ZWriteEnable = true;
		
	
		VertexShader = compile vs_2_0 mainVS();
		PixelShader = compile ps_2_0 mainPS();
	}
}
