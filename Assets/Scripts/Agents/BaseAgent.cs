using System.Threading.Tasks;
using UnityEngine;

public abstract class BaseAgent : MonoBehaviour
{
    private static readonly ApiService apiService = new();

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
}
