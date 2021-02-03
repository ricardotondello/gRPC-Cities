using System.Collections.Generic;
using System.Threading.Tasks;

namespace grpcserver.Interfaces
{
    public interface ICityCachedData
    {
        Task<List<CityResponse>> GetCities();

        Task SetCities(List<CityResponse> list);
    }
}