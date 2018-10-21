using System;
using System.Collections.Generic;
using Nancy;
using Nancy.Security;
using Nancy.Authentication.Forms;

namespace PirateJusticeStatus.Infrastructure
{
    public class User : IUserIdentity
    {
		private List<string> _claims;
        
        public Guid Id { get; private set; }

		public IEnumerable<string> Claims { get { return _claims; } }

		public string UserName { get; private set; }

		public User(string userName, params string[] claims)
		{
			Id = Guid.NewGuid();
			UserName = userName;
			_claims = new List<string>(claims);
		}
    }

	public class UserMapper : IUserMapper
    {
		private Dictionary<Guid, User> _users;

        public UserMapper()
		{
			_users = new Dictionary<Guid, User>();
		}

        public void Add(User user)
		{
			_users.Add(user.Id, user);
		}

        public void Remove(User user)
		{
			_users.Remove(user.Id);
		}

        public IUserIdentity GetUserFromIdentifier(Guid identifier, NancyContext context)
        {
            if (_users.ContainsKey(identifier))
			{
				return _users[identifier];
			}
			else
			{
				return null;
			}
        }
    }
}
