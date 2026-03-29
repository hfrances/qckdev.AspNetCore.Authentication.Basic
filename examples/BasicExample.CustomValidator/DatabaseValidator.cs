using System.Threading;
using System.Threading.Tasks;
using qckdev.AspNetCore.Authentication.Basic;

namespace BasicExample.CustomValidator
{
    public sealed class DatabaseValidator : IBasicAuthenticationValidator
    {
        private readonly IUserService _userService;
        private readonly IPasswordHasher _passwordHasher;

        public DatabaseValidator(IUserService userService, IPasswordHasher passwordHasher)
        {
            _userService = userService;
            _passwordHasher = passwordHasher;
        }

        public async Task<bool> ValidateAsync(
            string username,
            string password,
            string authenticationScheme,
            CancellationToken cancellationToken = default)
        {
            var user = await _userService.GetUserAsync(username, cancellationToken);
            if (user == null)
            {
                return false;
            }

            return _passwordHasher.Verify(user.PasswordHash, password);
        }
    }
}
