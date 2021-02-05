# gRPC-Cities

Simple faker exemple of gRPC stream between server to client

- DotNet Core 3.1

- Docker

- REDIS

## REDIS DistributedCache implementation

### configuring services

```cs
services.AddStackExchangeRedisCache(options => 
{
    options.Configuration = config.GetConnectionString("Redis"); //Your connection string
    options.InstanceName = "Redis_gRPC_"; //Unique value to identify your's app key
});
```

### SetStringAsync

```cs
public static async Task SetRecordAsync<T>(this IDistributedCache cache, 
    string recordId, 
    T data, 
    TimeSpan? absoluteExireTime = null, 
    TimeSpan? unusedExpireTime = null)
{
    var option = new DistributedCacheEntryOptions
    {
        AbsoluteExpirationRelativeToNow = absoluteExireTime ?? TimeSpan.FromSeconds(60),
        SlidingExpiration = unusedExpireTime
    };

    var jsonData = JsonSerializer.Serialize(data);

    await cache.SetStringAsync(recordId, jsonData, option);
}
```

### GetStringAsync

```cs
public static async Task<T> GetRecordAsync<T>(this IDistributedCache cache, string recordId)
{
    var jsonData = await cache.GetStringAsync(recordId);

    return (jsonData != null) ? JsonSerializer.Deserialize<T>(jsonData) : default(T);
}
```

### Usage

### implementation of GetCities and SetCities

```cs
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
```

```cs
private static async Task<List<CityResponse>> GetHttpCitiesAsync()
{
    var list = await _cachedData.GetCities();

    if (list != null) return list;

    // Get from your database here
    list = new List<CityResponse>();
    list = GetFromYourDatabase();
    //

    //Set to REDIS Cache
    await _cachedData.SetCities(list);
    return list;
} 
```