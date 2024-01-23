using CommunityToolkit.Mvvm.ComponentModel;
using NetworkMessage.Models;
using System.Collections.ObjectModel;

namespace RemoteControlMobileClient.BusinessLogic.Models
{
    internal partial class NestedFilesInDirectory : ObservableObject
    {
        [ObservableProperty]      
        private ObservableCollection<MyFileInfo> nestedFilesInfo;

        [ObservableProperty]
        private ObservableCollection<MyDirectoryInfo> nestedDirectoriesInfo;               
    }
}
