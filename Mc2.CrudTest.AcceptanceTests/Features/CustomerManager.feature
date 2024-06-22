Feature: Customer Manager

As a an operator I wish to be able to Create, Update, Delete customers and list all customers
	
@mytag
Scenario: create a new customer
	Given I have a new customer with the following details
	| FirstName         | John                 |
	| LastName          | Smith                |
	| DateOfBirth       | 1990-04-13           |
	| PhoneNumber       | 123-4560-7890        |
	| Email             | john.smith@gmail.com |
	| BankAccountNumber | 123456789            |
	When I create the customer
	Then the customer should be saved in the system 
	And I should be able to retrieve the customer with the same details 