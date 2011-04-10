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
float4x4 gWorld : World;
float gTime : Time;

float4 gStartColor = float4(3.0,3.0,1.0,1);
float4 gEndColor = float4(0.1,0.1,0.1,0.1);

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
	
struct VsOutput
{
    float4 pos : POSITION;
    float2 uv : TEXCOORD0;
    float4 vColor : COLOR;
};	
	
VsOutput mainVS(float4 pos: POSITION, float2 uv: TEXCOORD0, float lerpV: TEXCOORD1)
{
    VsOutput output = (VsOutput)0;

	//Rotation
	float sinRot = sin(gTime * 4);
	float cosRot = cos(gTime * 4);
	
    float3 eyeVector = gViewProj._m02_m12_m22;
    float3 side = normalize(cross(eyeVector, cWorldUp));
    float3 up  = normalize(cross(side, eyeVector));
   
    float2 centeredUv = uv - float2(0.5, 0.5);

	
    //output.pos = mul(float4(mul(pos, gWorld) + ((centeredUv.x * cosRot - centeredUv.y * sinRot) * side - (centeredUv.x * sinRot + centeredUv.y  * cosRot) * up), 1), gViewProj);
    output.pos = mul(float4(pos + ((centeredUv.x * cosRot - centeredUv.y * sinRot) * side - (centeredUv.x * sinRot + centeredUv.y  * cosRot) * up), 1), gViewProj);
   
   // output.pos = mul(float4(mul(pos, gWorld) + (centeredUv.x * cosRot - centeredUv.y * sinRot) * side - (centeredUv.x * sinRot + centeredUv.y  * cosRot) * up, 1), gViewProj);
    output.uv = uv;
    
    
    output.vColor = lerp(gStartColor, gEndColor, lerpV);
   
    return output;
}

float4 mainPS(float2 uv: TEXCOORD0, float4 vColor: COLOR) : COLOR 
{
	float4 colorFromTexture = tex2D(gDiffuseMapSampler, uv);
	//colorFromTexture.rgb = lerp(colorFromTexture.rgb, vColor.rgb, 0.5) ;   
	colorFromTexture.rgb *= vColor.rgb;   
	colorFromTexture.a = min(colorFromTexture.a, vColor.a);
	//colorFromTexture.rgb =  colorFromTexture.a;
//	colorFromTexture.xyz = 0;
	
	
	return colorFromTexture;
	
	//float4 colorFromTexture = tex2D(gDiffuseMapSampler, uv);
	//float alpha = (colorFromTexture.x + colorFromTexture.y + colorFromTexture.z) / 3.0f *  vColor.w;
	//return float4(alpha * vColor.xyz, alpha);   
	//return float4(colorFromTexture.xyz, vColor.w);   
	//return vColor;   
//	return tex2D(gDiffuseMapSampler, uv);//* float4(0.7, 0.4, 0.2, 1);   
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
		
		BlendOp = add;

	//	DestBlend = one;
	//	SrcBlend = SRCCOLOR;

//		DestBlend = INVSRCCOLOR;
//		SrcBlend = SRCCOLOR;

		//DestBlend = INVSRCALPHA;
		DestBlend = one;
	//	SrcBlend = SRCCOLOR;
		SrcBlend = SRCALPHA;

		ZEnable = true;
		ZWriteEnable = false;
		CullMode = None;
		ZFUNC = lessequal;

		VertexShader = compile vs_2_0 mainVS();
		PixelShader = compile ps_2_0 mainPS();
	}
}
