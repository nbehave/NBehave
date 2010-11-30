Feature: Support for examples
	As a NBehave user
	I want to be able to declare examples
	So that I can template my scenarios

	Scenario: Running a feature file with a examples section
		Given this scenario containing examples [col1]
		When the scenario is executed [col2]
		Then it should be templated and executed with each [row]

		Examples:
			| col1 | col2 | row |
			|  1   |  2   | 3   |
			|  2   |  4   | 6   |
