using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Bogus;
using grpcserver.Extensions;
using Microsoft.Extensions.Caching.Distributed;

namespace grpcserver.Services
{
    public class CityService : Cities.CitiesBase
    {
        private readonly ILogger<CityService> _logger;
        private static IDistributedCache _cache;
        public CityService(ILogger<CityService> logger, IDistributedCache cache)
        {
            _logger = logger;
            _cache = cache;
        }

        private static async Task<CityResponse[]> GetHttpCitiesAsync()
        {
            var recordKey = nameof(CityResponse) + "_" + DateTime.Now.ToString("yyyyMMdd_hhmm");

            var list = await _cache.GetRecordAsync<CityResponse[]>(recordKey);

            if (list is { }) return list;

            System.Console.WriteLine("Generated at " + DateTime.Now.ToString("yyyyMMdd_hhmm"));
            
            var rnd = new Random();
            list = Enumerable.Range(1, rnd.Next(1, 10_000)).Select(x => 
                    new Faker<CityResponse>()
                        .RuleFor(o => o.Id, f => f.Address.GetHashCode())
                        .RuleFor(o => o.Name, f => f.Address.City())
                        .Generate())
                .ToArray();
            await _cache.SetRecordAsync(recordKey, list);
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
