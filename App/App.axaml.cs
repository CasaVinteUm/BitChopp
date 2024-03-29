using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.Configuration;

namespace BitChopp;

using ViewModels;
using Views;

public partial class App : Application
{
    public IConfiguration? Configuration { get; private set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

        var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        Configuration = builder.Build();

        // Example of accessing the API key
        var apiKey = Configuration["ApiKey"];
        Console.WriteLine($"Your API Key: {apiKey}");
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (Configuration == null)
        {
            throw new InvalidOperationException("Configuration is not initialized");
        }

        var configService = new ConfigService(Configuration);
        var apiService = new ApiService(configService);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainViewModel(apiService)
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}