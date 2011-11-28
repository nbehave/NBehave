Feature: dummy
	This file is here (almost) only to trigger install.ps1 for nuget, feel free to delete it.
	You may also try to run this file with the plugin installed, just right click in it (or on it in solution explorer) and then click on "Run Unit Tests"
	
Scenario: dummy
	Given this file
	When you right click it and then click "Run Unit Tests"
	Then this file is "executed"
	And you should see 4 pending steps in R#'s testrunner