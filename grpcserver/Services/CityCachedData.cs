using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using grpcserver.Extensions;
using grpcserver.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace grpcserver.Services
{
    public class CityCachedData : ICityCachedData
    {
        private static string recordKey => nameof(CityResponse) + "_" + DateTime.Now.ToString("yyyyMMdd_hhmm");
        private static IDistributedCache _cache;
        public CityCachedData(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<List<CityResponse>> GetCities()
        {
            return await _cache.GetRecordAsync<List<CityResponse>>(recordKey);
        }

        public async Task SetCities(List<CityResponse> list)
        {
            await _cache.SetRecordAsync(recordKey, list);
        }
    }
}