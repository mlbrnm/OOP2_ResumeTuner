using Microsoft.Extensions.Logging;


namespace FinalProjectResumeTuner
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();
            builder.Services.AddSingleton<DatabaseBroker>();
            builder.Services.AddSingleton<AIProcessor>();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            var app = builder.Build();

            // Load data when the application starts
            var db = app.Services.GetRequiredService<DatabaseBroker>();
            db.Initialize();
            db.Reset();

            var ai = app.Services.GetRequiredService<AIProcessor>();

            return app;
        }
    }
}
