using System.Threading.Tasks;

public class MockApiService : ApiService
{
    public override Task<string> GetResponse(string input)
    {
        return Task.FromResult($"Mock response from {input}");
    }
}
