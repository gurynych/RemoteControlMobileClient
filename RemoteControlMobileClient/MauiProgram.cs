using DevExpress.Maui;
using RemoteControlMobileClient.MVVM.LifeCycles;
using RemoteControlMobileClient.MVVM.ViewModels;
using CommunityToolkit.Maui;
using NetworkMessage.Cryptography.AsymmetricCryptography;
using NetworkMessage.Cryptography.SymmetricCryptography;
using NetworkMessage.Cryptography.KeyStore;
using RemoteControlMobileClient.BusinessLogic.KeyStore;
using NetworkMessage.Communicator;
using RemoteControlMobileClient.BusinessLogic.Services;

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

            IEnumerable<Type> assemblyTypes = typeof(AuthorizationViewModel).Assembly.GetTypes().Where(x => x.IsClass);
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

            builder.Services.AddSingleton<IAsymmetricCryptographer, RSACryptographer>();
            builder.Services.AddSingleton<ISymmetricCryptographer, AESCryptographer>();
            builder.Services.AddSingleton<AsymmetricKeyStoreBase, ClientKeyStore>();
            builder.Services.AddSingleton<TcpCryptoClientCommunicator, SocketCommunicator>();

            app = builder.Build();
            foreach (Type singltoneType in builder.Services.Where(x => x.GetType().GetInterfaces().Contains(typeof(ISingleton))).Select(x => x.ServiceType))
            {
                _ = app.Services.GetRequiredService(singltoneType);
            }

            _ = app.Services.GetRequiredService<IAsymmetricCryptographer>();
            _ = app.Services.GetRequiredService<ISymmetricCryptographer>();
            _ = app.Services.GetRequiredService<AsymmetricKeyStoreBase>();
            _ = app.Services.GetRequiredService<SocketCommunicator>();

            return app;
        }

        public static T GetRequiredService<T>()
        {
            return app.Services.GetRequiredService<T>();
        }
    }
}
