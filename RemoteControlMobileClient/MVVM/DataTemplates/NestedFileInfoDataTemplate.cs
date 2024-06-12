using NetworkMessage.DTO;

namespace RemoteControlMobileClient.MVVM.DataTemplates
{
	class NestedFileInfoDataTemplate : DataTemplateSelector
    {
        public DataTemplate MyFileInfoTemplate { get; set; }
        public DataTemplate MyDirectoryInfoTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (item is FileInfoDTO fileInfo)
            {
                if (fileInfo.FileType == Enum.GetName(FileType.File))
                {
                    return MyFileInfoTemplate;
                }
                else if (fileInfo.FileType == Enum.GetName(FileType.Directory) || fileInfo.FileType == Enum.GetName(FileType.Drive))
                {
                    return MyDirectoryInfoTemplate;
                }
            }

            return null;
        }
    }
}
