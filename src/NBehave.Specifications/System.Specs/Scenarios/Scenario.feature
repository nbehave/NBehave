Feature: Support for scenarios
	As a NBehave user
	I want to be able to declare scenarios
	So that I can test my features

	Scenario: Running a feature file with a scenario
		Given this plain scenario
		When this plain scenario is executed
		Then this plain scenario should pass

	Scenario: Running a feature file with multiple scenarios
		Given this second scenario
		When the second scenario is executed
		Then it should also pass