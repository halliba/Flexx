namespace Flexx.Core
{
    public struct UserIdentity
    {
        public UserIdentity(string username, string publicKey)
        {
            Username = username;
            PublicKey = publicKey;
        }

        public bool Equals(UserIdentity other)
        {
            return string.Equals(Username, other.Username) && string.Equals(PublicKey, other.PublicKey);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is UserIdentity && Equals((UserIdentity) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Username != null ? Username.GetHashCode() : 0) * 397) ^ (PublicKey != null ? PublicKey.GetHashCode() : 0);
            }
        }

        public string Username { get; }

        public string PublicKey { get; }

        public static bool operator ==(UserIdentity ident1, UserIdentity ident2)
        {
            return ident1.Equals(ident2);
        }

        public static bool operator !=(UserIdentity ident1, UserIdentity ident2)
        {
            return !ident1.Equals(ident2);
        }
    }
}