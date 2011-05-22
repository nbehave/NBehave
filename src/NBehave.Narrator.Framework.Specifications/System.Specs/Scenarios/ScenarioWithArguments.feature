Feature: Support for scenarios
	As a NBehave user
	I want to be able to declare scenarios
	So that I can test my features

	Scenario: Running a feature file with a scenario that has arguments
		Given a scenario that has arguments
		When the scenario with arguments is executed
		Then the scenario with arguments should pass