﻿using FluentAssertions;
using Mc2.CrudTest.Presentation.Client.Tests.Models;
using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mc2.CrudTest.Presentation.Client.Tests.Fixture
{
    public class PlaywrightFixture : IAsyncLifetime
    {
        #region Properties
        public const string PlaywrightCollection = nameof(PlaywrightCollection);
        public IPlaywright? Playwright { get; set; }
        public Lazy<Task<IBrowser>>? ChromiumBrowser { get; private set; }
        public Lazy<Task<IBrowser>>? FirefoxBrowser { get; private set; }
        public Lazy<Task<IBrowser>>? WebkitBrowser { get; private set; }
        #endregion

        #region public Methods
        public async Task GotoPageAsync(
          string url,
          Func<IPage, Task> testHandler,
          Browser browserType)
        {
            IBrowser browser = await SelectBrowserAsync(browserType);
            await using IBrowserContext context = await browser
                .NewContextAsync(new BrowserNewContextOptions
                {
                    IgnoreHTTPSErrors = true
                });

            IPage page = await context.NewPageAsync();
            page.Should().NotBeNull();

            try
            {
                IResponse? gotoResult = await page.GotoAsync(
                    url, new PageGotoOptions
                    {
                        WaitUntil = WaitUntilState.NetworkIdle
                    });

                gotoResult.Should().NotBeNull();
                await gotoResult!.FinishedAsync();
                gotoResult.Ok.Should().BeTrue();

                await testHandler(page);
            }
            finally
            {
                await page.CloseAsync();
            }
        }

        #endregion

        #region IAsyncLifetime

        public async Task InitializeAsync()
        {
            InstallPlaywright();

            Playwright = await Microsoft.Playwright.Playwright.CreateAsync();

            ChromiumBrowser = new Lazy<Task<IBrowser>>(Playwright.Chromium.LaunchAsync());
            FirefoxBrowser = new Lazy<Task<IBrowser>>(Playwright.Firefox.LaunchAsync());
            WebkitBrowser = new Lazy<Task<IBrowser>>(Playwright.Webkit.LaunchAsync());

        }
        public async Task DisposeAsync()
        {
            if (Playwright != null) { }
            {
                if (ChromiumBrowser != null && ChromiumBrowser.IsValueCreated)
                {
                    IBrowser browser = await ChromiumBrowser.Value;
                    await browser.DisposeAsync();
                }

                if (FirefoxBrowser != null && FirefoxBrowser.IsValueCreated)
                {
                    IBrowser browser = await FirefoxBrowser.Value;
                    await browser.DisposeAsync();
                }

                if (WebkitBrowser != null && WebkitBrowser.IsValueCreated)
                {
                    IBrowser browser = await WebkitBrowser.Value;
                    await browser.DisposeAsync();
                }

                Playwright?.Dispose();
                Playwright = null;
            }
        }
        #endregion

        #region private methods
        private static void InstallPlaywright()
        {
            int exitCode = Microsoft.Playwright.Program.Main(new[] { "install-deps" });
            if (exitCode != 0)
            {
                throw new Exception($"Playwright exited with code {exitCode} on install-deps");
            }

            exitCode = Microsoft.Playwright.Program.Main(new[] { "install" });
            if (exitCode != 0)
            {
                throw new Exception($"Playwright exited with code {exitCode} on install");
            }
        }

        private Task<IBrowser> SelectBrowserAsync(Browser browser)
        {
            return browser switch
            {
                Browser.Chromium => ChromiumBrowser.Value,
                Browser.Firefox => FirefoxBrowser.Value,
                Browser.Webkit => WebkitBrowser.Value,
                _ => throw new NotImplementedException(),
            };
        }
        #endregion

        #region CollectionDefinition

        [CollectionDefinition(PlaywrightCollection)]
        public class PlaywrightCollectionDefinition : ICollectionFixture<PlaywrightFixture>
        {

        }
        #endregion
    }
}
