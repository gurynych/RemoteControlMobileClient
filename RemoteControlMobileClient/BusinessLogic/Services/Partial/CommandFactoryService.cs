using NetworkMessage.CommandFactory;
using RemoteControlMobileClient.MVVM.LifeCycles;

namespace RemoteControlMobileClient.BusinessLogic.Services.Partial
{
    internal partial class CommandFactoryService : ITransient
    {
        public partial ICommandFactory CreateCommandFactory();
    }
}
