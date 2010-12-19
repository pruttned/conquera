<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="xml"  indent="yes" />

	<xsl:variable name="count" select="/cellsTexture/@count" />
	<xsl:variable name="size" select="/cellsTexture/@size" />
	<xsl:variable name="baseCellSize" select="$size div $count" />
	<xsl:variable name="cellSpacing" select="/cellsTexture/@spacing * $baseCellSize" />
	<xsl:variable name="cellSize" select="$baseCellSize - $cellSpacing" />
	<xsl:variable name="a" select="1" />

	<xsl:template match="/cellsTexture">
		<svg viewBox="0 0 1 1" xmlns="http://www.w3.org/2000/svg" version="1.1">
			<xsl:attribute name="width">
				<xsl:value-of select="$size" />
				<xsl:text>px</xsl:text>
			</xsl:attribute>
			<xsl:attribute name="height">
				<xsl:value-of select="$size" />
				<xsl:text>px</xsl:text>
			</xsl:attribute>
			<xsl:attribute name="viewBox">
				<xsl:text>0 0 </xsl:text>
				<xsl:value-of select="$size" />
				<xsl:text disable-output-escaping="yes"> </xsl:text>
				<xsl:value-of select="$size" />
			</xsl:attribute>
			<xsl:call-template name="polygon">
				<xsl:with-param name="countI" select="$count"/>
				<xsl:with-param name="countJ" select="$count"/>
				<xsl:with-param name="colorR" select="124"/>
				<xsl:with-param name="colorG" select="103"/>
				<xsl:with-param name="colorB" select="176"/>
			</xsl:call-template>
		</svg>
	</xsl:template>

	<xsl:template name="polygon">
		<xsl:param name="countI" select="0" />
		<xsl:param name="countJ" select="0" />
		<xsl:param name="colorR" select="0" />
		<xsl:param name="colorG" select="0" />
		<xsl:param name="colorB" select="0" />
		<xsl:if test="$countJ > 0">

			<xsl:call-template name="polygon2">
				<xsl:with-param name="countI" select="$countI"/>
				<xsl:with-param name="countJ" select="$countJ"/>
				<xsl:with-param name="colorR" select="$colorR"/>
				<xsl:with-param name="colorG" select="$colorG"/>
				<xsl:with-param name="colorB" select="$colorB"/>
			</xsl:call-template>


			<xsl:call-template name="polygon">
				<xsl:with-param name="countI" select="$countI"/>
				<xsl:with-param name="countJ" select="$countJ - 1"/>
				<xsl:with-param name="colorR" select="( 69 * $colorB + 18) mod 255"/>
				<xsl:with-param name="colorG" select="( 17 * $colorR + 156) mod 255"/>
				<xsl:with-param name="colorB" select="( 198 * $colorG + 45) mod 255"/>
				<!-- farby su naschval poprehadzovane -->
			</xsl:call-template>
		</xsl:if>
	</xsl:template>

	<xsl:template name="polygon2">
		<xsl:param name="countI" select="0" />
		<xsl:param name="countJ" select="0" />
		<xsl:param name="colorR" select="0" />
		<xsl:param name="colorG" select="0" />
		<xsl:param name="colorB" select="0" />

		<xsl:if test="$countI > 0">

			<polygon xmlns="http://www.w3.org/2000/svg">
				<xsl:attribute name="style">
				<xsl:text>fill:rgb(</xsl:text>
				<xsl:value-of select="$colorR" />
				<xsl:text>,</xsl:text>
				<xsl:value-of select="$colorG" />
				<xsl:text>,</xsl:text>
				<xsl:value-of select="$colorB" />
				<xsl:text>); stroke:#000000; stroke-width:1px;</xsl:text>
				</xsl:attribute>
				<xsl:attribute name="points">
					<xsl:value-of select="$cellSize*0.5 + $baseCellSize * ($countI - 1)" />
					<xsl:text>,</xsl:text>
					<xsl:value-of select="$cellSize*1 + $baseCellSize * ($countJ - 1)" />
					<xsl:text disable-output-escaping="yes"> </xsl:text>
					<xsl:value-of select="$cellSize*0.067 + $baseCellSize * ($countI - 1)" />
					<xsl:text>,</xsl:text>
					<xsl:value-of select="$cellSize*0.75 + $baseCellSize * ($countJ - 1)" />
					<xsl:text disable-output-escaping="yes"> </xsl:text>
					<xsl:value-of select="$cellSize*0.067 + $baseCellSize * ($countI - 1)" />
					<xsl:text>,</xsl:text>
					<xsl:value-of select="$cellSize*0.25 + $baseCellSize * ($countJ - 1)" />
					<xsl:text disable-output-escaping="yes"> </xsl:text>
					<xsl:value-of select="$cellSize*0.5 + $baseCellSize * ($countI - 1)" />
					<xsl:text>,</xsl:text>
					<xsl:value-of select="$cellSize*0 + $baseCellSize * ($countJ - 1)" />
					<xsl:text disable-output-escaping="yes"> </xsl:text>
					<xsl:value-of select="$cellSize*0.93301 + $baseCellSize * ($countI - 1)" />
					<xsl:text>,</xsl:text>
					<xsl:value-of select="$cellSize*0.25 + $baseCellSize * ($countJ - 1)" />
					<xsl:text disable-output-escaping="yes"> </xsl:text>
					<xsl:value-of select="$cellSize*0.93301 + $baseCellSize * ($countI - 1) " />
					<xsl:text>,</xsl:text>
					<xsl:value-of select="$cellSize*0.75 + $baseCellSize * ($countJ - 1)" />
				</xsl:attribute>
			</polygon>	 	
			<polygon xmlns="http://www.w3.org/2000/svg">
				<xsl:attribute name="style">
				<xsl:text>fill:rgb(</xsl:text>
				<xsl:value-of select="255 - $colorR" />
				<xsl:text>,</xsl:text>
				<xsl:value-of select="255 - $colorG" />
				<xsl:text>,</xsl:text>
				<xsl:value-of select="255 - $colorB" />
				<xsl:text>); stroke-width:0px;</xsl:text>
				</xsl:attribute>
				<xsl:attribute name="points">
				
					<xsl:value-of select="$cellSize*0.5 + $baseCellSize * ($countI - 1)" />
					<xsl:text>,</xsl:text>
					<xsl:value-of select="$cellSize*0.0 + $baseCellSize * ($countJ - 1)" />
					<xsl:text disable-output-escaping="yes"> </xsl:text>

					<xsl:value-of select="$cellSize*0.067 + $baseCellSize * ($countI - 1)" />
					<xsl:text>,</xsl:text>
					<xsl:value-of select="$cellSize*0.75 + $baseCellSize * ($countJ - 1)" />
					<xsl:text disable-output-escaping="yes"> </xsl:text>
					
					<xsl:value-of select="$cellSize*0.93301 + $baseCellSize * ($countI - 1)" />
					<xsl:text>,</xsl:text>
					<xsl:value-of select="$cellSize*0.75 + $baseCellSize * ($countJ - 1)" />
				</xsl:attribute>
			</polygon>	 	


			<xsl:call-template name="polygon2">
				<xsl:with-param name="countI" select="$countI - 1"/>
				<xsl:with-param name="countJ" select="$countJ"/>
				<xsl:with-param name="colorR" select="( 16 * $colorR + 99) mod 255"/>
				<xsl:with-param name="colorG" select="( 185 * $colorG + 5) mod 255"/>
				<xsl:with-param name="colorB" select="( 67 * $colorB + 111) mod 255"/>
			</xsl:call-template>
		</xsl:if>
	</xsl:template>

</xsl:stylesheet>




