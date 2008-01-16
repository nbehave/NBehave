<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output cdata-section-elements="message stack-trace"/>
  
	<xsl:template match="/">
		<xsl:apply-templates/>
	</xsl:template>
	
  <xsl:template match="assembly">
		<test-results>
			<xsl:attribute name="name">
				<xsl:value-of select="@name"/>
			</xsl:attribute>
			<xsl:attribute name="date">
				<xsl:value-of select="@date"/>
			</xsl:attribute>
			<xsl:attribute name="time">
				<xsl:value-of select="@time"/>
			</xsl:attribute>
			<xsl:attribute name="total">
				<xsl:value-of select="@total"/>
			</xsl:attribute>
			<xsl:attribute name="failures">
				<xsl:value-of select="@failures"/>
			</xsl:attribute>
			<xsl:attribute name="not-run">
				<xsl:value-of select="@not-run"/>
			</xsl:attribute>
			<test-suite>
				<xsl:attribute name="name">
					<xsl:value-of select="@name"/>
				</xsl:attribute>
				<xsl:attribute name="success">
					<xsl:value-of select="@success"/>
				</xsl:attribute>
				<xsl:attribute name="time">
					<xsl:value-of select="@test-time"/>
				</xsl:attribute>
				<results>
					<xsl:apply-templates select="class"/>
				</results>
			</test-suite>
		</test-results>
	</xsl:template>

  <xsl:template match="class">
		<test-suite>
			<xsl:attribute name="name">
				<xsl:value-of select="@name"/>
			</xsl:attribute>
			<xsl:attribute name="success">
				<xsl:value-of select="@success"/>
			</xsl:attribute>
			<xsl:attribute name="time">
				<xsl:value-of select="@time"/>
			</xsl:attribute>
      <xsl:if test="failure">
        <xsl:copy-of select="failure"/>
				<!--<failure>
					<message>
						<xsl:apply-templates select="failure/message" mode=""/>
					</message>
				</failure>-->
			</xsl:if>
			<xsl:if test="reason">
				<reason>
					<xsl:apply-templates select="reason"/>
				</reason>
			</xsl:if>
      <results>
				<xsl:apply-templates select="test"/>
			</results>
		</test-suite>
	</xsl:template>
  
	<xsl:template match="test">
		<test-case>
			<xsl:attribute name="name">
				<xsl:value-of select="@name"/>
			</xsl:attribute>
			<xsl:attribute name="executed">
				<xsl:value-of select="@executed"/>
			</xsl:attribute>
			<xsl:if test="@success">
				<xsl:attribute name="success">
					<xsl:value-of select="@success"/>
				</xsl:attribute>
			</xsl:if>
			<xsl:if test="@time">
				<xsl:attribute name="time">
					<xsl:value-of select="@time"/>
				</xsl:attribute>
			</xsl:if>
			<xsl:copy-of select="node()"/>
		</test-case>
	</xsl:template>
  
</xsl:stylesheet>