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
};	
	
VsOutput mainVS(VsInput input)
{
    VsOutput output;
    
    float4 pos = float4(input.Position.xy * 2000, input.Position.zw);
    pos.xyz+=gEyePosition;
    
    output.Position = mul(pos, gWorldViewProj);
    output.Normal = normalize(mul(input.Normal, gWorld));
    output.Uv = input.Uv*40 + gTime/100;
    
    return output;
}

float4 mainPS(float2 uv: TEXCOORD0, float3 normal : TEXCOORD01) : COLOR 
{
	float4 color  = tex2D(gDiffuseMapSampler, uv);
	return color;   
}


//Must be present although it is not used
technique Default
{
	pass p0 
	<
		bool IsTransparent=false;
		string MainTexture = "gDiffuseMap";  
	>
	{
		AlphaTestEnable = false;
		AlphaBlendEnable = false;
		ZEnable = false;
		ZWriteEnable = false;
		
		VertexShader = compile vs_2_0 mainVS();
		PixelShader = compile ps_2_0 mainPS();
	}
}
technique SkyPlaneScenePass
{
	pass p0 
	<
		bool IsTransparent=false;
		string MainTexture = "gDiffuseMap";  
	>
	{
		AlphaTestEnable = false;
		AlphaBlendEnable = false;
		ZEnable = false;
		ZWriteEnable = false;
		
		VertexShader = compile vs_2_0 mainVS();
		PixelShader = compile ps_2_0 mainPS();
	}
}
