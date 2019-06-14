using Microsoft.AspNetCore.Http;
using SideCar.Settings;
using System.Threading.Tasks;

namespace SideCar.Services.Contracts
{
    public interface IIncomingProxyService
    {
        Task ProcessRequestAsync(HttpContext context, SideCarSettings settings);
    }
}
