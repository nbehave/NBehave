Feature: Table support in nbehave

Scenario: a table

Given a list of people:
|name          |country|
|Morgan Persson|Sweden |
|Jimmy Nilsson |Sweden |
|Jimmy Bogard  |USA    |

When I search for people from Sweden

Then I should find:
|Name          |Country|
|Morgan Persson|Sweden |
|Jimmy Nilsson |Sweden |
