using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetworkMessage.CommandFactory;
using NetworkMessage.Communicator;
using NetworkMessage.DTO;
using RemoteControlMobileClient.BusinessLogic.DTO;
using RemoteControlMobileClient.BusinessLogic.Services;
using RemoteControlMobileClient.BusinessLogic.Services.Partial;
using RemoteControlMobileClient.MVVM.LifeCycles;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using DeviceDTO = RemoteControlMobileClient.BusinessLogic.DTO.DeviceDTO;

namespace RemoteControlMobileClient.MVVM.ViewModels
{
	[QueryProperty(nameof(User), "User")]
	[QueryProperty(nameof(Device), "Device")]
	public partial class DeviceFolderViewModel : ObservableObject, ITransient
	{
		private string path = "root";
		private IProgress<long> progress;
		private readonly ServerAPIProvider apiProvider;

		[ObservableProperty] private UserDTO user;
		[ObservableProperty] private DeviceDTO device;
		[ObservableProperty] private bool isBusy;

		[ObservableProperty] private bool isDownload;
		[ObservableProperty] private string downloadableFileName;
		[ObservableProperty] private long downloadableBytesAmount;
		[ObservableProperty] private long totalBytesAmount;
		[ObservableProperty] private int percentDownloaded;

		public ObservableCollection<FileInfoDTO> FileInfoList { get; set; }

		public DeviceFolderViewModel(ServerAPIProvider apiProvider)
		{
			this.apiProvider = apiProvider;
			FileInfoList = new ObservableCollection<FileInfoDTO>();
		}

		[RelayCommand]
		private Task GoToConnectedDevicesPage()
		{
			return Shell.Current.GoToAsync("..");
		}

		[RelayCommand]
		private async Task LoadNestedFilesInRootDirectory()
		{
			IsBusy = true;

			this.path = "root";
			FileInfoList.Clear();
			CancellationTokenSource tokenSource = new CancellationTokenSource(50000);
			await foreach (FileInfoDTO file in apiProvider.GetNestedFilesInfoInDirectoryAsync(User, Device, path, tokenSource.Token))
			{
				FileInfoList.Add(file);
			}

			IsBusy = false;
		}

		[RelayCommand]
		private async Task LoadNestedFilesInDirectory(FileInfoDTO fileInfo)
		{
			if (fileInfo.FileType == Enum.GetName(FileType.File)) return;

			this.path += "/" + fileInfo.Name;
			IsBusy = true;
			FileInfoList.Clear();
			CancellationTokenSource tokenSource = new CancellationTokenSource(50000);
			await foreach (FileInfoDTO file in apiProvider.GetNestedFilesInfoInDirectoryAsync(User, Device, path, tokenSource.Token))
			{
				FileInfoList.Add(file);
			}

			IsBusy = false;
		}


		[RelayCommand(IncludeCancelCommand = true)]
		private Task DownloadSelectedItem(FileInfoDTO fileInfo, CancellationToken token)
		{
			return fileInfo.FileType switch
			{
				nameof(FileType.File) => DownloadFileAsync(fileInfo, token),
				nameof(FileType.Directory) => DownloadDirectoryAsync(fileInfo, token),
				_ => Task.CompletedTask
			};
		}

		private async Task DownloadFileAsync(FileInfoDTO fileInfo, CancellationToken token)
		{			
			if (fileInfo.FileType != Enum.GetName(FileType.File)) return;

			IsDownload = true;
			string pathForDownload = this.path + "/" + fileInfo.Name;

			FilesService filesService = new FilesService();
			string downloadPath = filesService.CreateUniqueDownloadPath(fileInfo.Name);
			try
			{
				DownloadableFileName = fileInfo.Name;
				TotalBytesAmount = fileInfo.FileLength!.Value;
				progress = new Progress<long>(value =>
				{
					DownloadableBytesAmount = value;
					PercentDownloaded = (int)(value * 1.0 / fileInfo.FileLength * 100);
				});

				using (FileStream fs = File.OpenWrite(downloadPath))
				{
					await apiProvider.DownloadFileAsync(User, Device, fileInfo.FullName, fs, progress, token);
				}

				await Toast.Make("Файл сохранен в папке загрузок", CommunityToolkit.Maui.Core.ToastDuration.Long).Show();
			}	
			catch (OperationCanceledException)
			{		
				await Toast.Make("Загрузка файла отменена", CommunityToolkit.Maui.Core.ToastDuration.Long).Show();
			}
			catch (Exception ex)
			{
				if (File.Exists(downloadPath))
				{
					File.Delete(downloadPath);
				}

				Debug.WriteLine(ex.Message);
				await Toast.Make("Ошибка сохранения файла", CommunityToolkit.Maui.Core.ToastDuration.Long).Show();
			}

			/*Stream stream = await apiProvider.DownloadFileAsync(User, Device, pathForDownload, tokenSource.Token);
			FilesService fileService = new FilesService();
			bool isSuccess = await fileService.SaveFileAsync(fileInfo.Name, stream, token: tokenSource.Token);
			if (isSuccess)
			{
				await Toast.Make("Файл сохранен в папке загрузки", CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
			}
			else
			{
				await Toast.Make("Ошибка сохранения файла", CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
			}*/

			/*if (fileBytes != null)
			{
				FilesService fileService = new FilesService();
				bool isSuccess = await fileService.SaveFileAsync(fileBytes, fileInfo.Name);
				if (isSuccess)
				{
					await Toast.Make("Файл сохранен в папке загрузки", CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
				}
				else
				{
					await Toast.Make("Ошибка сохранения файла", CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
				}
			}*/

			IsDownload = false;
		}

		private async Task DownloadDirectoryAsync(FileInfoDTO fileInfo, CancellationToken token)
		{
			if (fileInfo.FileType != Enum.GetName(FileType.Directory)) return;

			IsBusy = true;
			string pathForDownload = this.path + "/" + fileInfo.Name;

			FilesService filesService = new FilesService();
			string downloadPath = filesService.CreateUniqueDownloadPath(fileInfo.Name + ".rar");
			try
			{
				using (FileStream fs = File.OpenWrite(downloadPath))
				{
					await apiProvider.DownloadDirectoryAsync(User, Device, fileInfo.FullName, fs, token: token);
				}

				await Toast.Make("Файл сохранен в папке загрузок", CommunityToolkit.Maui.Core.ToastDuration.Long).Show();
			}
			catch (OperationCanceledException)
			{
				await Toast.Make("Загрузка файла отменена", CommunityToolkit.Maui.Core.ToastDuration.Long).Show();
			}
			catch (Exception ex)
			{
				if (File.Exists(downloadPath))
				{
					File.Delete(downloadPath);
				}

				Debug.WriteLine(ex.Message);
				await Toast.Make("Ошибка сохранения файла", CommunityToolkit.Maui.Core.ToastDuration.Long).Show();
			}

			IsBusy = false;
		}
	}
}
