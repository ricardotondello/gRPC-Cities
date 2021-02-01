using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Bogus;

namespace grpcserver.Services
{
    public class CityService : Cities.CitiesBase
    {
        private readonly ILogger<CityService> _logger;
        public CityService(ILogger<CityService> logger)
        {
            _logger = logger;
        }

        private static List<CityResponse> GetHttpCitiesAsync()
        {
            var rnd = new Random();
            return Enumerable.Range(1, rnd.Next(1, 1000)).Select(x => 
                new Faker<CityResponse>()
                    .RuleFor(o => o.Id, f => f.Address.GetHashCode())
                    .RuleFor(o => o.Name, f => f.Address.City())
                    .Generate())
                .ToList();
        }        
        
        public override Task<CityResponse> IsCity(CityByNameRequest request, ServerCallContext context)
        {
            var list = GetHttpCitiesAsync();
            return Task.FromResult(list
                .FirstOrDefault(c => c.Name.Contains(request.Name, StringComparison.InvariantCultureIgnoreCase)));
        }

        public override async Task GetCities(CityRequest request, 
            IServerStreamWriter<CityResponse> responseStream, 
            ServerCallContext context)
        {
            var list = GetHttpCitiesAsync();
            foreach (var c in list)
            {
                await responseStream.WriteAsync(c);
            }
        }
    }
}
