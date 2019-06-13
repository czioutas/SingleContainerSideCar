using System.Threading.Tasks;

namespace SideCar.Services.Contracts
{
    public interface IServiceDiscoveryService
    {
        string GetService(string key);
    }
}
