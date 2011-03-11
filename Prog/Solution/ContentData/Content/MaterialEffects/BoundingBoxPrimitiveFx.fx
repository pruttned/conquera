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
float3 gColor;
float3 gScale;
float3 gPosition;

struct VsOutput
{
    float4 pos : POSITION;
};	
	
VsOutput mainVS(float4 pos: POSITION)
{
    VsOutput output = (VsOutput)0;
      output.pos = pos * float4(gScale, 1);
    output.pos += float4(gPosition, 0);
         
	output.pos = mul(output.pos, gViewProj);

	return output;
}

float4 mainPS() : COLOR 
{
	return float4(gColor, 1); 
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
		ZEnable = true;
		ZWriteEnable = true;
		
		VertexShader = compile vs_2_0 mainVS();
		PixelShader = compile ps_2_0 mainPS();
	}
}

