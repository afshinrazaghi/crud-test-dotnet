using System;
using TechTalk.SpecFlow;

namespace Mc2.CrudTest.AcceptanceTests
{
    [Binding]
    public class UserCanApplyCURDOperationsAgainstBrowserStepDefinitions
    {
        [Given(@"user is on the customers list page")]
        public void GivenUserIsOnTheCustomersListPage()
        {
            throw new PendingStepException();
        }

        [When(@"user clicks on the create button")]
        public void WhenUserClicksOnTheCreateButton()
        {
            throw new PendingStepException();
        }

        [Then(@"user should navigate to ""([^""]*)"" page")]
        public void ThenUserShouldNavigateToPage(string p0)
        {
            throw new PendingStepException();
        }

        [When(@"user fill in the customer details with following information")]
        public void WhenUserFillInTheCustomerDetailsWithFollowingInformation(Table table)
        {
            throw new PendingStepException();
        }

        [When(@"user submit the customer create form")]
        public void WhenUserSubmitTheCustomerCreateForm()
        {
            throw new PendingStepException();
        }

        [Then(@"user should see success notification")]
        public void ThenUserShouldSeeSuccessNotification()
        {
            throw new PendingStepException();
        }

        [Then(@"user should see (.*) record of customer with following information in customers list page")]
        public void ThenUserShouldSeeRecordOfCustomerWithFollowingInformationInCustomersListPage(int p0, Table table)
        {
            throw new PendingStepException();
        }

        [When(@"user clicks on edit button for user with email of '([^']*)'")]
        public void WhenUserClicksOnEditButtonForUserWithEmailOf(string p0)
        {
            throw new PendingStepException();
        }

        [When(@"user fill in customer details with following information")]
        public void WhenUserFillInCustomerDetailsWithFollowingInformation(Table table)
        {
            throw new PendingStepException();
        }

        [When(@"user submit the customer update form")]
        public void WhenUserSubmitTheCustomerUpdateForm()
        {
            throw new PendingStepException();
        }

        [When(@"user clicks on remove button for user with email of '([^']*)'")]
        public void WhenUserClicksOnRemoveButtonForUserWithEmailOf(string p0)
        {
            throw new PendingStepException();
        }

        [Then(@"user should see a ""([^""]*)""")]
        public void ThenUserShouldSeeA(string p0)
        {
            throw new PendingStepException();
        }

        [When(@"user clicks on ""([^""]*)"" button")]
        public void WhenUserClicksOnButton(string ok)
        {
            throw new PendingStepException();
        }

        [Then(@"user should see ""([^""]*)""")]
        public void ThenUserShouldSee(string p0)
        {
            throw new PendingStepException();
        }

        [Then(@"user should see (.*) record of customer with the following information on customers list page")]
        public void ThenUserShouldSeeRecordOfCustomerWithTheFollowingInformationOnCustomersListPage(int p0, Table table)
        {
            throw new PendingStepException();
        }

        [Then(@"user should see ""([^""]*)"" text on customers list page")]
        public void ThenUserShouldSeeTextOnCustomersListPage(string p0)
        {
            throw new PendingStepException();
        }
    }
}
