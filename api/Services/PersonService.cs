namespace api.Services;

public class PersonService : IPersonService
{
    private readonly ILogger<PersonService> _logger;

    public PersonService(ILogger<PersonService> logger)
    {
        _logger = logger;
    }

    public async Task<bool> GenerateLog(string message, CancellationToken cancellationToken = default)
    {
        return await Log(message);
    }

    private async Task<bool> Log(string message, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation(message);
            return await Task.FromResult(true);
        }
        catch (Exception e)
        {
            return await Task.FromResult(false);
        }

    }
}
