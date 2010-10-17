Feature: S1
  As a X1
  I want Y1
  So that Z1

  Scenario: SC1
    Given numbers [left] and [right]
    When I add the numbers
    Then the sum is [sum]
    
    Examples:
    |left|right|sum|
    |1   | 2   |3  |
    |3   | 4   |7  |
                     
  Scenario: inline table
    Given the following people exists:
      |Name          |Country|
      |Morgan Persson|Sweden |
      |Jimmy Nilsson |Sweden |
      |Jimmy bogard  |USA    |
    When I search for people in sweden
    Then I should get:
      |Name          |
      |Morgan Persson|
      |Jimmy Nilsson |

Feature: S2
  As a X2
  I want Y2
  So that Z2

  Scenario: SC1
    Given something
    When some event occurs
    Then there is some outcome
                                         
Feature: S3
  As a X3
  I want Y3
  So that Z3

  Scenario: SC3
    Given something
    When some event occurs
    Then there is some outcome
  Scenario: FailingScenario
    Given something x
    When some event y occurs
    Then there is some failing outcome
