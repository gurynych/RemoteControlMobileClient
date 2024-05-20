using NetworkMessage.CommandFactory;
using NetworkMessage.CommandsResults;
using NetworkMessage.CommandsResults.ConcreteCommandResults;
using Newtonsoft.Json;
using RemoteControlMobileClient.BusinessLogic.Models;
using RemoteControlMobileClient.BusinessLogic.Services.Partial;
using RemoteControlMobileClient.MVVM.LifeCycles;
using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using Device = RemoteControlMobileClient.BusinessLogic.Models.Device;

namespace RemoteControlMobileClient.BusinessLogic.Services
{
    internal class ServerAPIProviderService : ITransient
    {
		public const string ServerAddress = "192.168.1.162";
		public const string ServerPort = "5000";
		private const string AuthtorizeAPIUri = $"http://{ServerAddress}:{ServerPort}/api/AuthentificationAPI/AuthorizeFromDevice";
		private const string RegisterAPIUri = $"http://{ServerAddress}:{ServerPort}/api/AuthentificationAPI/RegisterFromDevice";
		private const string AuthorizeWithTokenUri = $"http://{ServerAddress}:{ServerPort}/api/AuthentificationAPI/AuthorizeWithToken";
		private const string GetNestedFilesInfoInDirectoryUri = $"http://{ServerAddress}:{ServerPort}/api/DeviceAPI/GetNestedFilesInfoInDirectory";
		private const string GetConnectedDeviceUri = $"http://{ServerAddress}:{ServerPort}/api/DeviceAPI/GetConnectedDevices";
		private const string GetUserByTokenUri = $"http://{ServerAddress}:{ServerPort}/api/AuthentificationAPI/GetUserByToken";
		private const string DownloadFileUri = $"http://{ServerAddress}:{ServerPort}/api/DeviceAPI/DownloadFile";
		private readonly ICommandFactory factory;

        public ServerAPIProviderService(CommandFactoryService commandFactoryService)
        {
            factory = commandFactoryService.CreateCommandFactory();
        }

        /// <summary>
        /// Асинхронный метод для авторизации пользователя через API сервера
        /// </summary>
        /// <param name="user"></param>
        /// <returns>Открытый ключ сервера</returns>
        /// <exception cref="WebException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="FormatException"/>
        /// <exception cref="SEHException"/>
        public async Task<byte[]> UserAuthorizationUseAPIAsync(User user, CancellationToken token = default)
        {
            using HttpClient client = new HttpClient();
            try
            {
                FormUrlEncodedContent parameters = await GetAuthParametersAsync(user, token);
                HttpResponseMessage response = await client.PostAsync(AuthtorizeAPIUri, parameters, token);
                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync(token);
                    return Convert.FromBase64String(responseContent[1..^1]);
                }

                Debug.WriteLine("Ошибка при выполнении запроса: " + response.StatusCode);
                return default;
            }
            catch (Exception ex)
            {
                return default;
            }
        }

