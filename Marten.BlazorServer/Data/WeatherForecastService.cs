namespace Marten.BlazorServer.Data;

public class WeatherForecastService
{
    private readonly IQuerySession _querySession;
    private readonly IDocumentSession _documentSession;

    public WeatherForecastService(IQuerySession querySession, IDocumentSession documentSession)
    {
        _querySession = querySession;
        _documentSession = documentSession;
    }

    public async Task<IReadOnlyList<WeatherForecast>> GetForecastAsync()
    {
        return await _querySession.Query<WeatherForecast>().ToListAsync();
    }

    public async Task AddForecastAsync(int temperatureC, string summary)
    {
        var forecast = new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now),
            TemperatureC = temperatureC,
            Summary = summary
        };
        _documentSession.Store(forecast);
        await _documentSession.SaveChangesAsync();
    }
}
