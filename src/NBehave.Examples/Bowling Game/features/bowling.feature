Feature: A Game of Bowling
	As a bowler
	I want to have my score calculated
	So that I dont have to do it myself

Scenario: All rolls hit one
	Given a game of bowling
	When all my 20 rolls are 1
	Then my score should be 20

Scenario: All in the gutter
	Given a game of bowling
	When all my 20 rolls are 0
	Then my score should be 0

Scenario: One Spare
	Given a game of bowling
	When I roll one spare
		And the first preceding roll is 3
		And the rest of my 17 rolls are 0
	Then my score should be 16

Scenario: One Strike
	Given a game of bowling
	When I roll one strike
		And the first preceding roll is 3
		And the second preceding roll is 4
		And the rest of my 16 rolls are 0
	Then my score should be 24