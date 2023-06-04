using InMemoryCacheSample.Brokers;
using InMemoryCacheSample.Models;
using InMemoryCacheSample.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();
builder.Services.AddTransient<IWeatherBroker, WeatherBroker>();
builder.Services.AddTransient<IWeatherService, WeatherService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapGet("api/weather", async (IWeatherService service) =>
{
    var forecasts = await service.GetAsync();
    return Results.Ok(forecasts);
}).Produces<IEnumerable<WeatherForecast>>()
    .WithTags("Weather")
    .WithOpenApi();

app.Run();