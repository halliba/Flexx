using System.Text;

namespace Flexx.Core
{
    internal static class Config
    {
        public static Encoding DefaultEncoding { get; set; } = Encoding.UTF8;

        public static (string Name, string Password) DefaultChatRoom { get; set; } = ("Public", "Public");

        public static int PacketTrials { get; set; } = 10;

        public static int PacketTimeout { get; set; } = 10;
    }
}