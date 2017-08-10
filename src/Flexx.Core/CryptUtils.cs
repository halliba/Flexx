using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;

namespace Flexx.Core
{
    internal static class CryptUtils
    {
        public static byte[] RsaEncryptWithPublic(byte[] data, AsymmetricKeyParameter publicKey)
        {
            var encryptEngine = new Pkcs1Encoding(new RsaEngine());
            
            encryptEngine.Init(true, publicKey);

            var encrypted = encryptEngine.ProcessBlock(data, 0, data.Length);
            return encrypted;
        }

        public static byte[] RsaEncryptWithPrivate(byte[] data, AsymmetricKeyParameter privateKey)
        {
            var encryptEngine = new Pkcs1Encoding(new RsaEngine());
            
            encryptEngine.Init(true, privateKey);

            var encrypted = encryptEngine.ProcessBlock(data, 0, data.Length);
            return encrypted;
        }

        public static byte[] RsaDecryptWithPrivate(byte[] data, AsymmetricKeyParameter privateKey)
        {
            var decryptEngine = new Pkcs1Encoding(new RsaEngine());

            decryptEngine.Init(false, privateKey);

            var decrypted = decryptEngine.ProcessBlock(data, 0, data.Length);
            return decrypted;
        }

        public static byte[] RsaDecryptWithPublic(byte[] data, AsymmetricKeyParameter publicKey)
        {
            var decryptEngine = new Pkcs1Encoding(new RsaEngine());

            decryptEngine.Init(false, publicKey);

            var decrypted = decryptEngine.ProcessBlock(data, 0, data.Length);
            return decrypted;
        }
    }
}