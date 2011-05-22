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
float3 gEyePosition : EyePosition;

float4x4 gLightViewProj : ScenePassCameraViewProjection
	<string ScenePass = "ShadowPass";>;

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
	
texture2D gShadowMap : RenderTargetMap
	<string TargetName = "ShadowMap";>;
sampler2D gShadowMapSampler = sampler_state 
	{
	  Texture = <gShadowMap>;
	  MinFilter = linear;
	  MagFilter = linear;
	  MipFilter = linear;
	  AddressU = Clamp;
	  AddressV = Clamp;
	};	
	

struct VsOutput
{
    float4 pos : POSITION;
    float2 uv : TEXCOORD0;
    float4 posWorld : TEXCOORD1;
    float2 Depth : TEXCOORD2;
};	


VsOutput mainVS(float4 pos: POSITION, float2 uv: TEXCOORD0)
{
	VsOutput output = (VsOutput)0;
	
	output.pos = mul(pos, gWorldViewProj);
	output.uv = uv;
	output.posWorld = mul(pos, gWorld);
	
	output.Depth = float2(output.pos.z, output.pos.w);

	return output;
}

struct PsOut
{
    half4 Color : COLOR0;
    half4 Normal : COLOR1;
    half4 Depth : COLOR2;
};

PsOut mainPS(float2 uv: TEXCOORD0, float4 posWorld : TEXCOORD1, float2 depth : TEXCOORD02) 
{
	PsOut output;

	float4 lightingPosition = mul(posWorld, gLightViewProj);
    // Find the position in the shadow map for this pixel
    float2 shadowTexCoord = 0.5 * lightingPosition.xy / 
                            lightingPosition.w + float2( 0.5, 0.5 );
    shadowTexCoord.y = 1.0f - shadowTexCoord.y;
    
    float4 shadowColor  = float4(tex2D(gShadowMapSampler, shadowTexCoord).rgb, 1);

	output.Color  = tex2D(gDiffuseMapSampler, uv) * shadowColor;
    output.Normal = float4(0.5,0.5,1,0); // float4(0.5f * (normalize(normal) + 1.0f), 0);
    output.Depth = depth.x / depth.y;
	
	return output;
}


technique Default
{
	pass p0 
	<
		bool IsTransparent=false;
		string MainTexture = "gDiffuseMap";  
		bool ReceivesLight = true;
	>
	{
		AlphaBlendEnable = false;
		AlphaTestEnable = false;

		ZEnable = true;
		ZWriteEnable = true;
		CullMode = None;
		ZFUNC = lessequal;
	
		VertexShader = compile vs_2_0 mainVS();
		PixelShader = compile ps_2_0 mainPS();
	}
}

