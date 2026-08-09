using System.Threading.Tasks;
using UnityEngine.Networking;
using static UnityEngine.Networking.UnityWebRequest;

/// <summary>
/// Calls the local backend to generate weapon stats.
/// </summary>
public class StatsApiService
{
    private const string API_URL = Config.BACKEND_API_URL + "/weapon/stats/";
    private const int REQUEST_TIMEOUT_SECONDS = 30;

    public static async Task<string> GetStats(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ApiException("Description cannot be empty.");
        }

        string url = $"{API_URL}?description={EscapeURL(description)}";

        UnityWebRequest request = Get(url);
        request.timeout = REQUEST_TIMEOUT_SECONDS;

        UnityWebRequestAsyncOperation operation = request.SendWebRequest();

        while (!operation.isDone)
        {
            await Task.Yield();
        }

        if (request.result != Result.Success)
        {
            throw new ApiException(
                $"Stats backend request failed " +
                $"({request.responseCode}): " +
                request.downloadHandler.text);
        }

        return request.downloadHandler.text;
    }
}
