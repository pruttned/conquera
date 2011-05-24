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
float3 gColor = float3(0.6,0.0,0.6);
//float3 gSunLightDirection  = float3(-0.2310634, 0.06931902, 0.9704662);  //treba spravit SunLightDirection auto param  - to budu vlastne s polu s kamerami per scene auto params
float3 gSunLightDirection  = float3(-0.2857143, -0.4285714, 0.8571429);  //treba spravit SunLightDirection auto param  - to budu vlastne s polu s kamerami per scene auto params
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
    float2 Depth : TEXCOORD2;
    float4 posWorld : TEXCOORD3;
};	
	
VsOutput mainVS(VsInput input)
{
    VsOutput output;
    
    output.Position = mul(input.Position, gWorldViewProj);
	output.posWorld = mul(input.Position, gWorld);
    output.Normal = normalize(mul(input.Normal, gWorld));
	output.Depth = float2(output.Position.z, output.Position.w);
    output.Uv = input.Uv;
    
    return output;
}

struct PsOut
{
    half4 Color : COLOR0;
    half4 Normal : COLOR1;
    half4 Depth : COLOR2;
};

PsOut mainPS(float2 uv: TEXCOORD0, float3 normal : TEXCOORD01, float2 depth : TEXCOORD02, float4 posWorld : TEXCOORD03) : COLOR 
{
	PsOut output;

	float a = 0.3*sin(gTime*2+posWorld.x+posWorld.y*2)-0.1;
	output.Color  = (tex2D(gDiffuseMapSampler, uv) + float4(gColor, 0))+ float4(a,a,a,0);
    output.Normal = float4(0.5f * (normalize(normal) + 1.0f), 0.8); //a = diff color power
    output.Depth = depth.x / depth.y;
	
	return output;
}


float4 ShadowCasterPs(float2 uv: TEXCOORD0) : COLOR 
{
	//return tex2D(gDiffuseMapSampler, uv);   
	return float4(0.6f ,0.6f ,0.6f , tex2D(gDiffuseMapSampler, uv).a);
}

technique Default
{
	pass p0 
	<
		bool IsTransparent = false;
		bool ReceivesLight = true;
		string MainTexture = "gDiffuseMap";  
	>
	{
		AlphaTestEnable = false;
		AlphaBlendEnable = false;
		AlphaFunc = Greater; 
		AlphaRef = 0x000001;
		
		ZEnable = true;
		ZWriteEnable = true;
		CullMode = CCW;
		ZFUNC = lessequal;
		
		VertexShader = compile vs_2_0 mainVS();
		PixelShader = compile ps_2_0 mainPS();
	}
}

technique ShadowPass
{
	pass p0 
	<
		bool IsTransparent=false;
		string MainTexture = "gDiffuseMap";  
	>
	{
		AlphaTestEnable = true;
		AlphaBlendEnable = false;
		AlphaFunc = Greater; 
		AlphaRef = 0x000080;

		ZEnable = true;
		ZWriteEnable = true;
		CullMode = CCW;
		ZFUNC = lessequal;
		
		
		VertexShader = compile vs_2_0 mainVS();
		PixelShader = compile ps_2_0 ShadowCasterPs();
	}
}