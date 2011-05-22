Feature: Support for tables
	As a NBehave user
	I want to be able to declare tables
	So that I can test scenario using tabular data

	Scenario: Running a feature file with a scenario that uses tables
		Given this tabled scenario:
		  | Latin           | English      |
		  | Cucumis sativus | Cucumber     |
		  | Cucumis anguria | Burr Gherkin |
		When this tabled scenario is executed
		Then this tabled scenario should pass:
		  | Latin           | English      |
		  | Cucumis sativus | Cucumber     |
		  | Cucumis anguria | Burr Gherkin | 