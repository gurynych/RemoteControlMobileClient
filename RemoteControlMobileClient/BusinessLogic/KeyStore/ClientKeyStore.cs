using NetworkMessage.Cryptography.AsymmetricCryptography;
using NetworkMessage.Cryptography.KeyStore;

namespace RemoteControlMobileClient.BusinessLogic.KeyStore
{
    public class ClientKeyStore : AsymmetricKeyStoreBase
    {
        private readonly byte[] privateKey;

        public ClientKeyStore(IAsymmetricCryptographer cryptographer) : base(cryptographer)
        {
            privateKey = cryptographer.GeneratePrivateKey();
        }

        protected override byte[] SetPrivateKey()
        {
            return privateKey;
        }
    }
}
