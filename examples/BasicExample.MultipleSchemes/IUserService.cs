using System.Threading;
using System.Threading.Tasks;

namespace BasicExample.MultipleSchemes
{
    public interface IUserService
    {
        Task<AppUser?> GetUserAsync(string username, CancellationToken cancellationToken = default);
    }
}
