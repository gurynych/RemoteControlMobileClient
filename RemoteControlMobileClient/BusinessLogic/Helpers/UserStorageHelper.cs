using Newtonsoft.Json;
using RemoteControlMobileClient.BusinessLogic.DTO;

namespace RemoteControlMobileClient.BusinessLogic.Helpers
{
	public class UserStorageHelper
	{
		public static async Task WriteUserAsync(UserDTO user)
		{
			ArgumentNullException.ThrowIfNull(user);
			if (user.AuthToken == null || user.AuthToken.Length == 0)
			{
				throw new ArgumentNullException(nameof(user.AuthToken));
			}

			await SecureStorage.Default.SetAsync(nameof(user), JsonConvert.SerializeObject(user));
		}

		public static async Task<UserDTO> ReadUserAsync()
		{
			string userJson = await SecureStorage.Default.GetAsync("user");
			if (userJson == null)
			{
				return null;
			}

			return JsonConvert.DeserializeObject<UserDTO>(userJson);
		}
	}
}
