using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;

namespace Flexx.Core
{
    internal static class SignUtils
    {
        public static byte[] Sign(byte[] data, ICipherParameters privateKey)
        {
            var sig = SignerUtilities.GetSigner("SHA256withRSA");
            sig.Init(true, privateKey);

            sig.BlockUpdate(data, 0, data.Length);
            var signature = sig.GenerateSignature();

            return signature;
        }

        public static bool Verify(byte[] data, byte[] signature, ICipherParameters publicKey)
        {
            var signer = SignerUtilities.GetSigner("SHA256withRSA");

            signer.Init(false, publicKey);

            signer.BlockUpdate(data, 0, data.Length);
            return signer.VerifySignature(signature);
        }
    }
}