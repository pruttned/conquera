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

float4x4 gWorldViewProj : WorldViewProjection;
float4x4 gWorld : World;
float4x4 gInvertViewProjection : InvertViewProjection;
float3 gLightPosition : WorldBoundPos;
float2 gHalfPixel : ScreenHalfPixel;
float3 gEyePosition : EyePosition;

float gLightRadius = 3.5;
float3 gColor;

texture2D gColorMap: RenderTargetMap
	<string TargetName = "ScreenColorMap";>;
sampler2D gColorMapSampler = sampler_state 
	{
	  Texture = <gColorMap>;
	  MinFilter = POINT;
	  MagFilter = POINT;
	  MipFilter = POINT;
	  AddressU = CLAMP;
	  AddressV = CLAMP;
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
			
struct VsInput
{
    float4 Position : POSITION;
};
	
struct VsOutput
{
    float4 Position : POSITION;
    float4 ScreenPosition : TEXCOORD0;
};	
	
VsOutput mainVS(VsInput input)
{
    VsOutput output;
    output.Position = mul(input.Position, gWorldViewProj);
    output.ScreenPosition = output.Position;
    return output;
}

float4 mainPS(float4 screenPosition : TEXCOORD0) : COLOR 
{
    screenPosition.xy /= screenPosition.w;
   // screenPosition.xy+=gHalfPixel;

    float2 uv = 0.5f * (float2(screenPosition.x,-screenPosition.y) + 1);
    uv +=gHalfPixel;

    float4 normalData = tex2D(gNormalMapSampler,uv);
    float3 normal = 2.0f * normalData.xyz - 1.0f;
    
    float depthVal = tex2D(gDepthMapSampler,uv).r;

    float4 position = mul(float4(screenPosition.xy, depthVal, 1.0f), gInvertViewProjection);
    position /= position.w;

    float3 lightVector = gLightPosition - position;

    float attenuation = saturate(1.0f - pow(length(lightVector)/gLightRadius, 5)); 

    lightVector = normalize(lightVector); 

    float3 diffuseLight = max(0,dot(normal,lightVector)) * gColor.rgb;

//return float4(1,1,1,0.2);
    return float4(attenuation * diffuseLight.rgb, 1); 
}

technique Default
{
	pass p0 
	{
		AlphaBlendEnable = true;
		AlphaTestEnable = false;
	
		BlendOp = add;
		DestBlend = ONE;
		SrcBlend = ONE;
	

		ZEnable = false;
		ZWriteEnable = false;
		//ZFUNC = greaterequal;
		CullMode = CW;
		
		VertexShader = compile vs_2_0 mainVS();
		PixelShader = compile ps_2_0 mainPS();
	}
}
