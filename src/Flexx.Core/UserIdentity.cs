namespace Flexx.Core
{
    public class UserIdentity
    {
        public bool Equals(UserIdentity other)
        {
            return string.Equals(Username, other.Username) && string.Equals(PublicKey, other.PublicKey);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((UserIdentity) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Username != null ? Username.GetHashCode() : 0) * 397) ^ (PublicKey != null ? PublicKey.GetHashCode() : 0);
            }
        }

        public static bool operator ==(UserIdentity left, UserIdentity right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(UserIdentity left, UserIdentity right)
        {
            return !Equals(left, right);
        }

        public UserIdentity(string username, string publicKey)
        {
            Username = username;
            PublicKey = publicKey;
        }
        
        public string Username { get; }

        public string PublicKey { get; }
    }
}