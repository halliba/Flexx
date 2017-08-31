using System.IO;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.OpenSsl;

namespace Flexx.Core
{
    public static class PemUtils
    {
        public static AsymmetricKeyParameter GetKeyFromPem(string pemContent)
        {
            var reader = new StringReader(pemContent);
            var pemReader = new PemReader(reader);
            var keyParameter = (AsymmetricKeyParameter)pemReader.ReadObject();
            return keyParameter;
        }

        public static string GetPemFromKey(AsymmetricKeyParameter publicKey)
        {
            var writer = new StringWriter();
            var pemWriter = new PemWriter(writer);
            pemWriter.WriteObject(publicKey);
            return writer.ToString();
        }

        public static AsymmetricCipherKeyPair GetKeyPairFromPem(string pemContent)
        {
            var reader = new StringReader(pemContent);
            var pemReader = new PemReader(reader);
            var keyParameter = (AsymmetricCipherKeyPair)pemReader.ReadObject();
            return keyParameter;
        }

        public static string GetPemFromKeyPair(AsymmetricCipherKeyPair publicKey)
        {
            var writer = new StringWriter();
            var pemWriter = new PemWriter(writer);
            pemWriter.WriteObject(publicKey);
            return writer.ToString();
        }
    }
}