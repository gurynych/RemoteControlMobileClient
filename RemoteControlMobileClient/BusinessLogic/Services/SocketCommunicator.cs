using NetworkMessage.Commands;
using NetworkMessage.CommandsResults;
using NetworkMessage.Communicator;
using NetworkMessage.Cryptography.AsymmetricCryptography;
using NetworkMessage.Cryptography.KeyStore;
using NetworkMessage.Cryptography.SymmetricCryptography;
using NetworkMessage.CommandsResults.ConcreteCommandResults;
using NetworkMessage.Intents;
using NetworkMessage;
using System.Net.Sockets;
using RemoteControlMobileClient.BusinessLogic.Services.Partial;
using NetworkMessage.CommandFactory;
using RemoteControlMobileClient.MVVM.LifeCycles;
using System.Diagnostics;
using NetworkMessage.Intents.ConcreteIntents;

namespace RemoteControlMobileClient.BusinessLogic.Services
{
    internal class SocketCommunicator : TcpCryptoClientCommunicator, ISingleton
    {
        public const string ServerIP = ServerAPIProviderService.ServerAddress;
        public const int ServerPort = 11000;
        private ICommandFactory factory;

        public SocketCommunicator(
            IAsymmetricCryptographer asymmetricCryptographer,
            ISymmetricCryptographer symmetricCryptographer,
            AsymmetricKeyStoreBase keyStore, CommandFactoryService commandFactoryService)
            : base(new TcpClient(), asymmetricCryptographer, symmetricCryptographer, keyStore)
        {
            factory = commandFactoryService.CreateCommandFactory();
        }

		public override async Task<bool> HandshakeAsync(IProgress<long> progress = null, CancellationToken token = default)
		{
			try
			{
				BaseNetworkCommandResult result;
				byte[] publicKey = keyStore.GetPublicKey();
				result = new PublicKeyResult(publicKey);
				await SendObjectAsync(result, progress, token).ConfigureAwait(false);

				GuidIntent guidIntent = await ReceiveAsync<GuidIntent>(progress, token);
				if (guidIntent == null) throw new NullReferenceException(nameof(guidIntent));

				INetworkCommand command = guidIntent.CreateCommand(factory);
				result = await command.ExecuteAsync();
				await SendObjectAsync(result, progress, token);

				SuccessfulTransferResult transferResult =
					await ReceiveAsync<SuccessfulTransferResult>(token: token);
				return IsConnected = true;
			}
			catch (Exception ex)
			{
				Debug.Write(ex.Message);
				return IsConnected = false;
			}


		}
	}
}
