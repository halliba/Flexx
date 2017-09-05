namespace Flexx.Core
{
    public struct ChatPartner
    {
        public UserIdentity Identity { get; }

        public bool IsPublic { get; }

        public static ChatPartner Public => new ChatPartner(null, true);

        public ChatPartner(UserIdentity identity)
        {
            Identity = identity;
            IsPublic = false;
        }

        private ChatPartner(UserIdentity identity, bool isPublic)
        {
            Identity = identity;
            IsPublic = isPublic;
        }
    }
}