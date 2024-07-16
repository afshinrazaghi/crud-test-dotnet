Feature: user can apply CRUD operations against the customers

"""
Customer (
	FirstName,
	LastName,
	PhoneNumber,
	Email,
	BankAccountNumber,
	DateOfBirth
)
"""

Scenario: User Can Create, Edit, Delete And Read Customers
	Given PlatForm has "0" record of customers
	When user send command to create new customer with following information
		| FirstName | LastName | PhoneNumber   | Email              | BankAccountNumber  | DateOfBirth |
		| jane      | doe      | +989050647735 | jane.doe@gmail.com | NL20INGB0001234567 | 12-JUN-1990 |
	Then user send query and receive "1" record of customer with following information
		| FirstName | LastName | PhoneNumber   | Email              | BankAccountNumber  | DateOfBirth |
		| jane      | doe      | +989050647735 | jane.doe@gmail.com | NL20INGB0001234567 | 12-JUN-1990 |

	When user send command to update existing customer with email of "jane.doe@gmail.com" with following information
		| FirstName | LastName | PhoneNumber   | Email                | BankAccountNumber  | DateOfBirth |
		| john      | smith    | +989050327735 | john.smith@gmail.com | NL91ABNA0417164300 | 12-JUL-1994 |
	Then user send query and receive "1" record of customer with following information
		| FirstName | LastName | PhoneNumber   | Email                | BankAccountNumber  | DateOfBirth |
		| john      | smith    | +989050327735 | john.smith@gmail.com | NL91ABNA0417164300 | 12-JUL-1994 |
	And user send query and receive "0" record of customer with following information
		| FirstName | LastName | PhoneNumber   | Email                | BankAccountNumber  | DateOfBirth |
		| john      | smith    | +313123355239 | john.smith@gmail.com | NL91ABNA0417164300 | 12-JUL-1994 |

	When user send a command to delete existing customer with email of "john.smith@gmail.com"
	Then user send a query to get all customers and receive "0" record of customer

	