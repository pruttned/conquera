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
/*	float sampleWeights[15] = 
	{
		0.1061154,
		0.102850571,
		0.102850571,
		0.09364651,
		0.09364651,
		0.0801001,
		0.0801001,
		0.06436224,
		0.06436224,
		0.0485831723,
		0.0485831723,
		0.0344506279,
		0.0344506279,
		0.0229490642,
		0.0229490642
	};
	*/
		float sampleWeights[15] = 
	{
		0.199501351,
		0.176059321,
		0.176059321,
		0.121003687,
		0.121003687,
		0.0647686049,
		0.0647686049,
		0.02699957,
		0.02699957,
		0.008765477,
		0.008765477,
		0.00221625972,
		0.00221625972,
		0.000436407427,
		0.000436407427

	};

	float2 sampleOffsets[15] = 
	{
		0, 0,
		0.003521127 ,0,
		-0.003521127 ,0,
		0.008215963 ,0,
		-0.008215963 ,0,
		0.0129108 ,0,
		-0.0129108 ,0,
		0.01760563 ,0,
		-0.01760563 ,0,
		0.02230047 ,0,
		-0.02230047 ,0,
		0.0269953 ,0,
		-0.0269953 ,0,
		0.03169014 ,0,
		-0.03169014 ,0
	};
	
	float4 sum = 0;
    for (int i = 0; i < 15; i++)
    {
        sum += tex2D(gScreenMapSampler, uv + sampleOffsets[i]) * sampleWeights[i];
    }

     return sum;	
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
