Scenario: One Spare

	Given a game of bowling
	When I roll one spare
		And the first preceding roll is 3
		And the rest of my 17 rolls are 0
	Then my score should be 16