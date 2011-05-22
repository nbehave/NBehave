Feature: Support for scenarios
	As a NBehave user
	I want to be able to declare scenarios
	So that I can test my features

	Scenario: Running a feature file with a scenario that has a failing step
		Given this failing scenario
		When this failing scenario is executed
		Then the failing scenario should display an error message