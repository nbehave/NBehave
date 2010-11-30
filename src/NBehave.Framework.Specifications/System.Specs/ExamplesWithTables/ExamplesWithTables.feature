Feature: Support for scenario outlines with tables
	As a NBehave user
	I want to be able to declare scenario outlines with tables
	So that I can template my scenarios which have tables

	Scenario: Running a feature file with a scenario section
		Given this scenario containing scenario outline and a table:
			|  left  |  right  |
			| [left] | [right] |
		When the tabled scenario outline is executed
		Then the table should be templated into the scenario outline and executed with each row:
			|  sum  |
			| [sum] |

		Examples:
			| left | right | sum |
			|  1   |  2    | 3   |
			|  2   |  4    | 6   |
