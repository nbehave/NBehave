Feature: A Game of Bowling

Scenario: All rolls hit one

	Given a game of bowling
	When all my 20 rolls are 1
	Then my score should be 20
