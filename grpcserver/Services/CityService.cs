using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Bogus;
using grpcserver.Interfaces;

namespace grpcserver.Services
{
    public class CityService : Cities.CitiesBase
    {
        private readonly ILogger<CityService> _logger;
        private static ICityCachedData _cachedData;

        public CityService(ILogger<CityService> logger, ICityCachedData _cacheData)
        {
            _logger = logger;
            _cachedData = _cacheData;
        }
        private static async Task<List<CityResponse>> GetHttpCitiesAsync()
        {
            var list = await _cachedData.GetCities();

            if (list != null) return list;

            System.Console.WriteLine("Generated at " + DateTime.Now.ToString("yyyyMMdd_hhmm"));
            
            var rnd = new Random();
            list = new List<CityResponse>();            
            
            Parallel.For(1, rnd.Next(1, 10_000), (i, state) => 
            {
                var current = new Faker<CityResponse>()
                            .RuleFor(o => o.Id, f => f.Address.GetHashCode())
                            .RuleFor(o => o.Name, f => f.Address.City())
                            .Generate();
                list.Add(current);
            });

            await _cachedData.SetCities(list);
            return list;
        }        
        
        public override async Task<CityResponse> IsCity(CityByNameRequest request, ServerCallContext context)
        {
            var list = await GetHttpCitiesAsync();
            return list
                .FirstOrDefault(c => c.Name.Contains(request.Name, StringComparison.InvariantCultureIgnoreCase));
        }

        public override async Task GetCities(CityRequest request, 
            IServerStreamWriter<CityResponse> responseStream, 
            ServerCallContext context)
        {
            var list = await GetHttpCitiesAsync();
            foreach (var c in list)
            {
                await responseStream.WriteAsync(c);
            }
        }
    }
}
