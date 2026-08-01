using System;

public class ApiService
{
    private static string ApiKey =>
            Environment.GetEnvironmentVariable("API_KEY")
                ?? throw new InvalidOperationException("API_KEY not set.");

    public virtual string GetResponse(string input)
    {
        return $"Response from {input}";
    }
}
