using System.Threading.Tasks;

namespace AppFunctions.Services
{
    public interface IKeyVaultManager
    {
        Task<string> GetSecret(string secretName);
    }
}
