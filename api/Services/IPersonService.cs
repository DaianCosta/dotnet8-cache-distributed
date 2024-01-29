namespace api.Services;

public interface IPersonService
{
    Task<bool> GenerateLog(string message, CancellationToken cancellationToken = default);
}
