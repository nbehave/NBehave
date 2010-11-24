Feature: Support for background sections
	As a NBehave user
	I want to be able to declare background sections
	So that I can add context to my scenarios

	Background:
		Given this background section declaration
		And this one

	Scenario: Running a feature file with a background section
		Given this scenario under the context of a background section
		When the scenario with a background section is executed
		Then the background section steps should be called before this scenario