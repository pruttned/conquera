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
float gTime : Time;

texture2D gLavaNoiseMap;
sampler2D gLavaNoiseMapSampler = sampler_state 
	{
	  Texture = <gLavaNoiseMap>;
	  MinFilter = Linear;
	  MagFilter = Linear;
	  MipFilter = Linear;
	  AddressU = Wrap;
	  AddressV = Wrap;
	};
	
texture2D gLavaColdMap;
sampler2D gLavaColdMapSampler = sampler_state 
	{
	  Texture = <gLavaColdMap>;
	  MinFilter = Linear;
	  MagFilter = Linear;
	  MipFilter = Linear;
	  AddressU = Wrap;
	  AddressV = Wrap;
	};
	
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
		
struct VsInput
{
    float4 Position : POSITION0;
    float2 Uv : TEXCOORD0;
};
	
struct VsOutput
{
    float4 Position : POSITION;
    float2 Uv : TEXCOORD0;
    float2 Uv2 : TEXCOORD1;
    float2 Uv3 : TEXCOORD2;
};	


VsOutput mainVS(VsInput input)
{
    VsOutput output;
    
    output.Position = mul(input.Position, gWorldViewProj);

	output.Uv = input.Position/2 + float2(gTime * 0.032 , gTime * 0.02);
	output.Uv2 = input.Position/6+ float2(gTime * 0.032 , gTime * 0.02);
	output.Uv3 = input.Position/ 8 + float2(-gTime * 0.02, -gTime * 0.032);
    
    return output;
}

float4 mainPS(float2 uv: TEXCOORD0, float2 uv2 : TEXCOORD01, float2 uv3 : TEXCOORD02) : COLOR0
{
	float4 color  = 
		float4(0.8, 0.1, 0.1 ,1) + 
		(tex2D(gLavaNoiseMapSampler, uv2)) * float4(1.5, 1.2, 0, 1);
		
	color *= tex2D(gLavaColdMapSampler, uv);
	color.rg += tex2D(gNoiseMapSampler, uv2).rg * tex2D(gNoiseMapSampler, uv3).rg;
	color.a = 1;

	return color;
}

technique Default
{
	pass p0 
	<
		bool IsTransparent=false;
		string MainTexture = "gLavaNoiseMap";  
	>
	{
		AlphaTestEnable = false;
		AlphaBlendEnable = false;
		
		ZEnable = true;
		ZWriteEnable = true;
		CullMode = None;
		ZFUNC = lessequal;
		
		VertexShader = compile vs_2_0 mainVS();
		PixelShader = compile ps_2_0 mainPS();
	}
}
