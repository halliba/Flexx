using Newtonsoft.Json;

namespace Flexx.Core
{
    public class UserIdentity
    {
        public bool Equals(UserIdentity other)
        {
            return string.Equals(Name, other.Name) && string.Equals(PublicKey, other.PublicKey) && IsPublic == other.IsPublic;
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
                var hashCode = Name != null ? Name.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ (PublicKey != null ? PublicKey.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ IsPublic.GetHashCode();
                return hashCode;
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

        private UserIdentity()
        {
            IsPublic = true;
        }

        public static UserIdentity Public => new UserIdentity();

        [JsonProperty(PropertyName = "name")]
        public string Name { get; }

        [JsonProperty(PropertyName = "publicKey")]
        public string PublicKey { get; }

        [JsonIgnore]
        public bool IsPublic { get; }
    }
}