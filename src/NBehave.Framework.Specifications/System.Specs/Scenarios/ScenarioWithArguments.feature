Feature: Support for scenarios
	As a NBehave user
	I want to be able to declare scenarios
	So that I can test my features

	Scenario: Running a feature file with a scenario that has arguments
		Given a scenario that has arguments
		When the scenario is executed
		Then it should pass