using System;
using System.Reflection;

namespace Orchard.Security
{
    /// <summary>
    /// Used to identify a user.
    /// </summary>
    [Serializable]
    public class UserIdentifier 
    {

        /// <summary>
        /// UserName of the user.
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// Id of the user.
        /// </summary>
        public int UserId { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserIdentifier"/> class.
        /// </summary>
        protected UserIdentifier()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserIdentifier"/> class.
        /// </summary>
        /// <param name="userName">UserName of the user.</param>
        /// <param name="userId">Id of the user.</param>
        public UserIdentifier(string userName, int userId)
        {
            UserId = userId;
            UserName = userName;
        }

        /// <summary>
        /// Parses given string and creates a new <see cref="UserIdentifier"/> object.
        /// </summary>
        /// <param name="userIdentifierString">
        /// Should be formatted one of the followings:
        /// 
        /// - "userId@tenantId". Ex: "42@3" (for tenant users).
        /// - "userId". Ex: 1 (for host users)
        /// </param>
        public static UserIdentifier Parse(string userIdentifierString)
        {
            if (string.IsNullOrEmpty(userIdentifierString))
            {
                throw new ArgumentNullException("userIdentifierString can not be null or empty!");
            }

            var splitted = userIdentifierString.Split(new char[] { ':' },StringSplitOptions.RemoveEmptyEntries);
            if (splitted.Length == 1)
            {
                return new UserIdentifier(null,splitted[0].FromBase64().To<int>());
            }

            if (splitted.Length == 2)
            {
                return new UserIdentifier(splitted[1].FromBase64(), splitted[0].FromBase64().To<int>());
            }

            throw new ArgumentException("userIdentifierString is not properly formatted");
        }
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is UserIdentifier))
            {
                return false;
            }

            //Same instances must be considered as equal
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            //Transient objects are not considered as equal
            var other = (UserIdentifier)obj;

            //Must have a IS-A relation of types or must be same type
            var typeOfThis = GetType();
            var typeOfOther = other.GetType();
            if (!typeOfThis.GetTypeInfo().IsAssignableFrom(typeOfOther) && !typeOfOther.GetTypeInfo().IsAssignableFrom(typeOfThis))
            {
                return false;
            }

            return  UserId == other.UserId;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return UserId.GetHashCode();
        }

        /// <inheritdoc/>
        public static bool operator ==(UserIdentifier left, UserIdentifier right)
        {
            if (Equals(left, null))
            {
                return Equals(right, null);
            }

            return left.Equals(right);
        }

        /// <inheritdoc/>
        public static bool operator !=(UserIdentifier left, UserIdentifier right)
        {
            return !(left == right);
        }
    }
}
