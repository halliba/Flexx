using System;
using System.IO;
using System.Security.Cryptography;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;

namespace Flexx.Core.Utils
{
    internal static class CryptUtils
    {
        public static byte[] GenrateAesKey()
        {
            using (var aes = new AesManaged())
            {
                aes.KeySize = 128;
                aes.GenerateKey();
                return aes.Key;
            }
        }

        public static byte[] RsaEncryptWithPublic(byte[] data, AsymmetricKeyParameter publicKey)
        {
            var encryptEngine = new Pkcs1Encoding(new RsaEngine());
            
            encryptEngine.Init(true, publicKey);

            var encrypted = encryptEngine.ProcessBlock(data, 0, data.Length);
            return encrypted;
        }

        public static byte[] RsaDecryptWithPrivate(byte[] data, int offset, int length, AsymmetricKeyParameter privateKey)
        {
            var decryptEngine = new Pkcs1Encoding(new RsaEngine());

            decryptEngine.Init(false, privateKey);

            var decrypted = decryptEngine.ProcessBlock(data, offset, length);
            return decrypted;
        }

        /// <summary>
        /// Encrypt a byte array using AES
        /// </summary>
        /// <param name="key">128 bit key</param>
        /// <param name="secret">byte array that need to be encrypted</param>
        /// <returns>Encrypted array</returns>
        public static byte[] AesEncryptByteArray(byte[] secret, byte[] key)
        {
            using (var ms = new MemoryStream())
            {
                using (var cryptor = new AesManaged())
                {
                    cryptor.Mode = CipherMode.CBC;
                    cryptor.Padding = PaddingMode.PKCS7;
                    cryptor.KeySize = key.Length * 8;
                    cryptor.BlockSize = 128;
                    
                    var iv = cryptor.IV;

                    using (var cs = new CryptoStream(ms, cryptor.CreateEncryptor(key, iv), CryptoStreamMode.Write))
                    {
                        cs.Write(secret, 0, secret.Length);
                    }
                    var encryptedContent = ms.ToArray();

                    var result = new byte[iv.Length + encryptedContent.Length];
                    Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
                    Buffer.BlockCopy(encryptedContent, 0, result, iv.Length, encryptedContent.Length);

                    return result;
                }
            }
        }

        /// <summary>
        /// Decrypt a byte array using AES
        /// </summary>
        /// <param name="encrypted"></param>
        /// <param name="key">128 bit key</param>
        /// <returns>decrypted bytes</returns>
        public static byte[] AesDecryptBytes(byte[] encrypted, byte[] key)
        {
            var iv = new byte[16];
            var encryptedContent = new byte[encrypted.Length - 16];
            
            Buffer.BlockCopy(encrypted, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(encrypted, iv.Length, encryptedContent, 0, encryptedContent.Length);

            using (var ms = new MemoryStream())
            {
                try
                {
                    using (var cryptor = new AesManaged())
                    {
                        cryptor.Mode = CipherMode.CBC;
                        cryptor.Padding = PaddingMode.PKCS7;
                        cryptor.KeySize = key.Length * 8;
                        cryptor.BlockSize = 128;

                        using (var cs = new CryptoStream(ms, cryptor.CreateDecryptor(key, iv), CryptoStreamMode.Write))
                        {
                            cs.Write(encryptedContent, 0, encryptedContent.Length);
                        }
                        return ms.ToArray();
                    }
                }
                catch (CryptographicException)
                {
                    return null;
                }
            }
        }
    }
}