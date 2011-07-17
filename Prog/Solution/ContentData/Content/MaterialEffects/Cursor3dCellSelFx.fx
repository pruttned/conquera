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

float3 gColor;

texture2D gDiffuseMap;
sampler2D gDiffuseMapSampler = sampler_state 
	{
	  Texture = <gDiffuseMap>;
	  MinFilter = Linear;
	  MagFilter = Linear;
	  MipFilter = Linear;
	  AddressU = Wrap;
	  AddressV = CLAMP;
	};
	
struct VsInput
{
    float4 Position : POSITION0;
};
	
struct VsOutput
{
    float4 Position : POSITION;
    float4 Color : COLOR0;
};	
	
VsOutput mainVS(VsInput input)
{
    VsOutput output;
    
    input.Position.z*=0.5f;
    output.Position = mul(input.Position, gWorldViewProj);
    output.Color = float4(gColor, sin(gTime*2)*0.3+0.5);
    
    return output;
}

float4 mainPS(float4 color : COLOR0) : COLOR 
{
	return  color;
}



technique Default
{
	pass p0 
	<
		bool IsTransparent=true;
		string MainTexture = "gDiffuseMap";  
	>
	{
		AlphaTestEnable = false;
		AlphaBlendEnable = true;
		
		BlendOp = add;
		DestBlend = INVSRCALPHA;
		SrcBlend = SRCALPHA;

		ZEnable = true;
		ZWriteEnable = false;
		CullMode = None;
		ZFUNC = lessequal;
		
		
		VertexShader = compile vs_2_0 mainVS();
		PixelShader = compile ps_2_0 mainPS();
	}
}
