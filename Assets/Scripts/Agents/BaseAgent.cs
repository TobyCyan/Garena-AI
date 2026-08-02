using System.Threading.Tasks;
using UnityEngine;

public abstract class BaseAgent
{
    private readonly IApiService apiService;

    protected BaseAgent(IApiService apiService)
    {
        this.apiService = apiService;
    }

    protected BaseAgent()
    {
        apiService = new ApiService();
    }

    protected async Task<string> Prompt(string prompt)
    {
        try
        {
            return await apiService.GetResponse(prompt);
        }
        catch (ApiException e)
        {
            Debug.Log(e.Message);
            return null;
        }
    }

    public abstract string GetPromptResults();
}
