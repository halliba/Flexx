using System.Net;

namespace Flexx.Core
{
    public class ChatPartner
    {
        public UserIdentity Identity { get; }

        public IPAddress RemoteAdress { get; }

        public ChatPartner(UserIdentity identity, IPAddress remoteAdress)
        {
            Identity = identity;
            RemoteAdress = remoteAdress;
        }
    }
}