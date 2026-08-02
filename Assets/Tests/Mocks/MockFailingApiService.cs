using System.Threading.Tasks;

public class MockFailingApiService : ITextApiService
{
    public Task<string> GetResponse(string input)
    {
        throw new ApiException("Api Failed.");
    }
}
