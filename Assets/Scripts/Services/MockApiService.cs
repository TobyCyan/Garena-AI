public class MockApiService : ApiService
{
    public override string GetResponse(string input)
    {
        return $"Mock response from {input}";
    }
}
