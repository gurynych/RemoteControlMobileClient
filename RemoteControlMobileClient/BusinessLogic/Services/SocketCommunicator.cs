using NetworkMessage.Commands;
using NetworkMessage.CommandsResults;
using NetworkMessage.Communicator;
using NetworkMessage.Cryptography.AsymmetricCryptography;
using NetworkMessage.Cryptography.KeyStore;
using NetworkMessage.Cryptography.SymmetricCryptography;
using NetworkMessage.Intents;
using NetworkMessage;
using System.Net.Sockets;
using RemoteControlMobileClient.BusinessLogic.Services.Partial;
using NetworkMessage.CommandFactory;
using RemoteControlMobileClient.MVVM.LifeCycles;
using System.Diagnostics;

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

        public override async Task<bool> HandshakeAsync(IProgress<int> progress = null, CancellationToken token = default)
        {
            try
            {
                INetworkMessage message;
                BaseNetworkCommandResult result;
                byte[] publicKey = keyStore.GetPublicKey();
                result = new PublicKeyResult(publicKey);
                message = new NetworkMessage.NetworkMessage(result);
                await SendMessageAsync(message, progress, token).ConfigureAwait(false);

                GuidIntent guidIntent = await ReceiveNetworkObjectAsync<GuidIntent>(progress, token);
                if (guidIntent == null) throw new NullReferenceException(nameof(guidIntent));

                BaseNetworkCommand command = guidIntent.CreateCommand(factory);
                result = await command.ExecuteAsync();
                message = new NetworkMessage.NetworkMessage(result);
                await SendMessageAsync(message, progress, token);

                SuccessfulTransferResult transferResult =
                    await ReceiveNetworkObjectAsync<SuccessfulTransferResult>(token: token);
                return IsConnected = transferResult.IsSuccessful;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.Message);
                return IsConnected = false;
            }

            /*DownloadFileIntent fileIntent = await ReceiveNetworkObjectAsync<DownloadFileIntent>(progress, token).ConfigureAwait(false);
            if (fileIntent == null)
            {
                throw new NullReferenceException(nameof(fileIntent));
            }            
            command = fileIntent.CreateCommand(factory);
            result = await command.ExecuteAsync();
            message = new NetworkMessage.NetworkMessage(result);
            await SendMessageAsync(message, progress, token).ConfigureAwait(false);
            byte[] file = await File.ReadAllBytesAsync(fileIntent.Path, token);
            message = new NetworkMessage.NetworkMessage(file);
            await SendMessageAsync(message, progress, token).ConfigureAwait(false);*/
        }
    }
}
