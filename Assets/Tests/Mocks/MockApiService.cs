using System.Threading.Tasks;

public class MockApiService : ITextApiService
{
    public Task<string> GetResponse(string input)
    {
        return Task.FromResult($"Mock response from {input}");
    }
}
