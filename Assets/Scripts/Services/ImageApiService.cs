using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using static UnityEngine.Networking.UnityWebRequest;

/// <summary>
/// Calls the backend to generate weapon sprite images based on a prompt.
/// </summary>
public class ImageApiService
{
    private const string API_URL = Config.BACKEND_API_URL + "/weapon/sprite/";

    public static async Task<byte[]> GetImage(string prompt)
    {
        if (string.IsNullOrWhiteSpace(prompt))
        {
            throw new ApiException("Image prompt cannot be empty.");
        }

        string url = $"{API_URL}?description={EscapeURL(prompt)}";

        UnityWebRequest request = Get(url);

        UnityWebRequestAsyncOperation operation = request.SendWebRequest();

        while (!operation.isDone)
        {
            await Task.Yield();
        }

        if (request.result != Result.Success)
        {
            throw new ApiException(
                $"Image backend request failed " +
                $"({request.responseCode}): " +
                request.downloadHandler.text);
        }

        ImageResponse response;

        try
        {
            response = JsonUtility.FromJson<ImageResponse>(
                request.downloadHandler.text);
        }
        catch (Exception exception)
        {
            throw new ApiException(
                $"Could not parse image response: " +
                $"{exception.Message}");
        }

        if (response == null ||
            string.IsNullOrWhiteSpace(response.imageBase64))
        {
            throw new ApiException(
                "Backend did not return image data.");
        }

        try
        {
            return Convert.FromBase64String(response.imageBase64);
        }
        catch (FormatException exception)
        {
            throw new ApiException(
                $"Backend returned invalid Base64 image data: " +
                exception.Message);
        }
    }
}

[Serializable]
public class ImageResponse
{
    public string message;
    public string mimeType;
    public string imageBase64;
}
