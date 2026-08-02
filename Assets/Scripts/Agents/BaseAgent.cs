using System.Threading.Tasks;
using UnityEngine;

public abstract class BaseAgent
{
    private readonly ITextApiService apiService;

    protected BaseAgent(ITextApiService apiService)
    {
        this.apiService = apiService;
    }

    protected BaseAgent()
    {
        apiService = new ChatApiService();
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
