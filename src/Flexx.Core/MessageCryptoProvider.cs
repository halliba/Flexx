using System.Text;
using Flexx.Core.Api;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Security;

namespace Flexx.Core
{
    internal class MessageCryptoProvider
    {
        public AsymmetricCipherKeyPair KeyPair { get; set; }

        public MessageCryptoProvider(AsymmetricCipherKeyPair keyPair)
        {
            KeyPair = keyPair;
        }

        public void SignTransport(Transport transport)
        {
            var signature = SignUtils.Sign(transport.Data, KeyPair.Private);
            transport.Signature = signature;
            transport.PublicKey = PemUtils.GetPemFromKey(KeyPair.Public);
        }

        public bool ValidateTransport<TData>(Transport transport, out TData data) where TData : BaseModel
        {
            data = null;
            var publicKey = PemUtils.GetKeyFromPem(transport.PublicKey);

            var verified = SignUtils.Verify(transport.Data, transport.Signature, publicKey);
            if (!verified) return false;

            var text = Encoding.Unicode.GetString(transport.Data);
            data = JsonConvert.DeserializeObject<TData>(text);
            return data.Sender.PublicKey == transport.PublicKey;
        }

        public void EncryptTransport(Transport transport, AsymmetricKeyParameter publicKey)
        {
            var data = CryptUtils.RsaEncryptWithPublic(transport.Data, publicKey);
            transport.Data = data;
        }

        public bool DecryptTransport(Transport transport)
        {
            var data = CryptUtils.RsaDecryptWithPrivate(transport.Data, KeyPair.Private);
            transport.Data = data;
            return true;
        }
        
        public static MessageCryptoProvider Generate()
        {
            var generator = new RsaKeyPairGenerator();
            generator.Init(new KeyGenerationParameters(new SecureRandom(), 1024));
            var keyPair = generator.GenerateKeyPair();
            return new MessageCryptoProvider(keyPair);
        }
    }
}