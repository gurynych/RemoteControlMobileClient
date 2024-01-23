using NetworkMessage.CommandFactory;
using NetworkMessage.Mobile.Platforms.Android;

namespace RemoteControlMobileClient.BusinessLogic.Services.Partial
{
    internal partial class CommandFactoryService
    {
        public partial ICommandFactory CreateCommandFactory() => new AndroidCommandFactory();
    }
}
