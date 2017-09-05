using Newtonsoft.Json;

namespace Flexx.Core
{
    public class UserIdentity
    {
        public bool Equals(UserIdentity other)
        {
            return string.Equals(Name, other.Name) && string.Equals(PublicKey, other.PublicKey);
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
                return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ (PublicKey != null ? PublicKey.GetHashCode() : 0);
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

        public UserIdentity(string name, string publicKey)
        {
            Name = name;
            PublicKey = publicKey;
        }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; }

        [JsonProperty(PropertyName = "publicKey")]
        public string PublicKey { get; }
    }
}