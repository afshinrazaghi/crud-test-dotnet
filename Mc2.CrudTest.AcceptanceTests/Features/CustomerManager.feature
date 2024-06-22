Feature: Customer Manager

As a an operator I wish to be able to Create, Update, Delete customers and list all customers
	
@create
Scenario: create a new customer
	Given I have a new customer with the following details
		| FirstName | LastName | DateOfBirth | PhoneNumber | Email                        | BankAccountNumber |
		| Afshin    | Razaghi  | 1990-04-13  | 231323456   | afshin.razaghi.net@gmail.com | 12 34 56 789      |
	When I create the customer
	Then the customer should be saved in the system
	And I should be able to retrieve the customer with the same details


@update
Scenario: update an existing customer
	Given an existing customer with following details
		| FirstName | LastName | DateOfBirth | PhoneNumber | Email                      | BankAccountNumber |
		| Reza      | Bahram   | 1985-06-13  | 556733455   | reaza.bahram.123@gmail.com | 55 33 23 154      |
	When I update the customer's details with the following information
		| FirstName | LastName | DateOfBirth | PhoneNumber | Email                   | BankAccountNumber |
		| Hamid     | Hami     | 1987-11-13  | 666722445   | hamid.hami.12@gmail.com | 22 15 44 831      |
	Then the customer's details should be updated successfully
	And the customer should have the following updated details
		| FirstName | LastName | DateOfBirth | PhoneNumber | Email                   | BankAccountNumber |
		| Hamid     | Hami     | 1987-11-13  | 666722445   | hamid.hami.12@gmail.com | 22 15 44 831      |


@delete
Scenario: delete a customer
	Given a customer with the following details
		| FirstName | LastName | DateOfBirth | PhoneNumber | Email                      | BankAccountNumber |
		| Reza      | Bahram   | 1985-06-13  | 231323456   | reaza.bahram.123@gmail.com | 55 33 23 154      |
	When I delete the customer with target Id
	Then the customer with target Id should not exist


@retrieveAllCustomers
Scenario: successfully retrieve all customers
	Given the following customers exists
		| FirstName | LastName | DateOfBirth | PhoneNumber | Email                        | BankAccountNumber |
		| Afshn     | Razaghi  | 1990-04-13  | 889732341   | afshin.razaghi.net@gmail.com | 32 33 25 123      |
		| Mostafa   | Moradi   | 1889-02-03  | 223341234   | mostafa.moradi@gmail.com     | 23 22 45 145      |
	When I retrieve all customers
	Then I should see the following customers
		| FirstName | LastName | DateOfBirth | PhoneNumber | Email                        | BankAccountNumber |
		| Afshn     | Razaghi  | 1990-04-13  | 889732341   | afshin.razaghi.net@gmail.com | 32 33 25 123      |
		| Mostafa   | Moradi   | 1889-02-03  | 223341234   | mostafa.moradi@gmail.com     | 23 22 45 145      |

