Feature: HandleOrders
	In order to fulfill the client order and get money from him
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

Scenario: Sending taxi to already taken order
	Given There is an already taken order
	And There is a taxi SO12345
	When I send taxi SO12345 to an order
	Then error message should be displayed

Scenario: Sending taxi to already completed order
	Given There is already completed order
	And There is a taxi SO12345
	When I send taxi SO12345 to an order
	Then error message should be displayed
