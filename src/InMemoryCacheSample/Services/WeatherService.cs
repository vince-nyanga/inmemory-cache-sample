using InMemoryCacheSample.Brokers;
using InMemoryCacheSample.Models;
using Microsoft.Extensions.Caching.Memory;

namespace InMemoryCacheSample.Services;

internal sealed class WeatherService : IWeatherService
{
    private const string CacheKey = "WeatherForecasts";
    private static readonly SemaphoreSlim Semaphore = new(1,1);
    
    private readonly IWeatherBroker _broker;
    private readonly IMemoryCache _cache;
    private readonly ILogger<WeatherService> _logger;

    public WeatherService(IWeatherBroker broker, IMemoryCache cache, ILogger<WeatherService> logger)
    {
        _broker = broker;
        _cache = cache;
        _logger = logger;
    }

    public async ValueTask<IEnumerable<WeatherForecast>> GetAsync()
    {
        if (TryGetCachedForecasts(out var forecasts))
        {
            _logger.LogInformation("Returning cached forecasts");
            return forecasts;
        }
        
        try
        {
            await Semaphore.WaitAsync();
            
            if (TryGetCachedForecasts(out forecasts))
            {
                _logger.LogInformation("Returning cached forecasts");
                return forecasts;
            }
            
            _logger.LogInformation("Fetching forecasts from broker");
            
            forecasts = (await _broker.GetForecastsAsync()).ToArray();
            
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromSeconds(5))
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(10));
            
            _cache.Set(CacheKey, forecasts, cacheEntryOptions);
            
            return forecasts;
        }
        finally
        {
            Semaphore.Release();
        }
    }

    private bool TryGetCachedForecasts(out IEnumerable<WeatherForecast> forecasts) =>
        _cache.TryGetValue(CacheKey, out forecasts);
}