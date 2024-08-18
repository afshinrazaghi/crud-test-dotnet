using Mc2.CrudTest.Presentation.Client;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Blazored.Toast;
using CurrieTechnologies.Razor.SweetAlert2;
namespace Mc2.CrudTest.Presentation.Client
{
    public partial class Program
    {
        public static string BaseEndPoint = "http://localhost:5093";
        public static string EndPoint
        {
            get
            {
                return $"{BaseEndPoint}/api/customer";
            }
        }
        public static async Task Main(string[] args)
        {
            WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(BaseEndPoint) /*new Uri(builder.HostEnvironment.BaseAddress)*/ });
            builder.Services.AddBlazoredToast();
            builder.Services.AddSweetAlert2(options =>
            {
                options.Theme = SweetAlertTheme.Default;
            });

            await builder.Build().RunAsync();
        }
    }
}