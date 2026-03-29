using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BasicExample.MultipleSchemes
{
    public sealed class UserService : IUserService
    {
        private readonly IDictionary<string, AppUser> _users;

        public UserService()
        {
            _users = new Dictionary<string, AppUser>(StringComparer.Ordinal)
            {
                ["enterprise.user"] = new AppUser
                {
                    Username = "enterprise.user",
                    PasswordHash = PasswordHasher.Hash("enterprise-pass")
                }
            };
        }

        public Task<AppUser?> GetUserAsync(string username, CancellationToken cancellationToken = default)
        {
            _users.TryGetValue(username, out var user);
            return Task.FromResult(user);
        }
    }
}
