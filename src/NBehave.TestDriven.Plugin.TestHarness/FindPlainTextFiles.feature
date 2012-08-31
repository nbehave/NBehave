Feature: Locating plain text specification (part 1)
	As a user of TestDriven.Net
	I need to locate the set of plain text specification files that relate to the assembly
	So I can run the scenarios
		
	Scenario: Find files (should pass)
		Given an assembly NBehave.TestDriven.Plugin.Tests
		When I look for text files
		Then I should find 2 feature files
		And I should find 0 story files
		And I should find 0 specification files	
		
	Scenario: Find files (Has a missing Specification)
		Given an assembly NBehave.TestDriven.Plugin.Tests
		When I look for Something Ive not defined
		Then I should not expect success or failure
		
Feature: Locating plain text specification (part 1)
	As a user of TestDriven.Net
	I need to locate the set of plain text specification files that relate a class
	So I can run the scenarios
	
	Scenario: Find files (should pass)
		Given an assembly NBehave.TestDriven.Plugin.Tests
		And a class NBehave.TestDriven.Plugin.Tests.FindPlainTextFiles
		When I look for text files
		Then I should find 1 feature files
		And I should find 0 story files
		And I should find 0 specification files	
