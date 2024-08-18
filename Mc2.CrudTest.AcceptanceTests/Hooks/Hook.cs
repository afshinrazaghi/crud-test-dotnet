using Mc2.CrudTest.AcceptanceTests.Fixtures;
using System;
using TechTalk.SpecFlow;

namespace Mc2.CrudTest.AcceptanceTests.Hooks
{
    [Binding]
    public class Hooks
    {
        private readonly MsSqlFixture _msSqlFixture;
        private readonly MongoDbFixture _mongoDbFixture;

        public Hooks(MsSqlFixture msSqlFixture, MongoDbFixture mongoDbFixture)
        {
            this._msSqlFixture = msSqlFixture;
            this._mongoDbFixture = mongoDbFixture;
        }

        [BeforeScenario]
        public async Task BeforeScenario()
        {
            await _msSqlFixture.InitializeAsync();
            await _mongoDbFixture.InitializeAsync();
        }

        [AfterScenario]
        public async void AfterScenario()
        {
            await _msSqlFixture.DisposeAsync();
            await _mongoDbFixture.DisposeAsync();
        }
    }
}