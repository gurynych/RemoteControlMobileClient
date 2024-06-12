using CommunityToolkit.Maui.Storage;
using Java.IO;
using System.Diagnostics;

namespace RemoteControlMobileClient.BusinessLogic.Services.Partial
{
	internal partial class FilesService
	{
		public partial async Task<bool> SaveFileAsync(byte[] fileBytes, string fileName)
		{
			if (!Android.OS.Environment.IsExternalStorageManager) return false;

			Java.IO.File downloadsDir = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads);
			Java.IO.File file = new Java.IO.File(downloadsDir, fileName);
			try
			{
				FileOutputStream outputStream = new FileOutputStream(file);
				await outputStream.WriteAsync(fileBytes);
				outputStream.Close();
				return true;
			}
			catch (Java.IO.IOException e)
			{
				e.PrintStackTrace();
				return false;
			}
		}

		public partial async Task<bool> SaveFileAsync(string fileName, Stream stream, IProgress<double> progress = null, CancellationToken token = default)
		{
			if (string.IsNullOrWhiteSpace(fileName))
			{
				throw new ArgumentException($"\"{nameof(fileName)}\" не может быть пустым или содержать только пробел.", nameof(fileName));
			}
			ArgumentNullException.ThrowIfNull(stream);

			try
			{
				if (progress == null)
				{
					await FileSaver.Default.SaveAsync(fileName, stream, token);
				}
				else
				{
					await FileSaver.Default.SaveAsync(fileName, stream, progress, token);
				}
			}
			catch (Exception e)
			{
				Debug.WriteLine(e.Message);
				return false;
			}

			return true;
		}

		public partial string CreateUniqueDownloadPath(string fileName)
		{
			if (!Android.OS.Environment.IsExternalStorageManager) return string.Empty;
			string path = Path.Combine(Android.OS.Environment.DirectoryDownloads, fileName);

			if (string.IsNullOrWhiteSpace(fileName))
				throw new ArgumentException("Value cannot be null or whitespace.", nameof(fileName));


			Java.IO.File downloadsDir = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads);
			string fullPath = downloadsDir.AbsolutePath;

			int lastDotIndex = fileName.LastIndexOf('.');
			string extension = "";
			string fileNameWithoutExtension = fileName;
			if (lastDotIndex != -1)
			{
				extension = fileName[lastDotIndex..];
				fileNameWithoutExtension = fileName[..lastDotIndex];
			}

			string fileNameWithoutExtensionConst = fileNameWithoutExtension;

			int i = 1;
			while (System.IO.File.Exists(Path.Combine(fullPath, fileNameWithoutExtension + extension)))
			{
				fileNameWithoutExtension = fileNameWithoutExtensionConst + $"({i++})";
			}

			return Path.Combine(fullPath, fileNameWithoutExtension + extension);
		}
	}
}
