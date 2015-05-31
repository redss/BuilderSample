Feature: HandleOrders
	In order to fulfill the client order
	As a taxi operator
	I want to be able to send a taxi to a client

Scenario: Sending a taxi
	Given There is a new order
	And There is a taxi SO12345
	When I send taxi SO12345 to an order
	Then order should be assigned to taxi SO12345
	And order should be in progress

Scenario: Sending taxi when it's already handling an order
	Given There is a new order
	And There is a taxi SO12345 assigned to some ongoing order
	When I send taxi SO12345 to an order
	Then error message should be displayed