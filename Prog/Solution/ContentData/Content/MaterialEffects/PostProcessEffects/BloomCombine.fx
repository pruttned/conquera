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

texture2D gScreenMap;
sampler2D gScreenMapSampler = sampler_state 
	{
	  Texture = <gScreenMap>;
	  MinFilter = Point;
	  MagFilter = Point;
	  MipFilter = Point;
	  //AddressU = Clamp;
	  //AddressV = Clamp;
	};

texture2D gBluredBrightnessMap;
sampler2D gBluredBrightnessMapSampler = sampler_state 
	{
	  Texture = <gBluredBrightnessMap>;
	  MinFilter = Linear;
	  MagFilter = Linear;
	  MipFilter = Linear;
	  //AddressU = Clamp;
	  //AddressV = Clamp;
	};

struct VsOutput
{
    float4 pos : POSITION;
    float2 uv : TEXCOORD0;
};	

VsOutput mainVS(float3 position : POSITION0, float2 uv : TEXCOORD0)
{
	VsOutput output;
    output.pos = float4(position,1);
    output.uv = uv;
    return output;
}	
	
float4 mainPS(float2 pos : POSITION, float2 uv : TEXCOORD0) : COLOR 
{
    float4 bloom = tex2D(gBluredBrightnessMapSampler, uv) * 1.1 + tex2D(gScreenMapSampler, uv)*0.00001;
    float4 base = tex2D(gScreenMapSampler, uv) ;//* 1.0;
    
    return lerp(base, bloom, 0.4);
    //base *= (1 - saturate(bloom));
    
    //return base + bloom;
}

technique Default
{
	pass p0 
	<
		bool IsTransparent=false;  
	>
	{
		AlphaBlendEnable = false;
		AlphaTestEnable = false;
		ZEnable = false;
		ZWriteEnable = false;
	
		VertexShader = compile vs_2_0 mainVS();
		PixelShader = compile ps_2_0 mainPS();
	}
}



