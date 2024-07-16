Feature: apply CRUD operations against the customers

"""
Customer(
	FirstName,
	LastName,
	PhoneNumber,
	Email,
	BankAccountNumber,
	DateOfBirth
)
"""

As a an operator I wish to be able to Create, Update, Delete customers and list all customers
	
@create
Scenario: user creates a new customer
	Given platform has 0 record of customer
	When user send command to create new customer with the following information
		| FirstName | LastName | DateOfBirth | PhoneNumber   | Email                        | BankAccountNumber  |
		| Afshin    | Razaghi  | 12-JUN-1990 | +989050647735 | afshin.razaghi.net@gmail.com | NL91RABO0315273637 |
	Then user send query and receive 1 record of customer with the following information
		| FirstName | LastName | DateOfBirth | PhoneNumber   | Email                        | BankAccountNumber  |
		| Afshin    | Razaghi  | 12-JUN-1990 | +989050647735 | afshin.razaghi.net@gmail.com | NL91RABO0315273637 |

@update
Scenario: update an existing customer
	Given a customer with following details exists
		| FirstName | LastName | DateOfBirth | PhoneNumber   | Email                        | BankAccountNumber  |
		| Afshin    | Razahgi  | 12-JUN-1990 | +989050647735 | afshin.razaghi.net@gmail.com | NL91RABO0315273637 |
	When user send command to update customer with email of "afshin.razaghi.net@gmail.com" with following information
		| FirstName | LastName | DateOfBirth | PhoneNumber   | Email                | BankAccountNumber  |
		| Hamid     | Hami     | 17-FEB-1995 | +989050647748 | hamid.hami@gmail.com | NL20INGB0001234567 |
	Then user send query and receive 1 record of customer with the following information
		| FirstName | LastName | DateOfBirth | PhoneNumber   | Email                | BankAccountNumber  |
		| Hamid     | Hami     | 17-FEB-1995 | +989050647748 | hamid.hami@gmail.com | NL20INGB0001234567 |
	And user send query and receive 0 record of customer with the following information
		| FirstName | LastName | DateOfBirth | PhoneNumber   | Email                        | BankAccountNumber  |
		| Afshin    | Razahgi  | 12-JUN-1990 | +989050647735 | afshin.razaghi.net@gmail.com | NL91RABO0315273637 |
	
@delete
Scenario: delete a customer
	Given a customer with following details exists
		| FirstName | LastName | DateOfBirth | PhoneNumber   | Email                | BankAccountNumber  |
		| Hamid     | Hami     | 17-FEB-1995 | +989050647748 | hamid.hami@gmail.com | NL20INGB0001234567 |
	When user send command to delete existing customer with email of "hamid.hami@gmail.com"
	Then user send query to get all customers and receive 0 record of customer

@retrieveAllCustomers
Scenario: successfully retrieve all customers
	Given a customer with following details exists
		| FirstName | LastName | DateOfBirth | PhoneNumber   | Email                | BankAccountNumber  |
		| Hamid     | Hami     | 17-FEB-1995 | +989050647748 | hamid.hami@gmail.com | NL20INGB0001234567 |
	When user send query to get all customers
	Then user should receive customers with the following information
		| FirstName | LastName | DateOfBirth | PhoneNumber   | Email                | BankAccountNumber  |
		| Hamid     | Hami     | 17-FEB-1995 | +989050647748 | hamid.hami@gmail.com | NL20INGB0001234567 |

