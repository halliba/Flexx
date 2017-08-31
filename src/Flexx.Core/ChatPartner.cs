using System.Net;

namespace Flexx.Core
{
    public struct ChatPartner
    {
        public UserIdentity Identity { get; }

        public IPAddress RemoteAdress { get; }

        public bool IsPublic { get; }

        public static ChatPartner Public => new ChatPartner(null, null, true);

        public ChatPartner(UserIdentity identity, IPAddress remoteAdress)
        {
            Identity = identity;
            RemoteAdress = remoteAdress;
            IsPublic = false;
        }

        private ChatPartner(UserIdentity identity, IPAddress remoteAdress, bool isPublic)
        {
            Identity = identity;
            RemoteAdress = remoteAdress;
            IsPublic = isPublic;
        }
    }
}