        /// <summary>
        /// Асинхронный метод для регистрации пользователя через API сервера
        /// </summary>
        /// <param name="user"></param>
        /// <returns>Открытый ключ сервера</returns>
        /// <exception cref="WebException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="FormatException"/>      
        /// <exception cref="SEHException"/>
        public async Task<byte[]> UserRegistrationUseAPIAsync(User user, CancellationToken token = default)
        {
            using HttpClient client = new HttpClient();
            try
            {
                FormUrlEncodedContent parameters = await GetRegParametersAsync(user, token);
                HttpResponseMessage response = await client.PostAsync(RegisterAPIUri, parameters, token);
                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync(token);
                    return Convert.FromBase64String(responseContent[1..^1]);
                }

                Debug.WriteLine("Ошибка при выполнении запроса: " + response.StatusCode);
                return default;
            }
            catch (Exception ex)
            {
                return default;
            }
        }

        /// <summary>
        /// Обращается к API сервера для получения подключенных устройств к аккаунту пользователя
        /// </summary>
        /// <param name="user">Пользователь, для которого необходимо получтиь список устройств</param>        
        /// <returns>В случае успехасписок подключенных устройств пользователя, иначе - null</returns>
        public async Task<List<Device>> GetConnectedDeviceAsync(User user, CancellationToken token = default)
        {
            using HttpClient client = new HttpClient();
            try
            {
                string encodedToken = WebUtility.UrlEncode(Convert.ToBase64String(user.AuthToken));
                string requestUri = GetConnectedDeviceUri +
                    $"?userToken={encodedToken}";

                HttpResponseMessage response = await client.GetAsync(requestUri, token);

                //ByteArrayContent bytesContent = new ByteArrayContent(userToken);
                //HttpResponseMessage response = await client.PostAsync(GetConnectedDeviceUri, bytesContent, token);
                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync(token);
                    return JsonConvert.DeserializeObject<List<Device>>(responseContent);
                }

                Debug.WriteLine("Ошибка при выполнении запроса: " + response.StatusCode);
                return default;
            }
            catch (Exception ex)
            {
                return default;
            }
        }

        /// <summary>
        /// Обращается к API сервера для получения вложенных файлов в директорию по указанному пути
        /// </summary>
        /// <param name="user">Пользователь, к которому подключено выбранное устройство</param>
        /// <param name="deviceId">Id устройства, у которого запрашиваются данные</param>
        /// <param name="path">Путь до запрашиваемого расположения</param>        
        /// <returns>В случае успеха возвращается структура со списками файлов и директорий</returns>
        public async Task<NestedFilesInDirectory> GetNestedFilesInfoInDirectoryAsync(User user, int deviceId, string path, CancellationToken token = default)
        {
            using HttpClient client = new HttpClient();
            try
            {
                string encodedToken = WebUtility.UrlEncode(Convert.ToBase64String(user.AuthToken));
                string requestUri = GetNestedFilesInfoInDirectoryUri +
                    $"?userToken={encodedToken}&" +
                    $"deviceId={deviceId}&" +
                    $"&path={path}";

                HttpResponseMessage response = await client.GetAsync(requestUri, token);
                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync(token);
                    return JsonConvert.DeserializeObject<NestedFilesInDirectory>(responseContent);
                }

                Debug.WriteLine("Ошибка при выполнении запроса: " + response.StatusCode);
                return default;
            }
            catch (Exception ex)
            {
                return default;
            }
        }

        /// <summary>
        /// Обращается к API сервера для загрузки файла по запрашиваему пути
        /// </summary>
        /// <param name="user">Пользователь, к которому подключено выбранное устройство</param>
        /// <param name="deviceId">Id устройства, у которого запрашиваются данные</param>
        /// <param name="path">Путь до файла, который необходимо скачать</param>        
        /// <returns></returns>
        public async Task<byte[]> DownloadFileAsync(User user, int deviceId, string path, CancellationToken token = default)
        {
            using HttpClient client = new HttpClient();
            try
            {
                string encodedToken = WebUtility.UrlEncode(Convert.ToBase64String(user.AuthToken));
                string requestUri = DownloadFileUri +
                    $"?userToken={encodedToken}&" +
                    $"deviceId={deviceId}&" +
                    $"&path={path}";

                HttpResponseMessage response = await client.GetAsync(requestUri, token);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsByteArrayAsync(token);
                }

                Debug.WriteLine("Ошибка при выполнении запроса: " + response.StatusCode);
                return default;
            }
            catch (Exception ex)
            {
                return default;
            }
        }

        /// <summary>
        /// Асинхронный метод для получения параметров пользователя для авторизации
        /// </summary>        
        /// <returns>FormUrlEncodedContent с параметрами пользователя</returns>
        /// <exception cref="SEHException"/>
        private async Task<FormUrlEncodedContent> GetAuthParametersAsync(User user, CancellationToken token = default)
        {
            DeviceGuidResult guidResult = await factory.CreateGuidCommand().ExecuteAsync(token).ConfigureAwait(false) as DeviceGuidResult;
            var parameters = new Dictionary<string, string>
            {
                { "email", user.Email },
                { "password", user.Password },
                { "deviceGuid", guidResult.Guid },
                { "deviceName", DeviceInfo.Name },
                { "deviceType", DeviceInfo.Idiom.ToString() },
                { "devicePlatform", DeviceInfo.Platform.ToString() },
                { "devicePlatformVersion", DeviceInfo.VersionString },
                { "deviceManufacturer", DeviceInfo.Manufacturer }
            };

            return new FormUrlEncodedContent(parameters);
        }

        /// <summary>
        /// Асинхронный метод для получения параметров пользователя для регистрации
        /// </summary>
        /// <param name="user"></param>
        /// <returns>NameValuecollection с параметрами нового пользователя</returns>
        /// <exception cref="SEHException"/>
        private async Task<FormUrlEncodedContent> GetRegParametersAsync(User user, CancellationToken token = default)
        {
            DeviceGuidResult guidResult = await factory.CreateGuidCommand().ExecuteAsync(token).ConfigureAwait(false) as DeviceGuidResult;
            var parameters = new Dictionary<string, string>
            {
                { "login",user.Login },
                { "email", user.Email },
                { "password", user.Password },
                { "deviceGuid", guidResult.Guid },
                { "deviceName", DeviceInfo.Name },
                { "deviceType", DeviceInfo.DeviceType.ToString() },
                { "devicePlatform", DeviceInfo.Platform.ToString() },
                { "devicePlatformVersion", DeviceInfo.VersionString },
                { "deviceManufacturer", DeviceInfo.Manufacturer }
            };

            return new FormUrlEncodedContent(parameters);
        }
    }
}