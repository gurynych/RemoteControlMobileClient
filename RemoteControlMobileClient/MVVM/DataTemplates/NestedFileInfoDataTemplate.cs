using NetworkMessage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteControlMobileClient.MVVM.DataTemplates
{
    class NestedFileInfoDataTemplate : DataTemplateSelector
    {
        public DataTemplate MyFileInfoTemplate { get; set; }
        public DataTemplate MyDirectoryInfoTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (item is MyFileInfo)
            {
                return MyFileInfoTemplate;
            }
            else if (item is MyDirectoryInfo)
            {
                return MyDirectoryInfoTemplate;
            }

            return null;
        }
    }
}
