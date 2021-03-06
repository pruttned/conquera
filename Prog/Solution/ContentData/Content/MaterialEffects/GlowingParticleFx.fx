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

float4x4 gViewProj : ViewProjection;
//float4x4 gWorld : World;
float gTime : Time;


float2 gUvs[4];// = {float2(0,0), float2(0,0.5), float2(0.5,0), float2(0.5, 0.5)};
//float4 gEndColor = float4(0,0,0,1);

float4 gColorVariation : ColorVariation;

#define cWorldUp float3(0,0,1)

texture2D gDiffuseMap;
sampler2D gDiffuseMapSampler = sampler_state 
	{
	  Texture = <gDiffuseMap>;
	  MinFilter = Linear;
	  MagFilter = Linear;
	  MipFilter = Linear;
	  AddressU = Clamp;
	  AddressV = Clamp;
	};
	
texture2D gParticleColorFunctionMap : ColorFunctionMap;
sampler2D gParticleColorFunctionMapSampler = sampler_state 
	{
	  Texture = <gParticleColorFunctionMap>;
	  MinFilter = Linear;
	  MagFilter = Linear;
	  MipFilter = Linear;
	  AddressU = Clamp;
	  AddressV = Clamp;
	};	
	
struct VsOutput
{
    float4 pos : POSITION;
    float2 uv : TEXCOORD0;
    float4 vColorVariation : TEXCOORD2;
    float2 colorIndex : TEXCOORD1;
    
};	
	
VsOutput mainVS(float4 pos: POSITION, float2 normalizedUv: TEXCOORD0, float lerpV: TEXCOORD1, float seed: TEXCOORD2, float rot : TEXCOORD3, float size: TEXCOORD4)
{
    VsOutput output = (VsOutput)0;

	//Rotation
//	float cosRot = cos(gTime * 4);
//	float sinRot = sin(lerpV * 4 * seed);
	//float cosRot = cos(lerpV * 4 * seed);
	float sinRot = sin(rot+seed);
	float cosRot = cos(rot+seed);

//sinRot = 0;
//cosRot = 1;
	
    float3 eyeVector = gViewProj._m02_m12_m22;

    float3 side = normalize(cross(eyeVector, cWorldUp));
    float3 up  = normalize(cross(side, eyeVector));
   
    float2 centeredUv = normalizedUv - float2(0.5, 0.5);
centeredUv*=size; //nastavovanie velkosti
	
    //output.pos = mul(float4(pos, gWorld) + ((centeredUv.x * cosRot - centeredUv.y * sinRot) * side - (centeredUv.x * sinRot + centeredUv.y  * cosRot) * up), 1), gViewProj);
    
   
    output.pos = mul(float4(pos + ((centeredUv.x * cosRot - centeredUv.y * sinRot) * side - (centeredUv.x * sinRot + centeredUv.y  * cosRot) * up), 1), gViewProj);
   
   // output.pos = mul(float4(mul(pos, gWorld) + (centeredUv.x * cosRot - centeredUv.y * sinRot) * side - (centeredUv.x * sinRot + centeredUv.y  * cosRot) * up, 1), gViewProj);
   
   //abs(sin(gTime))*
    output.uv = normalizedUv/2 + gUvs[seed*4];
   
   
  //  output.vColor = lerp(gStartColor, gEndColor, lerpV);
  //  output.vColor.r = seed;
 //   seed = fmod(seed *11235.123456f + 0.1159123f, 1.0);
 //   output.vColor.g = seed;
 //   seed = fmod(seed *11235.123456f + 0.1159123f, 1.0);
//    output.vColor.b = seed*0.6;
//    output.vColor.a = seed;

   
   seed = (seed -0.5)*2 ;
    output.vColorVariation.r = gColorVariation.r * seed ;
    seed = fmod(seed *11235.123456f + 0.1159123f, 1.0);
    output.vColorVariation.g = gColorVariation.g * seed;
    seed = fmod(seed *11235.123456f + 0.1159123f, 1.0);
    output.vColorVariation.b = gColorVariation.b * seed;
    output.vColorVariation.a = gColorVariation.a * seed;
	


        float index  = lerpV * 4096;
    output.colorIndex = float2(fmod(index, 64)/64, index /64/64);
    return output;
}

float4 mainPS(float2 uv: TEXCOORD0, float4 vColorVariation: TEXCOORD2, float2 colorIndex : TEXCOORD1) : COLOR 
{
	float4 color = vColorVariation + tex2D(gParticleColorFunctionMapSampler, colorIndex );
	float4 colorFromTexture = tex2D(gDiffuseMapSampler, uv);
	float alpha = (colorFromTexture.x + colorFromTexture.y + colorFromTexture.z) / 3.0f  * color.a;
	
	
	return float4(color.xyz,alpha);   
	//return float4(alpha * color.xyz,alpha);     toto treba pre DestBlend = one;
}

float4 mainPS2(float2 uv: TEXCOORD0, float4 vColorVariation: TEXCOORD2, float2 colorIndex : TEXCOORD1) : COLOR 
{
	float4 color = vColorVariation + tex2D(gParticleColorFunctionMapSampler, colorIndex );
	float4 colorFromTexture = tex2D(gDiffuseMapSampler, uv);
	float alpha = (colorFromTexture.x + colorFromTexture.y + colorFromTexture.z) / 3.0f  * color.a;
	
	
	return float4(color.xyz, alpha*0.5f);   
	//return float4(alpha * color.xyz,alpha);     toto treba pre DestBlend = one;
}


technique Default
{
	pass p0 
	<
		bool IsTransparent=true;
		string MainTexture = "gDiffuseMap";  
	>
	{
		AlphaBlendEnable = true;
		AlphaTestEnable = true;	
		AlphaFunc = Greater; 
		AlphaRef = 0x000001;

		BlendOp = add;

	//	DestBlend = one;
	//	SrcBlend = SRCCOLOR;

//		DestBlend = INVSRCCOLOR;
//		SrcBlend = SRCCOLOR;

		//DestBlend = INVSRCCOLOR;
		DestBlend = INVSRCALPHA;
	//	DestBlend = one;
	//	SrcBlend = SRCCOLOR;
		SrcBlend = SRCALPHA;
		
		
		ZEnable = true;
		ZWriteEnable = false;
		CullMode = None;
		ZFUNC = lessequal;


		VertexShader = compile vs_2_0 mainVS();
		PixelShader = compile ps_2_0 mainPS();
	}
	
		pass p1 
	<
		bool IsTransparent=true;
		string MainTexture = "gDiffuseMap";  
	>
	{
		AlphaFunc = Greater; 
		AlphaRef = 0x000001;
	
		AlphaBlendEnable = true;
		AlphaTestEnable = true;
		
		BlendOp = add;

	//	DestBlend = one;
	//	SrcBlend = SRCCOLOR;

//		DestBlend = INVSRCCOLOR;
//		SrcBlend = SRCCOLOR;

		//DestBlend = INVSRCCOLOR;
	//	DestBlend = INVSRCALPHA;
		DestBlend = one;
	//	SrcBlend = SRCCOLOR;
		SrcBlend = SRCALPHA;

		ZEnable = true;
		ZWriteEnable = false;
		CullMode = None;
		ZFUNC = lessequal;

		VertexShader = compile vs_2_0 mainVS();
		PixelShader = compile ps_2_0 mainPS2();
	}


}

