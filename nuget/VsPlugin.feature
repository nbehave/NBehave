Feature: dummy
	This file is here (almost) only to trigger install.ps1 for nuget, feel free to delete it.
	You may also try to run this file with the plugin installed.
	
Scenario: dummy
	Given this file
	When you right click this file and then click "Run Scenario"
		# or click the green dot with the arrow to the left 
	Then this file is "executed"
	And you should see 4 pending steps in the output window