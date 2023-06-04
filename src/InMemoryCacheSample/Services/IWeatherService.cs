using InMemoryCacheSample.Models;

namespace InMemoryCacheSample.Services;

internal interface IWeatherService
{
    ValueTask<IEnumerable<WeatherForecast>> GetAsync();
}