using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RemoteControlMobileClient.BusinessLogic.DTO;
using RemoteControlMobileClient.BusinessLogic.Services;
using RemoteControlMobileClient.BusinessLogic.Services.Partial;
using RemoteControlMobileClient.MVVM.LifeCycles;
using RemoteControlMobileClient.Pages;
using System.Diagnostics;
using DeviceDTO = RemoteControlMobileClient.BusinessLogic.DTO.DeviceDTO;

namespace RemoteControlMobileClient.MVVM.ViewModels
{
	[QueryProperty(nameof(User), "User")]
	[QueryProperty(nameof(Device), "Device")]
	public partial class DeviceInfoViewModel : ObservableObject, ITransient
	{
		private readonly ServerAPIProvider apiProvider;

		private bool isLoad;
		public bool IsLoad
		{
			get => isLoad;
			set
			{
				SetProperty(ref isLoad, value);
				OnPropertyChanged();

				LoadDeviceStatusesCommand.NotifyCanExecuteChanged();
				DownloadScreenshotCommand.NotifyCanExecuteChanged();
				OpenDeviceFolderCommand.NotifyCanExecuteChanged();
				OpenRunningProgramsCommand.NotifyCanExecuteChanged();
				GoToBackCommand.NotifyCanExecuteChanged();
			}
		}
		[ObservableProperty] private UserDTO user;
		[ObservableProperty] private DeviceDTO device;
		[ObservableProperty] private DeviceStatusesDTO deviceStatuses;

		public DeviceInfoViewModel(ServerAPIProvider apiProvider)
		{
			this.apiProvider = apiProvider ?? throw new ArgumentNullException(nameof(apiProvider));
		}


		[RelayCommand(CanExecute = nameof(CanExecute))]
		private async Task LoadDeviceStatuses()
		{
			if (Device.DeviceType != "PC") return;
			IsLoad = true;

			CancellationTokenSource tokenSource = null;
			try
			{
				if (Device.IsConnected)
				{
					tokenSource = new CancellationTokenSource(50000);
					DeviceStatuses =
						await apiProvider.GetDeviceStatusesAsync(User, Device, tokenSource.Token);
				}
			}
			catch (Exception ex)
			{
				await Toast.Make(ex.Message, CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
			}
			finally
			{
				tokenSource?.Cancel();
				tokenSource?.Dispose();
			}

			IsLoad = false;
		}
		
		[RelayCommand(CanExecute = nameof(CanExecute))]
		private async Task DownloadScreenshot()
		{
			IsLoad = true;

			try
			{
				CancellationTokenSource tokenSource = new CancellationTokenSource(50000);
				
				string screenshotName = $"Screenshot_{Device?.DeviceName}_{DateTime.Now.ToString("dd_MM_yyyy")}.jpeg";
				FilesService filesService = new FilesService();
				string screenshotPath = filesService.CreateUniqueDownloadPath(screenshotName);;

				using (FileStream fs = File.Open(screenshotPath, FileMode.Create))
				{
					await apiProvider.GetScreenshot(User, Device, fs, tokenSource.Token);
				}

				await Toast.Make("Скриншот сохранен в папке загрузки", CommunityToolkit.Maui.Core.ToastDuration.Long).Show();
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
				await Toast.Make("Ошибка сохранения файла", CommunityToolkit.Maui.Core.ToastDuration.Long).Show();
			}

			IsLoad = false;
		}

		[RelayCommand(CanExecute = nameof(CanExecute))]
		private Task OpenDeviceFolder()
		{
			if (IsLoad) return Task.CompletedTask;
			return Shell.Current.GoToAsync(nameof(DeviceFolderPage), new Dictionary<string, object>()
			{
				{ "User", User },
				{ "Device", Device }
			});
		}

		[RelayCommand(CanExecute = nameof(CanExecute))]
		private Task OpenRunningPrograms()
		{
			return Shell.Current.GoToAsync(nameof(RunningProgramsPage), new Dictionary<string, object>()
			{
				{ "User", User },
				{ "Device", Device }
			});
		}

		[RelayCommand(CanExecute = nameof(CanExecute))]
		private Task GoToBack()
		{
			if (IsLoad) return Task.CompletedTask;
			return Shell.Current.GoToAsync("..");
		}

		private bool CanExecute()
		{
			return !IsLoad;
		}

	}
}
