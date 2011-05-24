Feature: File with incorrect steps

Scenario: Given, when then in lowercase
	given a step
	when steps are all lowercase
	then we should not crash