@Tag1 @Tag2
Feature: Support for scenarios
	As a NBehave user
	I want to be able to declare scenarios
	So that I can test my features

	@Tag3 @Tag4
	Scenario: Scenario1
		Given this plain scenario
		When this plain scenario is executed
		Then this plain scenario should pass

	@Tag3
	Scenario: Scenario2
		Given this second scenario
		When the second scenario is executed
		Then it should also pass