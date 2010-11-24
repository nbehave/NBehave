Feature: Support for scenarios
	As a NBehave user
	I want to be able to declare scenarios
	So that I can test my features

	Scenario: Running a feature file with a scenario
		Given this scenario
		When the scenario is executed
		Then it should pass

	Scenario: Running a feature file with multiple scenarios
		Given another scenario
		When the scenario is executed
		Then it should also pass