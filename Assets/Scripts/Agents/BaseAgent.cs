using System.Threading.Tasks;
using UnityEngine;

public abstract class BaseAgent : MonoBehaviour
{
    private static readonly ApiService apiService = new();

    protected async Task Prompt(string prompt)
    {
        await apiService.GetResponse(prompt);
    }
}
