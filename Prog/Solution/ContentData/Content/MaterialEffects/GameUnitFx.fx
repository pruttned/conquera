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

float4x4 gViewProjection : ViewProjection;
float4x4 gWorldViewProj : WorldViewProjection;
float4x4 gWorld : World;
float3 gEyePosition : EyePosition;
float4x4 Skin[30] : Skin;
float3 gPlayerColor;

//float3 gSunLightDirection  = float3(-0.2310634, 0.06931902, 0.9704662);  //treba spravit SunLightDirection auto param  - to budu vlastne s polu s kamerami per scene auto params
float3 gSunLightDirection  = float3(-0.2857143, -0.4285714, 0.8571429);  //treba spravit SunLightDirection auto param  - to budu vlastne s polu s kamerami per scene auto params


texture2D gDiffuseMap;// : RenderTargetMap
	//<string TargetName = "TestRtt";>; //temp
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
    float4 BoneIndices : BLENDINDICES0;
    float4 BoneWeights : BLENDWEIGHT0;
};
	
struct VsOutput
{
    float4 Position : POSITION;
    float2 Uv : TEXCOORD0;
    float3 Normal : TEXCOORD1;
    float2 Depth : TEXCOORD2;
};	
	
VsOutput mainVS(VsInput input)
{
    VsOutput output;
    
    float4x4 skinTransform = 0;
    
    skinTransform += Skin[input.BoneIndices.x] * input.BoneWeights.x;
    skinTransform += Skin[input.BoneIndices.y] * input.BoneWeights.y;
    skinTransform += Skin[input.BoneIndices.z] * input.BoneWeights.z;
    skinTransform += Skin[input.BoneIndices.w] * input.BoneWeights.w;
    
    float4 position = mul(input.Position, skinTransform);
    output.Position = mul(position, gViewProjection);

    output.Normal = normalize(mul(input.Normal, skinTransform));
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

PsOut mainPS(float2 uv: TEXCOORD0, float3 normal : TEXCOORD01, float2 depth : TEXCOORD02) : COLOR 
{
	PsOut output;

	output.Color  = tex2D(gDiffuseMapSampler, uv);
	
	output.Color = lerp (output.Color * float4(gPlayerColor, 1), output.Color, output.Color.a);
	
    output.Normal = float4(0.5f * (normalize(normal) + 1.0f), 1-output.Color.a); //a = diff color power
    output.Depth = depth.x / depth.y;
	
	return output;
}

float4 ShadowCasterPs(float depth : TEXCOORD0) : COLOR 
{
	return float4(0.6f,0.6f,0.6f,1);
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
		CullMode = CCW;
		ZFUNC = lessequal;
	
		VertexShader = compile vs_2_0 mainVS();
		PixelShader = compile ps_2_0 mainPS();
	}
}

technique ShadowPass
{
	pass trt 
	<
		bool IsTransparent=false;
	>
	{
		AlphaTestEnable = false;

		ZEnable = true;
		ZWriteEnable = true;
		CullMode = CCW;
		ZFUNC = lessequal;
		
		VertexShader = compile vs_2_0 mainVS();
		PixelShader = compile ps_2_0 ShadowCasterPs();
	}
}