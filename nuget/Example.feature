Feature: a silly example with a list

Scenario: Add item to list
	Given an empty list
	When I add foo to list
	Then list the list should contain foo