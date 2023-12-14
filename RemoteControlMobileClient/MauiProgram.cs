using DevExpress.Maui;
using RemoteControlMobileClient.MVVM.LifeCycles;
using RemoteControlMobileClient.MVVM.ViewModels;
using CommunityToolkit.Maui;

namespace RemoteControlMobileClient
{
    public static class MauiProgram
    {
        private static MauiApp app;

        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .UseDevExpress(useLocalization: true)
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("univia-pro-regular.ttf", "Univia-Pro");
                    fonts.AddFont("roboto-bold.ttf", "Roboto-Bold");
                    fonts.AddFont("roboto-regular.ttf", "Roboto");
                });

            IEnumerable<Type> assemblyTypes = typeof(AuthentificationViewModel).Assembly.GetTypes().Where(x => x.IsClass);
            foreach (Type type in assemblyTypes)
            {
                Type[] interfaces = type.GetInterfaces();                
                if (interfaces.Contains(typeof(ISingleton)))
                {
                    builder.Services.AddSingleton(type);
                }
                else if (interfaces.Contains(typeof(ITransient)))
                {
                    builder.Services.AddTransient(type);
                }
            }

            app = builder.Build();
            foreach (Type singltoneType in builder.Services.Where(x => x.GetType().GetInterfaces().Contains(typeof(ISingleton))).Select(x => x.ServiceType))
            {
                _ = app.Services.GetRequiredService(singltoneType);
            }

            return app;
        }

        public static T GetRequiredService<T>()
        {
            return app.Services.GetRequiredService<T>();
        }
    }
}
