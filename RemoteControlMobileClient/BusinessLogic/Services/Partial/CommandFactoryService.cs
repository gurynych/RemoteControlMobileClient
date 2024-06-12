using NetworkMessage.CommandFactory;
using RemoteControlMobileClient.MVVM.LifeCycles;

namespace RemoteControlMobileClient.BusinessLogic.Services.Partial
{
    public partial class CommandFactoryService : ITransient
    {
        public partial ICommandFactory CreateCommandFactory();
    }
}
