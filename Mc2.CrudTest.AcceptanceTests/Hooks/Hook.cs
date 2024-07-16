using Mc2.CrudTest.AcceptanceTests.Fixtures;
using System;
using TechTalk.SpecFlow;

namespace Mc2.CrudTest.AcceptanceTests.Hooks
{
    

    [Binding]
    public class Hooks
    {
        public Hooks()
        {
            
        }


        //private static MsSqlFixture msSqlFixture;
        //private static MongoDbFixture mongoDbFixture;

        //[BeforeTestRun]
        //public static async Task BeforeTestRun()
        //{
        //    msSqlFixture = new MsSqlFixture();
        //    mongoDbFixture = new MongoDbFixture();
        //    await msSqlFixture.InitializeAsync();
        //    await mongoDbFixture.InitializeAsync();
        //}

        //[AfterTestRun]
        //public static async Task AfterTestRun()
        //{
        //    await msSqlFixture?.DisposeAsync();
        //    await mongoDbFixture?.DisposeAsync();
        //}
    }
}