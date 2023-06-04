using InMemoryCacheSample.Models;

namespace InMemoryCacheSample.Brokers;

internal interface IWeatherBroker
{
    ValueTask<IEnumerable<WeatherForecast>> GetForecastsAsync();
}