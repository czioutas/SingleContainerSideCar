using Microsoft.AspNetCore.Http;
using SideCar.Settings;
using System.Threading.Tasks;

namespace SideCar.Services.Contracts
{
    public interface IOutgoingProxyService
    {
        Task ProcessRequestAsync(HttpContext context);
    }
}
