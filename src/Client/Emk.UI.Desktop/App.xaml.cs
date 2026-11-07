using System.Net.Http;
using System.Windows;
using Emk.Core.Services;
using Emk.Infrastructure.ApiClient;
using Emk.Infrastructure.LocalStorage;
using Emk.Security.Auth;
using Emk.UI.Desktop.ViewModels;
using Emk.UI.Desktop.Views;

namespace Emk.UI.Desktop
{
    /// <summary>Точка сборки (composition root): единственное место, где модули связываются между собой.</summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var clock = TimeProvider.System;
            var tokens = new SessionTokenProvider(clock);
            var http = new HttpClient { BaseAddress = new Uri("http://localhost:5080/v1/") };
            var api = new EmkApiClient(http, tokens);
            var service = new AppointmentService(api, new JsonSlotCache(), clock);

            new MainWindow(new AppointmentViewModel(service)).Show();
        }
    }
}
