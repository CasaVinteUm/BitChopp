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

    public ConfigService? ConfigService { get; private set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

        var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        Configuration = builder.Build();
        ConfigService = new ConfigService(Configuration);

        Console.WriteLine($"LnBitsHost: {ConfigService?.GetLnBitsHost()}");
        Console.WriteLine($"Your API Key: {ConfigService?.GetApiKey()}");
        Console.WriteLine($"Switch ID: {ConfigService?.GetSwitchId()}");
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (Configuration == null || ConfigService == null)
        {
            throw new InvalidOperationException("Configuration is not initialized");
        }

        var apiService = new ApiService(ConfigService);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainViewModel(apiService, ConfigService)
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
