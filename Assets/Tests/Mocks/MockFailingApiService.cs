using System.Threading.Tasks;

public class MockFailingApiService : IApiService
{
    public Task<string> GetResponse(string input)
    {
        throw new ApiException("Api Failed.");
    }
}
