using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Security;

namespace Flexx.Core
{
    public class PersonalIdentity : UserIdentity
    {
        public AsymmetricCipherKeyPair KeyPair { get; }

        public PersonalIdentity(string username, AsymmetricCipherKeyPair keyPair)
            : base(username, PemUtils.GetPemFromKey(keyPair.Public))
        {
            KeyPair = keyPair;
        }

        public static PersonalIdentity Generate(string userName)
        {
            var generator = new RsaKeyPairGenerator();
            generator.Init(new KeyGenerationParameters(new SecureRandom(), 1024));
            var keyPair = generator.GenerateKeyPair();
            return new PersonalIdentity(userName, keyPair);
        }
    }
}