﻿using NetworkMessage.CommandFactory;

namespace RemoteControlMobileClient.BusinessLogic.Services.Partial
{
    public partial class CommandFactoryService
    {
        public partial ICommandFactory CreateCommandFactory() => new NetworkMessage.Mobile.Platforms.Android.AndroidCommandFactory();
    }
}
