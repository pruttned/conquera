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
float3 gSunLightDirection  = float3(-0.2857143, -0.4285714, 0.0);  //treba spravit SunLightDirection auto param  - to budu vlastne s polu s kamerami per scene auto params
															//z = 0 !! - treba
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
	
	texture2D gWallLightMap;
sampler2D gWallLightMapSampler = sampler_state 
	{
	  Texture = <gWallLightMap>;
	  MinFilter = Linear;
	  MagFilter = Linear;
	  MipFilter = Linear;
	  AddressU = Wrap;
	  AddressV = Wrap;
	};

	
struct VsOutput
{
    float4 pos : POSITION;
    float2 uv : TEXCOORD0;
    float3 posWorld : TEXCOORD1;
    float4 normal : TEXCOORD2;
};	
	
VsOutput mainVS(float4 pos: POSITION, float4 normal: NORMAL, float2 uv: TEXCOORD0)
{
	VsOutput output = (VsOutput)0;
	
	//pos.y += 0.01f;
	//pos += (float4(normal.xyz, 0) * sin(gTime)/2); 
	
//	float sinTime = sin(gTime*0.8)*0.2;
//	pos.z += sinTime * pos.z;

	
	output.pos = mul(pos, gWorldViewProj);
	output.uv = uv*2;
	
	output.normal = normalize(mul(normal, gWorld));
	output.posWorld = mul(pos, gWorld);

	return output;
}

float4 mainPS(float2 uv: TEXCOORD0, float4 posWorld : TEXCOORD1, float4 normal : TEXCOORD2) : COLOR 
{
	float4 diffColor = tex2D(gDiffuseMapSampler, uv);
	diffColor.rgb =  saturate(diffColor.rgb * dot(gSunLightDirection, normal)*2 + float3(0.02,0.01,0));
	float3 light = tex2D(gWallLightMapSampler, float2(1-posWorld.z/5+1, 0)).xyz * (sin(gTime+posWorld.x*posWorld.y)*0.2+0.8);
	diffColor.rgb += light*(light * float3(1, 1, 0)+float3(0.8, 0.1, 0.1) ) ;
	
	//diffColor.rgb = normal.xyz;

//diffColor = lerp(float4(0,0,0,1), diffColor, saturate(posWorld.z/5));
//diffColor.a = saturate(posWorld.z/2+1);
//   return float4(0,0,0,0);


	//float3 eyeDir = normalize(gEyePosition - posWorld.xyz);   
	//float sinTime = (sin(gTime*0.7)*0.5+1) *0.2;
	//float4 baseColor = float4(float4(0.7, 0.6, 1.0, 0.8) + sin(posWorld.xyz)*0.3, 0.8);
	//float4 baseColor =  ;
	//float4 outColor = baseColor * saturate(1-dot(eyeDir,normalize(normal)) + sinTime);
	//outColor.a *= 1+posWorld.z*(sin(gTime*0.7))*0.5;
	//outColor.a += 0.2;
	//return diffColor  * lerp(outColor*2, 1, sin(gTime)*0.4+0.5);
	
	return diffColor;
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
		
	//	BlendOp = add;

		AlphaFunc = Greater; 
		AlphaRef = 0x000001;

		DestBlend = INVSRCALPHA;
		SrcBlend = SRCALPHA;
		
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
		CullMode = none;
		
		VertexShader = compile vs_2_0 mainVS();
		PixelShader = compile ps_2_0 mainPS();
	}
}