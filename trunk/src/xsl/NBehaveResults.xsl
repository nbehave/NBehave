<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="html" version="1.0" encoding="UTF-8" indent="yes"/>
	<xsl:template match="/">
		<xsl:apply-templates select="//results" />
	</xsl:template>
	<xsl:template match="theme">
				<style type="text/css">
					BODY {
						background-color:darkgray;
						text-align: center;
						font-family: "Trebuchet MS", Tahoma, Verdana, Arial;
					}
					.pageBox {
						text-align: left;
						background-color:white;
						border: 1px solid gray;
						width: 700px;
					}
					.nbehaveTitle {
						background-color: cornflowerblue;
						text-align: center;
						font-size: 20pt;
						font-weight: bold;
						font-variant: small-caps;
						color: white;
						height: 75px;
						vertical-align: middle;
						margin-bottom: 20px;
					}
					.themeBox {
						margin-left: 20px;
						margin-right: 20px;
					}
					.themeTitle {
						font-size: 18pt;
						font-weight: bold;
						border-bottom: 1px solid gray;
						margin-bottom: 10px;
					}
					.storyBox {
						margin-left: 20px;
						margin-bottom: 20px;
					}
					.storyTitlePassed {
						font-size: 16pt;
						font-weight: bold;
						border-bottom: 1px solid lightgray;
						margin-bottom: 5px;
						color: green;
					}
					.storyTitlePending {
						font-size: 16pt;
						font-weight: bold;
						border-bottom: 1px solid lightgray;
						margin-bottom: 5px;
						color: gray;
					}
					.storyTitleFailed {
						font-size: 16pt;
						font-weight: bold;
						border-bottom: 1px solid lightgray;
						margin-bottom: 5px;
						color: red;
					}
					
					.storyNarrative {
						margin-left: 0px;
						margin-bottom: 3px;
						font-size: 10pt;
						font-weight: normal;
						background-color: lightyellow;
						border-bottom: 1px solid lightgray;
						overflow: hidden;
						border: 0;
						line-height: 98%;
						font-family: "Trebuchet MS", Tahoma, Verdana, Arial;
					}
					
					.storyInfos {
						margin-left: 20px;
						background-color: lightyellow;
						font-size: 8pt;
						font-weight: normal;
						margin-bottom: 10px;
						line-height: 98%;
					}
					.storyInfoLabel {
						font-weight: bold;
					}
					.storyInfoValue {
						font-weight: normal;
					}
					.scenarioBox {
						margin-left: 50px;
						margin-bottom: 30px;
					}
					.scenarioTitlePassed {
						font-size: 12pt;
						font-weight: bold;
						color: green;
						border-bottom: 1px solid lightgray;
						margin-bottom: 5px;
					}
					
					.scenarioTitlePending {
						font-size: 12pt;
						font-weight: bold;
						color: gray;
						border-bottom: 1px solid lightgray;
						margin-bottom: 5px;
					}

					.scenarioTitleFailed {
						font-size: 12pt;
						font-weight: bold;
						color: red;
						border-bottom: 1px solid lightgray;
						margin-bottom: 5px;
					}
					
					.scenarioInfos {
						margin-left: 50px;
						background-color: lightyellow;
						font-size: 8pt;
						font-weight: normal;
						margin-bottom: 10px;
						line-height: 98%;
					}

					.actionStepPassed {
						font-size: 8pt;
						font-weight: normal;
						color: green;
						line-height: 98%;
					}
					
					.actionStepPending {
						font-size: 8pt;
						font-weight: normal;
						color: gray;
						line-height: 98%;
					}

					.actionStepFailed {
						font-size: 8pt;
						font-weight: normal;
						color: red;
						line-height: 98%;
					}
		
					.scenarioInfoLabel{
						font-weight: bold;
					}
					.scenarioInfoValue{
						font-weight: normal;
					}
					.scenarioNarration {
						border: 1px solid lightgray;
						padding: 5px;
						font-size: 8pt;
						font-family: "Courier new";
						margin-left: 20px;
					}
				</style>
				<div class="pageBox">
					<div class="nbehaveTitle">NBehave tests results</div>
					<xsl:for-each select=".">
					<div class="themeBox">
						<div class="themeTitle">
							Theme: <xsl:value-of select="@name"/>
						</div>
						<div class="storyInfos">
							<div>
								<span class="storyInfoLabel">Stories run: </span> <span class="storyInfoValue"><xsl:value-of select="@stories"/></span>
							</div>
							<div>
								<span class="storyInfoLabel">Scenarios run: </span> <span class="storyInfoValue"><xsl:value-of select="@scenarios"/></span>
							</div>
							<div>
								<span class="storyInfoLabel">Scenarios failed: </span> <span class="storyInfoValue"><xsl:value-of select="@scenariosFailed"/></span>
							</div>
							<div>
								<span class="storyInfoLabel">Scenarios pending: </span> <span class="storyInfoValue"><xsl:value-of select="@scenariosPending"/></span>
							</div>
							<div>
								<span class="storyInfoLabel">Execution time: </span> <span class="storyInfoValue"><xsl:value-of select="@time"/> s</span>
							</div>							
						</div>
						<xsl:for-each select="stories/story">
						<div class="storyBox">
							
							<div class="storyTitle">
								<xsl:attribute name="class">
									<xsl:if test="@scenariosFailed>0">storyTitleFailed</xsl:if>
									<xsl:if test="@scenariosFailed=0">
										<xsl:if test="@scenariosPending=0">storyTitlePassed</xsl:if>
										<xsl:if test="@scenariosPending>0">storyTitlePending</xsl:if>												
									</xsl:if>
								</xsl:attribute>
								Story: <xsl:value-of select="@name"/>
						
							</div>
							<div class="storyInfos">
								<div>
									<span class="storyInfoLabel">
										<textarea class="storyNarrative" rows="3" cols="80" readonly="true">
											<xsl:value-of select="narrative"/>
										</textarea>
									</span>
								</div>
								<div>
									<span class="storyInfoLabel">Scenarios run: </span> <span class="storyInfoValue"><xsl:value-of select="@scenarios"/></span>
								</div>
								<div>
									<span class="storyInfoLabel">Scenarios failed: </span> <span class="storyInfoValue"><xsl:value-of select="@scenariosFailed"/></span>
								</div>
								<div>
									<span class="storyInfoLabel">Scenarios pending: </span> <span class="storyInfoValue"><xsl:value-of select="@scenariosPending"/></span>
								</div>
								<div>
									<span class="storyInfoLabel">Execution time: </span> <span class="storyInfoValue"><xsl:value-of select="@time"/> s</span>
								</div>
							</div>
							<xsl:for-each select="scenarios/scenario">
							<div class="scenarioBox">
								<!-- <div class="scenarioFailedTitle"> -->
								<div>
									<xsl:attribute name="class">
										<xsl:if test="@outcome='pending'">scenarioTitlePending</xsl:if>
										<xsl:if test="@outcome='passed'">scenarioTitlePassed</xsl:if>
										<xsl:if test="@outcome='failed'">scenarioTitleFailed</xsl:if>
									</xsl:attribute>
									Scenario <xsl:value-of select="@name"/>
								</div>
								<!--
								<div class="scenarioInfos">
									<div>
										 <span class="scenarioInfoLabel">Execution time: </span> <span class="scenarioInfoValue"><xsl:value-of select="@time"/> s</span> 
									</div>
								</div>
								-->
								<div class="scenarioNarration">
									<xsl:for-each select="actionStep">
										<div>
											<xsl:attribute name="class">
												<xsl:if test="@outcome='pending'">actionStepPending</xsl:if>
												<xsl:if test="@outcome='passed'">actionStepPassed</xsl:if>
												<xsl:if test="@outcome='failed'">actionStepFailed</xsl:if>
											</xsl:attribute>
											<xsl:value-of select="@name"/><br />
											<xsl:if test="@outcome='failed'">
												<div class="scenarioNarration">
													<xsl:value-of select="failure" />
												</div>
											</xsl:if>
										</div>
									</xsl:for-each>
								</div>
							</div>
							</xsl:for-each>
						</div>
						</xsl:for-each>
					</div>
					</xsl:for-each>
				</div>
	</xsl:template>
</xsl:stylesheet>
