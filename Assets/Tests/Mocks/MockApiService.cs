using System.Threading.Tasks;

public class MockApiService : IApiService
{
    public Task<string> GetResponse(string input)
    {
        return Task.FromResult($"Mock response from {input}");
    }
}
