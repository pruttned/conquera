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

// Based on http://www.catalinzima.com/tutorials/deferred-rendering-in-xna/

texture2D gDepthMap: RenderTargetMap
	<string TargetName = "ScreenDepthMap";>;
sampler2D gDepthMapSampler = sampler_state 
	{
	  Texture = <gDepthMap>;
	  MinFilter = POINT;
	  MagFilter = POINT;
	  MipFilter = POINT;
	  AddressU = CLAMP;
	  AddressV = CLAMP;
	};
	
texture2D gColorMap : RenderTargetMap
	<string TargetName = "ScreenColorMap";>;
sampler2D gColorMapSampler = sampler_state
{
    Texture = (gColorMap);
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = POINT;
    MinFilter = POINT;
    Mipfilter = POINT;
};
texture2D gLightMap : RenderTargetMap
	<string TargetName = "ScreenLightMap";>;
sampler2D gLightMapSampler = sampler_state
{
    Texture = (gLightMap);
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = POINT;
    MinFilter = POINT;
    Mipfilter = POINT;
};
texture2D gNormalMap: RenderTargetMap
	<string TargetName = "ScreenNormalMap";>;
sampler2D gNormalMapSampler = sampler_state 
	{
	  Texture = <gNormalMap>;
	  MinFilter = POINT;
	  MagFilter = POINT;
	  MipFilter = POINT;
	  AddressU = CLAMP;
	  AddressV = CLAMP;
	};

struct VsInput
{
    float3 Position : POSITION0;
    float2 Uv: TEXCOORD0;
};

struct VsOutput
{
    float4 Position : POSITION0;
    float2 Uv: TEXCOORD0;
};

struct PsOutput
{
    float4 Color : COLOR0;
    float Depth: DEPTH;
};

VsOutput mainVS(VsInput input)
{
    VsOutput output;
    output.Position = float4(input.Position,1);
    output.Uv= input.Uv;
    return output;
}

PsOutput mainPS(float2 uv: TEXCOORD0)
{
	PsOutput output;
	
    float4 normalData = tex2D(gNormalMapSampler,uv);
    float3 diffuseColor = tex2D(gColorMapSampler,uv).rgb;
    float4 light = tex2D(gLightMapSampler,uv);
    float3 diffuseLight = light.rgb;
    
    //directional light
	float3 normal = 2.0f * normalData.xyz - 1.0f;
    float3 lightVector = normalize(float3(-0.2857143, -0.4285714, 0.8571429)); //todo: param
	float NdL = max(0,dot(normal,lightVector));
    float3 dirDiffuseLight = NdL * float3(0.3,0.3,0.4); //todo: param

	diffuseLight += dirDiffuseLight;

    //output.Color =  float4(lerp((diffuseColor * diffuseLight)*1.2+diffuseColor.rgb*float3(0.6,0.4,0.1),diffuseColor, normalData.a),1);
    output.Color =  float4(lerp((diffuseColor * diffuseLight)*1.2+diffuseColor.rgb*float3(0.2,0.2,0.01),diffuseColor, normalData.a),1);
    //output.Color =  float4(lerp((diffuseColor * diffuseLight)*1.2,diffuseColor, normalData.a),1);
    output.Depth = tex2D(gDepthMapSampler,uv).r;
    
    return output;
}

technique Default
{
	pass p0 
	{
		AlphaBlendEnable = false;
		AlphaTestEnable = false;
	
		ZEnable = true;
		ZWriteEnable = true;
		CullMode = None;
		ZFUNC = always;
		
		
		VertexShader = compile vs_2_0 mainVS();
		PixelShader = compile ps_2_0 mainPS();
	}
}

