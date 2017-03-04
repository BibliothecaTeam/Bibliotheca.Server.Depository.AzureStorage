using System.Threading.Tasks;
using Hangfire.Server;

namespace Bibliotheca.Server.Depository.AzureStorage.Jobs
{
    public interface IServiceDiscoveryRegistrationJob
    {
        Task RegisterServiceAsync(PerformContext context);
    }
}