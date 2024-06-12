using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using NetworkMessage.DTO;
using RemoteControlMobileClient.BusinessLogic.DTO;
using RemoteControlMobileClient.BusinessLogic.Services;
using RemoteControlMobileClient.MVVM.LifeCycles;
using System.Collections.ObjectModel;

namespace RemoteControlMobileClient.MVVM.ViewModels
{
	[QueryProperty(nameof(User), "User")]
	[QueryProperty(nameof(Device), "Device")]
	public partial class RunningProgramsViewModel : ObservableObject, ITransient
	{
		private readonly ServerAPIProvider apiProvider;

		[NotifyCanExecuteChangedFor(nameof(LoadRunningProgramsCommand))]
		[ObservableProperty] private bool isLoad;
		[ObservableProperty] private UserDTO user;
		[ObservableProperty] private DeviceDTO device;

		public ObservableCollection<ProgramInfoDTO> RunningPrograms { get; set; }

		public RunningProgramsViewModel(ServerAPIProvider apiProvider)
        {
			this.apiProvider = apiProvider ?? throw new ArgumentNullException(nameof(apiProvider));
			RunningPrograms = new ObservableCollection<ProgramInfoDTO>();
		}

		[RelayCommand]
		private async Task LoadRunningPrograms()
		{
			IsLoad = true;

			CancellationTokenSource tokenSource = new CancellationTokenSource(50000);
			try
			{
				RunningPrograms.Clear();
				List<ProgramInfoDTO> runningPrograms = (await apiProvider.GetRunninProgramsAsync(User, Device, tokenSource.Token)).ToList();
				runningPrograms.ForEach(RunningPrograms.Add);
			}
			catch (Exception ex)
			{
				IsLoad = false;
				await Toast.Make(ex.Message, CommunityToolkit.Maui.Core.ToastDuration.Long).Show();
			}
			finally
			{
				tokenSource.Cancel();
				tokenSource.Dispose();
			}

			IsLoad = false;
		}

		[RelayCommand]
		private Task GoToBack()
		{			
			return Shell.Current.GoToAsync("..");
		}
	}
}
