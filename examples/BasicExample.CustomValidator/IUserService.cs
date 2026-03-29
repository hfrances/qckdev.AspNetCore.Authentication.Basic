using System.Threading;
using System.Threading.Tasks;

namespace BasicExample.CustomValidator
{
    public interface IUserService
    {
        Task<AppUser?> GetUserAsync(string username, CancellationToken cancellationToken = default);
    }
}
