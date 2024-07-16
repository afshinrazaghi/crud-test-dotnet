using FluentAssertions;
using Mc2.CrudTest.Presentation.Application.Common.Interfaces.Persistence.Query;
using Mc2.CrudTest.Presentation.Client.Tests.Fixture;
using Mc2.CrudTest.Presentation.Infrastructure.Command.Context;
using Mc2.CrudTest.Presentation.Infrastructure.Query.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Mc2.CrudTest.Presentation.Application.Features.Customers.CommandHandlers;
using Mc2.CrudTest.Presentation.Client.Tests.WebApplications;
using System.Diagnostics;
using Mc2.CrudTest.Presentation.Infrastructure.Command.Persistence;
using Mc2.CrudTest.Presentation.Infrastructure.Command;
using NSubstitute;
using Mc2.CrudTest.Presentation.Shared.SharedKernel.Command;
using MediatR;
using Microsoft.Extensions.Logging;
using Mc2.CrudTest.Presentation.Domain.Factories;
using Bogus;
using Mc2.CrudTest.Presentation.Domain.Entities.CustomerAggregate;
using Mc2.CrudTest.Presentation.Domain.ValueObjects;
using Mc2.CrudTest.Presentation.Shared.Validators;
using Mc2.CrudTest.Presentation.Application.Features.Customers.Commands;
using Mc2.CrudTest.Presentation.Shared.Extensions;
using System.Net.Mime;
using Mc2.CrudTest.Presentation.Shared.Models;
using Mc2.CrudTest.Presentation.Application.Features.Customers.Responses;
using Castle.Core.Resource;
using Microsoft.Extensions.Options;
using Mc2.CrudTest.Presentation.Application.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Mc2.CrudTest.Presentation.Client.Tests
{
    [Collection(PlaywrightFixture.PlaywrightCollection)]
    public class CustomerCRUDTests : IClassFixture<MongoDbFixture>, IClassFixture<MsSqlFixture>, IDisposable
    {
        private readonly PlaywrightFixture _playwrightFixture;
        private readonly MongoDbFixture _mongoDbFixture;
        private readonly MsSqlFixture _msSqlFixture;
        public readonly string ClientBaseUrl = "http://localhost:5095";
        private readonly string ServerBaseUrl = "http://localhost:5093";
        private readonly Process _blazorProcess;
        WebApplicationFactory<Mc2.CrudTest.Presentation.Program> _webApplicationFactory { get; }

        public CustomerCRUDTests(MongoDbFixture mongoFixture, MsSqlFixture msSqlFixture, PlaywrightFixture playwrightFixture)
        {
            _playwrightFixture = playwrightFixture;
            _mongoDbFixture = mongoFixture;
            _msSqlFixture = msSqlFixture;
            _webApplicationFactory = InitializeWebAppFactory();

            _blazorProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = "run --project ../../../../Mc2.CrudTest.Presentation/Client",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };

            _blazorProcess.Start();

            // Wait for the app to start
            Task.Delay(10000).Wait(); // Adjust the delay as necessary
        }

        #region Create Customer
        [Fact]
        public async Task CreateNewCustomer_WhenCalledWithValidInfo_ShouldAddedToCustomerList()
        {
            // Arrange
            var httpClient = _webApplicationFactory.CreateClient(CreateClientOptions());
            await ClearCustomers();

            var customer = CustomerFactory.Create(
                "Afshin",
                "Razaghi",
                new DateTime(1990, 04, 12),
                PhoneNumber.Create("+989195512635").Value,
                Email.Create("afshin.razaghi.net@gmail.com").Value,
                BankAccountNumber.Create("NL91ABNA0417164300")
            );

            // Act
            // Assert

            await _playwrightFixture.GotoPageAsync(
                $"{ClientBaseUrl}/customers",
                async (page) =>
                {
                    await page.ClickAsync("text=Create");
                    await page.WaitForURLAsync($"{ClientBaseUrl}/createCustomer");
                    await page.FillAsync("input[name='firstName']", customer.FirstName);
                    await page.FillAsync("input[name='lastName']", customer.LastName);
                    await page.FillAsync("input[name='email']", customer.Email.Value);
                    await page.FillAsync("input[name='phoneNumber']", customer.PhoneNumber.Value);
                    await page.FillAsync("input[name='bankAccountNumber']", customer.BankAccountNumber.Value);
                    await page.FillAsync("input[name='dateOfBirth']", customer.DateOfBirth.ToString("yyyy-MM-dd"));
                    await page.ClickAsync("button[type=submit]");
                    await page.WaitForURLAsync($"{ClientBaseUrl}/customers");
                    (await page.Locator("text=Customer List").IsVisibleAsync()).Should().BeTrue();
                    await page.WaitForSelectorAsync("table tbody");
                    //var imgByteArray = await page.ScreenshotAsync();
                    //File.WriteAllBytes($"D://files//images//{Guid.NewGuid()}.jpg", imgByteArray);

                    var rows = await page.QuerySelectorAllAsync("tbody tr");
                    rows.Count.Should().Be(1);
                    var row = rows[0];
                    var cells = await row.QuerySelectorAllAsync("td");
                    (await cells[0].InnerTextAsync()).Should().Be(customer.FirstName);
                    (await cells[1].InnerTextAsync()).Should().Be(customer.LastName);
                    (await cells[2].InnerTextAsync()).Should().Be(customer.Email.Value);
                    (await cells[3].InnerTextAsync()).Should().Be(customer.PhoneNumber.Value);
                    (await cells[4].InnerTextAsync()).Should().Be(customer.BankAccountNumber.Value);
                    Convert.ToDateTime((await cells[5].InnerTextAsync())).Should().Be(customer.DateOfBirth);
                },
                Models.Browser.Chromium
             );
        }

        [Fact]
        public async Task CreateNewCustomer_WhenDuplicateEmail_ReturnsError()
        {
            // Arrange
            var httpClient = _webApplicationFactory.CreateClient(CreateClientOptions());
            await ClearCustomers();

            #region Create a customer
            var builder = new DbContextOptionsBuilder<WriteDbContext>()
                    .UseSqlServer(_msSqlFixture.Container.GetConnectionString());
            var writeDbContext = new WriteDbContext(builder.Options);

            var repository = new CustomerWriteOnlyRepository(writeDbContext);
            var unitOfWork = new UnitOfWork(
                writeDbContext,
                Substitute.For<IEventStoreRepository>(),
                Substitute.For<IMediator>(),
                Substitute.For<ILogger<UnitOfWork>>()
            );

            var customer = new Faker<Customer>()
                .RuleFor(customer => customer.FirstName, faker => faker.Person.FirstName)
                .RuleFor(customer => customer.LastName, faker => faker.Person.LastName)
                .RuleFor(customer => customer.Email, faker => Email.Create(faker.Person.Email).Value)
                .RuleFor(customer => customer.PhoneNumber, faker => PhoneNumber.Create("+989195512635").Value)
                .RuleFor(customer => customer.BankAccountNumber, faker => BankAccountNumber.Create("NL91ABNA0417164300").Value)
                .RuleFor(customer => customer.DateOfBirth, faker => faker.Person.DateOfBirth)
                .Generate();
            repository.Add(customer);
            await unitOfWork.SaveChangesAsync();
            writeDbContext.ChangeTracker.Clear();
            #endregion
            // Act

            // Assert
            await _playwrightFixture.GotoPageAsync(
                $"{ClientBaseUrl}/customers",
                async (page) =>
                {
                    await page.ClickAsync("text=Create");
                    await page.WaitForURLAsync($"{ClientBaseUrl}/createCustomer");
                    await page.FillAsync("input[name='firstName']", customer.FirstName);
                    await page.FillAsync("input[name='lastName']", customer.LastName);
                    await page.FillAsync("input[name='email']", customer.Email.Value);
                    await page.FillAsync("input[name='phoneNumber']", customer.PhoneNumber.Value);
                    await page.FillAsync("input[name='bankAccountNumber']", customer.BankAccountNumber.Value);
                    await page.FillAsync("input[name='dateOfBirth']", customer.DateOfBirth.ToString("yyyy-MM-dd"));
                    await page.ClickAsync("button[type=submit]");
                    await page.WaitForSelectorAsync(".blazored-toast-error");
                    (await page.Locator(".blazored-toast-error").InnerTextAsync()).Should().Be("email already exists");
                },
                Models.Browser.Chromium
             );
        }

        [Fact]
        public async Task CreateNewCustomer_WhenPhoneNumberInvalid_ReturnsError()
        {
            // Arrange
            var httpClient = _webApplicationFactory.CreateClient(CreateClientOptions());
            await ClearCustomers();

            var customer = new Faker<Customer>()
               .RuleFor(customer => customer.FirstName, faker => faker.Person.FirstName)
               .RuleFor(customer => customer.LastName, faker => faker.Person.LastName)
               .RuleFor(customer => customer.Email, faker => Email.Create("afshin.razaghi.net@gmail.com").Value)
               .RuleFor(customer => customer.BankAccountNumber, faker => BankAccountNumber.Create("NL91ABNA0417164300").Value)
               .RuleFor(customer => customer.DateOfBirth, faker => faker.Person.DateOfBirth)
               .Generate();

            // Act
            // Assert
            await _playwrightFixture.GotoPageAsync(
                $"{ClientBaseUrl}/customers",
                async (page) =>
                {
                    await page.ClickAsync("text=Create");
                    await page.WaitForURLAsync($"{ClientBaseUrl}/createCustomer");
                    await page.FillAsync("input[name='firstName']", customer.FirstName);
                    await page.FillAsync("input[name='lastName']", customer.LastName);
                    await page.FillAsync("input[name='email']", customer.Email.Value);
                    await page.FillAsync("input[name='phoneNumber']", "09195512635");
                    await page.FillAsync("input[name='bankAccountNumber']", customer.BankAccountNumber.Value);
                    await page.FillAsync("input[name='dateOfBirth']", customer.DateOfBirth.ToString("yyyy-MM-dd"));
                    await page.ClickAsync("button[type=submit]");
                    await page.WaitForSelectorAsync(".blazored-toast-error");
                    (await page.Locator(".blazored-toast-error").InnerTextAsync()).Should().Be("Mobile Number is not valid");
                },
                Models.Browser.Chromium
             );


        }

        [Fact]
        public async Task CreateNewCustomer_WhenEmailInvalid_ReturnsError()
        {
            // Arrange
            var httpClient = _webApplicationFactory.CreateClient(CreateClientOptions());
            await ClearCustomers();

            var customer = new Faker<Customer>()
               .RuleFor(customer => customer.FirstName, faker => faker.Person.FirstName)
               .RuleFor(customer => customer.LastName, faker => faker.Person.LastName)
               .RuleFor(customer => customer.PhoneNumber, faker => PhoneNumber.Create("+989195512635").Value)
               .RuleFor(customer => customer.BankAccountNumber, faker => BankAccountNumber.Create("NL91ABNA0417164300").Value)
               .RuleFor(customer => customer.DateOfBirth, faker => faker.Person.DateOfBirth)
               .Generate();

            // Act
            // Assert
            await _playwrightFixture.GotoPageAsync(
                $"{ClientBaseUrl}/customers",
                async (page) =>
                {
                    await page.ClickAsync("text=Create");
                    await page.WaitForURLAsync($"{ClientBaseUrl}/createCustomer");
                    await page.FillAsync("input[name='firstName']", customer.FirstName);
                    await page.FillAsync("input[name='lastName']", customer.LastName);
                    await page.FillAsync("input[name='email']", "abc.def");
                    await page.FillAsync("input[name='phoneNumber']", customer.PhoneNumber.Value);
                    await page.FillAsync("input[name='bankAccountNumber']", customer.BankAccountNumber.Value);
                    await page.FillAsync("input[name='dateOfBirth']", customer.DateOfBirth.ToString("yyyy-MM-dd"));
                    await page.ClickAsync("button[type=submit]");
                    await page.WaitForSelectorAsync(".blazored-toast-error");
                    (await page.Locator(".blazored-toast-error").InnerTextAsync()).Should().Be("'Email' is not a valid email address.");
                },
                Models.Browser.Chromium
             );


        }

        [Fact]
        public async Task CreateNewCustomer_WhenEmailBankAccountNumberInvalid_ReturnsError()
        {
            // Arrange
            var httpClient = _webApplicationFactory.CreateClient(CreateClientOptions());
            await ClearCustomers();

            var customer = new Faker<Customer>()
               .RuleFor(customer => customer.FirstName, faker => faker.Person.FirstName)
               .RuleFor(customer => customer.LastName, faker => faker.Person.LastName)
               .RuleFor(customer => customer.Email, faker => Email.Create(faker.Person.Email).Value)
               .RuleFor(customer => customer.PhoneNumber, faker => PhoneNumber.Create("+989195512635").Value)
               .RuleFor(customer => customer.DateOfBirth, faker => faker.Person.DateOfBirth)
               .Generate();

            // Act
            // Assert
            await _playwrightFixture.GotoPageAsync(
                $"{ClientBaseUrl}/customers",
                async (page) =>
                {
                    await page.ClickAsync("text=Create");
                    await page.WaitForURLAsync($"{ClientBaseUrl}/createCustomer");
                    await page.FillAsync("input[name='firstName']", customer.FirstName);
                    await page.FillAsync("input[name='lastName']", customer.LastName);
                    await page.FillAsync("input[name='email']", customer.Email.Value);
                    await page.FillAsync("input[name='phoneNumber']", customer.PhoneNumber.Value);
                    await page.FillAsync("input[name='bankAccountNumber']", "NNNLLLLLM");
                    await page.FillAsync("input[name='dateOfBirth']", customer.DateOfBirth.ToString("yyyy-MM-dd"));
                    await page.ClickAsync("button[type=submit]");
                    await page.WaitForSelectorAsync(".blazored-toast-error");
                    (await page.Locator(".blazored-toast-error").InnerTextAsync()).Should().Be("Back Account Number is not valid");
                },
                Models.Browser.Chromium
             );


        }
        #endregion

        #region Update Customer

        [Fact]
        public async Task UpdateExistingCustomer_WhenCalledWithValidInfo_ReturnsTrueAndUpdateCustomer()
        {
            // Arrange
            var httpClient = _webApplicationFactory.CreateClient(CreateClientOptions());
            await ClearCustomers();

            var command = new Faker<CreateCustomerCommand>()
              .RuleFor(command => command.FirstName, faker => faker.Person.FirstName)
              .RuleFor(command => command.LastName, faker => faker.Person.LastName)
              .RuleFor(command => command.DateOfBirth, faker => faker.Person.DateOfBirth)
              .RuleFor(command => command.PhoneNumber, faker => "+989195512635")
              .RuleFor(command => command.Email, faker => faker.Person.Email)
              .RuleFor(command => command.BankAccountNumber, faker => "NL91ABNA0417164300")
              .Generate(2);

            //Act & Assert
            var customer = command[0];
            var updateCustomerInfo = command[1];

            await _playwrightFixture.GotoPageAsync(
              $"{ClientBaseUrl}/customers",
              async (page) =>
              {
                  await page.ClickAsync("text=Create");
                  await page.WaitForURLAsync($"{ClientBaseUrl}/createCustomer");
                  await page.FillAsync("input[name='firstName']", customer.FirstName);
                  await page.FillAsync("input[name='lastName']", customer.LastName);
                  await page.FillAsync("input[name='email']", customer.Email);
                  await page.FillAsync("input[name='phoneNumber']", customer.PhoneNumber);
                  await page.FillAsync("input[name='bankAccountNumber']", customer.BankAccountNumber);
                  await page.FillAsync("input[name='dateOfBirth']", customer.DateOfBirth.ToString("yyyy-MM-dd"));
                  await page.ClickAsync("button[type=submit]");
                  await page.WaitForURLAsync($"{ClientBaseUrl}/customers");
                  (await page.Locator("text=Customer List").IsVisibleAsync()).Should().BeTrue();
                  await page.WaitForSelectorAsync("table tbody");
                  //var imgByteArray = await page.ScreenshotAsync();
                  //File.WriteAllBytes($"D://files//images//{Guid.NewGuid()}.jpg", imgByteArray);

                  await page.ClickAsync("button[name=edit]");
                  await page.WaitForURLAsync((url) => string.Equals(url, $"{ClientBaseUrl}/editCustomer/{customer.Email}", StringComparison.OrdinalIgnoreCase));
                  await page.WaitForSelectorAsync("input[name='firstName']:not([value=''])");

                  await page.FillAsync("input[name='firstName']", updateCustomerInfo.FirstName);
                  await page.FillAsync("input[name='lastName']", updateCustomerInfo.LastName);
                  await page.FillAsync("input[name='email']", updateCustomerInfo.Email);
                  await page.FillAsync("input[name='phoneNumber']", "+989305734980");
                  await page.FillAsync("input[name='bankAccountNumber']", "NL91RABO0315273637");
                  await page.FillAsync("input[name='dateOfBirth']", updateCustomerInfo.DateOfBirth.ToString("yyyy-MM-dd"));
                  await page.ClickAsync("button[type=submit]");
                  await page.WaitForURLAsync($"{ClientBaseUrl}/customers");
                  await page.WaitForSelectorAsync("table tbody");
                  var rows = await page.QuerySelectorAllAsync("tbody tr"); ;
                  rows.Count.Should().Be(1);

                  var row = rows[0];
                  var cells = await row.QuerySelectorAllAsync("td");
                  (await cells[0].InnerTextAsync()).Should().Be(updateCustomerInfo.FirstName);
                  (await cells[1].InnerTextAsync()).Should().Be(updateCustomerInfo.LastName);
                  (await cells[2].InnerTextAsync()).Should().BeEquivalentTo(updateCustomerInfo.Email);
                  (await cells[3].InnerTextAsync()).Should().Be("+989305734980");
                  (await cells[4].InnerTextAsync()).Should().BeEquivalentTo("NL91RABO0315273637");
                  Convert.ToDateTime((await cells[5].InnerTextAsync())).Should().Be(updateCustomerInfo.DateOfBirth.Date);
              },
              Models.Browser.Chromium);
        }

        [Fact]
        public async Task UpdateExistingCustomer_WhenDuplicateEmail_ReturnsError()
        {
            // Arrange
            var httpClient = _webApplicationFactory.CreateClient(CreateClientOptions());
            await ClearCustomers();

            #region Create a customer
            var builder = new DbContextOptionsBuilder<WriteDbContext>()
                    .UseSqlServer(_msSqlFixture.Container.GetConnectionString());
            var writeDbContext = new WriteDbContext(builder.Options);
            var repository = new CustomerWriteOnlyRepository(writeDbContext);
            var unitOfWork = new UnitOfWork(
                writeDbContext,
                Substitute.For<IEventStoreRepository>(),
                Substitute.For<IMediator>(),
                Substitute.For<ILogger<UnitOfWork>>()
            );

            var customerOne = new Faker<Customer>()
                .RuleFor(customer => customer.FirstName, faker => faker.Person.FirstName)
                .RuleFor(customer => customer.LastName, faker => faker.Person.LastName)
                .RuleFor(customer => customer.Email, faker => Email.Create(faker.Person.Email).Value)
                .RuleFor(customer => customer.PhoneNumber, faker => PhoneNumber.Create("+989195512635").Value)
                .RuleFor(customer => customer.BankAccountNumber, faker => BankAccountNumber.Create("NL91ABNA0417164300").Value)
                .RuleFor(customer => customer.DateOfBirth, faker => faker.Person.DateOfBirth)
                .Generate();

            var customerTwo = new Faker<Customer>()
                .RuleFor(customer => customer.FirstName, faker => faker.Person.FirstName)
                .RuleFor(customer => customer.LastName, faker => faker.Person.LastName)
                .RuleFor(customer => customer.Email, faker => Email.Create(faker.Person.Email).Value)
                .RuleFor(customer => customer.PhoneNumber, faker => PhoneNumber.Create("+989305734980").Value)
                .RuleFor(customer => customer.BankAccountNumber, faker => BankAccountNumber.Create("NL91RABO0315273637").Value)
                .RuleFor(customer => customer.DateOfBirth, faker => faker.Person.DateOfBirth)
                .Generate();

            var customerThree = new Faker<Customer>()
                .RuleFor(customer => customer.FirstName, faker => faker.Person.FirstName)
                .RuleFor(customer => customer.LastName, faker => faker.Person.LastName)
                .RuleFor(customer => customer.Email, faker => Email.Create(faker.Person.Email).Value)
                .RuleFor(customer => customer.PhoneNumber, faker => PhoneNumber.Create("+989031263736").Value)
                .RuleFor(customer => customer.BankAccountNumber, faker => BankAccountNumber.Create("NL20INGB0001234567").Value)
                .RuleFor(customer => customer.DateOfBirth, faker => faker.Person.DateOfBirth)
                .Generate();

            writeDbContext.Add(customerOne);
            writeDbContext.Add(customerTwo);
            await writeDbContext.SaveChangesAsync();

            var synchronizeDb = new NoSqlDbContext(
               Options.Create(new Presentation.Shared.AppSettings.ConnectionOptions
               {
                   NoSqlConnection = _mongoDbFixture.Container.GetConnectionString()
               }),
               Substitute.For<ILogger<NoSqlDbContext>>()
           );

            var customerOneQueryModel = new CustomerQueryModel
            {
                Id = customerOne.Id,
                FirstName = customerOne.FirstName,
                LastName = customerOne.LastName,
                Email = customerOne.Email.Value,
                DateOfBirth = customerOne.DateOfBirth.Date,
                PhoneNumber = customerOne.PhoneNumber.Value,
                BankAccountNumber = customerOne.BankAccountNumber.Value
            };


            var customerTwoQueryModel = new CustomerQueryModel
            {
                Id = customerTwo.Id,
                FirstName = customerTwo.FirstName,
                LastName = customerTwo.LastName,
                Email = customerTwo.Email.Value,
                DateOfBirth = customerTwo.DateOfBirth.Date,
                PhoneNumber = customerTwo.PhoneNumber.Value,
                BankAccountNumber = customerTwo.BankAccountNumber.Value
            };

            var customerQueryModels = new List<CustomerQueryModel> { customerOneQueryModel, customerTwoQueryModel };
            await synchronizeDb.UpsertAsync<CustomerQueryModel>(customerOneQueryModel, f => f.Id.Equals(customerOneQueryModel.Id));
            await synchronizeDb.UpsertAsync<CustomerQueryModel>(customerTwoQueryModel, f => f.Id.Equals(customerTwoQueryModel.Id));

            #endregion
            // Act

            // Assert

            await _playwrightFixture.GotoPageAsync(
                $"{ClientBaseUrl}/customers",
                async (page) =>
                {
                    await page.WaitForSelectorAsync("button[name=edit]");
                    var rows = await page.QuerySelectorAllAsync("tbody tr");
                    var firstRow = rows[0];
                    var firstRowEditButton = await firstRow.QuerySelectorAsync("button[name=edit]");
                    firstRowEditButton.Should().NotBeNull();
                    await firstRowEditButton!.ClickAsync();
                    await page.WaitForURLAsync((url) => string.Equals(url, $"{ClientBaseUrl}/editCustomer/{customerOne.Email}", StringComparison.OrdinalIgnoreCase));
                    await page.WaitForSelectorAsync("input[name='firstName']:not([value=''])");

                    await page.FillAsync("input[name='firstName']", customerThree.FirstName);
                    await page.FillAsync("input[name='lastName']", customerThree.LastName);
                    await page.FillAsync("input[name='email']", customerTwo.Email.Value);
                    await page.FillAsync("input[name='phoneNumber']", customerThree.PhoneNumber.Value);
                    await page.FillAsync("input[name='bankAccountNumber']", customerThree.BankAccountNumber.Value);
                    await page.FillAsync("input[name='dateOfBirth']", customerThree.DateOfBirth.ToString("yyyy-MM-dd"));
                    await page.ClickAsync("button[type=submit]");
                    var imgByteArray = await page.ScreenshotAsync();
                    File.WriteAllBytes($"D://files//images//{Guid.NewGuid()}.jpg", imgByteArray);

                    await page.WaitForSelectorAsync(".blazored-toast-error");
                    (await page.Locator(".blazored-toast-error").InnerTextAsync()).Should().Be("email already exists");
                },
                Models.Browser.Chromium
            );

        }


        #endregion

        #region Remove Customer
        [Fact]
        public async Task DeleteCustomer_WhenCalled_RemoveCustomerFromTable()
        {
            // Arrange
            var httpClient = _webApplicationFactory.CreateClient(CreateClientOptions());
            await ClearCustomers();

            var builder = new DbContextOptionsBuilder<WriteDbContext>()
                    .UseSqlServer(_msSqlFixture.Container.GetConnectionString());
            var writeDbContext = new WriteDbContext(builder.Options);
            var repository = new CustomerWriteOnlyRepository(writeDbContext);
            var unitOfWork = new UnitOfWork(
                writeDbContext,
                Substitute.For<IEventStoreRepository>(),
                Substitute.For<IMediator>(),
                Substitute.For<ILogger<UnitOfWork>>()
            );

            var customer = new Faker<Customer>()
                .RuleFor(customer => customer.FirstName, faker => faker.Person.FirstName)
                .RuleFor(customer => customer.LastName, faker => faker.Person.LastName)
                .RuleFor(customer => customer.Email, faker => Email.Create(faker.Person.Email).Value)
                .RuleFor(customer => customer.PhoneNumber, faker => PhoneNumber.Create("+989195512635").Value)
                .RuleFor(customer => customer.BankAccountNumber, faker => BankAccountNumber.Create("NL91ABNA0417164300").Value)
                .RuleFor(customer => customer.DateOfBirth, faker => faker.Person.DateOfBirth.Date)
                .Generate();
            repository.Add(customer);
            await unitOfWork.SaveChangesAsync();
            writeDbContext.ChangeTracker.Clear();

            var synchronizeDb = new NoSqlDbContext(
                Options.Create(new Presentation.Shared.AppSettings.ConnectionOptions
                {
                    NoSqlConnection = _mongoDbFixture.Container.GetConnectionString()
                }),
                Substitute.For<ILogger<NoSqlDbContext>>()
            );

            var customerQueryModel = new CustomerQueryModel
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email.Value,
                DateOfBirth = customer.DateOfBirth.Date,
                PhoneNumber = customer.PhoneNumber.Value,
                BankAccountNumber = customer.BankAccountNumber.Value
            };

            await synchronizeDb.UpsertAsync<CustomerQueryModel>(customerQueryModel, f => f.Id.Equals(customer.Id));

            // Act & Assert

            await _playwrightFixture.GotoPageAsync(
             $"{ClientBaseUrl}/customers",
             async (page) =>
             {
                 var imgByteArray = await page.ScreenshotAsync();
                 File.WriteAllBytes($"D://files//images//{Guid.NewGuid()}.jpg", imgByteArray);

                 await page.WaitForSelectorAsync("table tbody");
                 await page.ClickAsync("button[name=remove]");

                 imgByteArray = await page.ScreenshotAsync();
                 File.WriteAllBytes($"D://files//images//{Guid.NewGuid()}.jpg", imgByteArray);

                 await page.WaitForSelectorAsync(".swal2-confirm");
                 await page.ClickAsync(".swal2-confirm");
                 await page.WaitForSelectorAsync("h2:text('Success')");
                 await page.ClickAsync(".swal2-confirm");
                 await page.WaitForSelectorAsync("table tbody");
                 var rows = await page.QuerySelectorAllAsync("tbody tr");
                 rows.Count.Should().Be(1);

                 imgByteArray = await page.ScreenshotAsync();
                 File.WriteAllBytes($"D://files//images//{Guid.NewGuid()}.jpg", imgByteArray);

                 var row = rows[0];
                 var cells = await row.QuerySelectorAllAsync("td");
                 cells.Count().Should().Be(1);
                 (await cells[0].InnerTextAsync()).Should().Be("No Records Found");

                 imgByteArray = await page.ScreenshotAsync();
                 File.WriteAllBytes($"D://files//images//{Guid.NewGuid()}.jpg", imgByteArray);
             },
             Models.Browser.Chromium);


        }
        #endregion

        #region Get All Customers
        [Fact]
        public async Task CustomersPage_WhenCalledWithExistCustomers_ShowsExistingCustomers()
        {
            // Arrange
            var httpClient = _webApplicationFactory.CreateClient(CreateClientOptions());
            await ClearCustomers();

            var builder = new DbContextOptionsBuilder<WriteDbContext>()
                    .UseSqlServer(_msSqlFixture.Container.GetConnectionString());
            var writeDbContext = new WriteDbContext(builder.Options);

            var repository = new CustomerWriteOnlyRepository(writeDbContext);
            var unitOfWork = new UnitOfWork(
                writeDbContext,
                Substitute.For<IEventStoreRepository>(),
                Substitute.For<IMediator>(),
                Substitute.For<ILogger<UnitOfWork>>()
            );

            var customerOne = new Faker<Customer>()
                .RuleFor(customer => customer.FirstName, faker => faker.Person.FirstName)
                .RuleFor(customer => customer.LastName, faker => faker.Person.LastName)
                .RuleFor(customer => customer.Email, faker => Email.Create(faker.Person.Email).Value)
                .RuleFor(customer => customer.PhoneNumber, faker => PhoneNumber.Create("+989195512635").Value)
                .RuleFor(customer => customer.BankAccountNumber, faker => BankAccountNumber.Create("NL91ABNA0417164300").Value)
                .RuleFor(customer => customer.DateOfBirth, faker => faker.Person.DateOfBirth)
                .Generate();

            var customerTwo = new Faker<Customer>()
                .RuleFor(customer => customer.FirstName, faker => faker.Person.FirstName)
                .RuleFor(customer => customer.LastName, faker => faker.Person.LastName)
                .RuleFor(customer => customer.Email, faker => Email.Create(faker.Person.Email).Value)
                .RuleFor(customer => customer.PhoneNumber, faker => PhoneNumber.Create("+989305734980").Value)
                .RuleFor(customer => customer.BankAccountNumber, faker => BankAccountNumber.Create("NL91RABO0315273637").Value)
                .RuleFor(customer => customer.DateOfBirth, faker => faker.Person.DateOfBirth)
                .Generate();

            var customerThree = new Faker<Customer>()
                .RuleFor(customer => customer.FirstName, faker => faker.Person.FirstName)
                .RuleFor(customer => customer.LastName, faker => faker.Person.LastName)
                .RuleFor(customer => customer.Email, faker => Email.Create(faker.Person.Email).Value)
                .RuleFor(customer => customer.PhoneNumber, faker => PhoneNumber.Create("+989031263736").Value)
                .RuleFor(customer => customer.BankAccountNumber, faker => BankAccountNumber.Create("NL20INGB0001234567").Value)
                .RuleFor(customer => customer.DateOfBirth, faker => faker.Person.DateOfBirth)
                .Generate();

            repository.Add(customerOne);
            repository.Add(customerTwo);
            repository.Add(customerThree);
            await unitOfWork.SaveChangesAsync();
            writeDbContext.ChangeTracker.Clear();

            var synchronizeDb = new NoSqlDbContext(
               Options.Create(new Presentation.Shared.AppSettings.ConnectionOptions
               {
                   NoSqlConnection = _mongoDbFixture.Container.GetConnectionString()
               }),
               Substitute.For<ILogger<NoSqlDbContext>>()
           );

            var customerOneQueryModel = new CustomerQueryModel
            {
                Id = customerOne.Id,
                FirstName = customerOne.FirstName,
                LastName = customerOne.LastName,
                Email = customerOne.Email.Value,
                DateOfBirth = customerOne.DateOfBirth.Date,
                PhoneNumber = customerOne.PhoneNumber.Value,
                BankAccountNumber = customerOne.BankAccountNumber.Value
            };


            var customerTwoQueryModel = new CustomerQueryModel
            {
                Id = customerTwo.Id,
                FirstName = customerTwo.FirstName,
                LastName = customerTwo.LastName,
                Email = customerTwo.Email.Value,
                DateOfBirth = customerTwo.DateOfBirth.Date,
                PhoneNumber = customerTwo.PhoneNumber.Value,
                BankAccountNumber = customerTwo.BankAccountNumber.Value
            };


            var customerThreeQueryModel = new CustomerQueryModel
            {
                Id = customerThree.Id,
                FirstName = customerThree.FirstName,
                LastName = customerThree.LastName,
                Email = customerThree.Email.Value,
                DateOfBirth = customerThree.DateOfBirth.Date,
                PhoneNumber = customerThree.PhoneNumber.Value,
                BankAccountNumber = customerThree.BankAccountNumber.Value
            };

            var customerQueryModels = new List<CustomerQueryModel> { customerOneQueryModel, customerTwoQueryModel, customerThreeQueryModel };
            customerQueryModels = customerQueryModels.OrderBy(x => x.FirstName).ThenByDescending(x => x.DateOfBirth).ToList() ;

            await synchronizeDb.UpsertAsync<CustomerQueryModel>(customerOneQueryModel, f => f.Id.Equals(customerOneQueryModel.Id));
            await synchronizeDb.UpsertAsync<CustomerQueryModel>(customerTwoQueryModel, f => f.Id.Equals(customerTwoQueryModel.Id));
            await synchronizeDb.UpsertAsync<CustomerQueryModel>(customerThreeQueryModel, f => f.Id.Equals(customerThreeQueryModel.Id));

            // Act

            await _playwrightFixture.GotoPageAsync(
                $"{ClientBaseUrl}/customers",
                async (page) =>
                {
                    await page.WaitForSelectorAsync("button[name='edit']");

                    var imgByteArray = await page.ScreenshotAsync();
                    File.WriteAllBytes($"D://files//images//{Guid.NewGuid()}.jpg", imgByteArray);

                    var rows = await page.QuerySelectorAllAsync("tbody tr");

                    for (int i = 0; i < rows.Count; i++)
                    {
                        var row = rows[i];
                        var customerQueryModel = customerQueryModels[i];
                        var cells = await row.QuerySelectorAllAsync("td");

                        (await cells[0].InnerTextAsync()).Should().Be(customerQueryModel.FirstName);
                        (await cells[1].InnerTextAsync()).Should().Be(customerQueryModel.LastName);
                        (await cells[2].InnerTextAsync()).Should().BeEquivalentTo(customerQueryModel.Email);
                        (await cells[3].InnerTextAsync()).Should().Be(customerQueryModel.PhoneNumber);
                        (await cells[4].InnerTextAsync()).Should().BeEquivalentTo(customerQueryModel.BankAccountNumber);
                        Convert.ToDateTime((await cells[5].InnerTextAsync())).Date.Should().Be(customerQueryModel.DateOfBirth.Date);
                    }
                },
                Models.Browser.Chromium
            );

        }

        #endregion

        private WebApplicationFactory<Mc2.CrudTest.Presentation.Program> InitializeWebAppFactory(
          Action<IServiceCollection>? configureServices = null,
          Action<IServiceScope>? configureServiceScope = null)
        {
            return new WebTestingHostFactory<Mc2.CrudTest.Presentation.Program>()
                .WithWebHostBuilder(hostBuilder =>
                {
                    hostBuilder.UseUrls("http://localhost:5093");
                    hostBuilder.UseSetting("ConnectionStrings:SqlConnection", _msSqlFixture.Container.GetConnectionString());
                    hostBuilder.UseSetting("ConnectionStrings:NoSqlConnection", _mongoDbFixture.Container.GetConnectionString());

                    hostBuilder.UseEnvironment(Environments.Development);

                    hostBuilder.ConfigureServices(services =>
                    {
                        services.RemoveAll<WriteDbContext>();
                        services.RemoveAll<DbContextOptions<WriteDbContext>>();
                        services.RemoveAll<EventStoreDbContext>();
                        services.RemoveAll<DbContextOptions<EventStoreDbContext>>();
                        services.RemoveAll<ISynchronizeDb>();

                        services.AddDbContext<WriteDbContext>(
                            options => options.UseSqlServer(new SqlConnection(_msSqlFixture.Container.GetConnectionString()))
                        );

                        services.AddDbContext<EventStoreDbContext>(options =>
                        {
                            options.UseSqlServer(new SqlConnection(_msSqlFixture.Container.GetConnectionString()));
                        });

                        services.AddSingleton<IReadDbContext, NoSqlDbContext>();
                        services.AddSingleton<ISynchronizeDb, NoSqlDbContext>();

                        configureServices?.Invoke(services);

                        using var serviceProvider = services.BuildServiceProvider(true);
                        using var serviceScope = serviceProvider.CreateScope();

                        var writeDbContext = serviceScope.ServiceProvider.GetRequiredService<WriteDbContext>();
                        writeDbContext.Database.EnsureCreated();
                        writeDbContext.Database.ExecuteSqlRaw("Delete from Customers");

                        //services.AddSingleton(_ => Substitute.For<EventStoreDbContext>());
                        var eventStoreDbContext = serviceScope.ServiceProvider.GetRequiredService<EventStoreDbContext>();
                        eventStoreDbContext.Database.EnsureCreated();

                        configureServiceScope?.Invoke(serviceScope);

                        writeDbContext.Dispose();
                        eventStoreDbContext.Dispose();
                    });
                });
        }

        private static WebApplicationFactoryClientOptions CreateClientOptions() => new WebApplicationFactoryClientOptions { AllowAutoRedirect = false };

        public void Dispose()
        {
            if (!_blazorProcess.HasExited)
            {
                _blazorProcess.Kill();
            }
        }

        private async Task ClearCustomers()
        {
            var builder = new DbContextOptionsBuilder<WriteDbContext>()
                   .UseSqlServer(_msSqlFixture.Container.GetConnectionString());
            var writeDbContext = new WriteDbContext(builder.Options);
            await writeDbContext.Database.ExecuteSqlRawAsync("Delete From Customers");
            await writeDbContext.SaveChangesAsync();

            var noSqlDbContext = new NoSqlDbContext(
                  Options.Create(new Presentation.Shared.AppSettings.ConnectionOptions
                  {
                      NoSqlConnection = _mongoDbFixture.Container.GetConnectionString()
                  }), Substitute.For<ILogger<NoSqlDbContext>>());

            var asyncCursor = await noSqlDbContext.GetCollection<CustomerQueryModel>().FindAsync(Builders<CustomerQueryModel>.Filter.Empty);
            var records = await asyncCursor.ToListAsync();
            foreach (var record in records)
            {
                await noSqlDbContext.GetCollection<CustomerQueryModel>().DeleteOneAsync(Builders<CustomerQueryModel>.Filter.Eq(x => x.Id, record.Id));
            }
        }
    }
}