Feature: user can apply CURD operations against browser

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




@CRUDCustomerSuccessfully
Scenario: User Can Create, Edit, Delete And Read Customer From Client
	Given user is on the customers list page
	When user clicks on the create button
	Then user should navigate to "/createCustomer" page

	When user fill in the customer details with following information
		| FirstName | LastName | PhoneNumber   | Email              | BankAccountNumber  | DateOfBirth |
		| jane      | doe      | +989050647735 | jane.doe@gmail.com | NL20INGB0001234567 | 12-04-1990  |
	And user submit the customer create form
	Then user should see success notification
	And user should navigate to "/customers" page
	And user should see 1 record of customer with following information in customers list page
		| FirstName | LastName | PhoneNumber   | Email              | BankAccountNumber  | DateOfBirth |
		| jane      | doe      | +989050647735 | jane.doe@gmail.com | NL20INGB0001234567 | 12-04-1990  |

	When user clicks on edit button for user with email of 'jane.doe@gmail.com'
	Then user should navigate to "/editCustomer/jane.doe@gmail.com" page

	When user fill in customer details with following information
		| FirstName | LastName | PhoneNumber   | Email                | BankAccountNumber  | DateOfBirth |
		| john      | smith    | +989050327735 | john.smith@gmail.com | NL91ABNA0417164300 | 12-02-1994 |
	And user submit the customer update form
	Then user should see success notification
	And user should navigate to "/customers" page
	And user should see 1 record of customer with following information in customers list page
		| FirstName | LastName | PhoneNumber   | Email                | BankAccountNumber  | DateOfBirth |
		| john      | smith    | +989050327735 | john.smith@gmail.com | NL91ABNA0417164300 | 12-02-1994 |
	And user should see 0 record of customer with following information in customers list page
		| FirstName | LastName | PhoneNumber   | Email              | BankAccountNumber  | DateOfBirth |
		| jane      | doe      | +989050647735 | jane.doe@gmail.com | NL20INGB0001234567 | 12-04-1990  |

	When user clicks on remove button for user with email of 'john.smith@gmail.com'
	Then user should see a "confirm dialog"
	When user clicks on "Ok" button
	Then user should see "successfull dialog"
	When user clicks on "Ok" button
	Then user should see 0 record of customer with the following information on customers list page
		| FirstName | LastName | PhoneNumber   | Email                | BankAccountNumber  | DateOfBirth |
		| john      | smith    | +989050327735 | john.smith@gmail.com | NL91ABNA0417164300 | 12-02-1994 |
	And user should see "No Records Found" text on customers list page

	


