using System;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Generates an image through the OpenAI Images API
/// and returns the encoded image file as bytes.
/// </summary>
public class ImageApiService : IImageApiService
{
    private const string API_URL =
        "https://api.openai.com/v1/images/generations";
    private const string DEFAULT_RESOLUTION = "32x32";
    private const string DEFAULT_FORMAT = "png";

    private static string ApiKey =>
        Environment.GetEnvironmentVariable("OPENAI_API_KEY")
        ?? throw new InvalidOperationException(
            "OPENAI_API_KEY environment variable is not set.");

    public async Task<byte[]> GetImage(string prompt)
    {
        if (string.IsNullOrWhiteSpace(prompt))
        {
            throw new ApiException("Image prompt cannot be empty.");
        }

        var requestBody = new OpenAIImageRequest
        {
            model = "gpt-image-2",
            prompt = prompt,
            size = DEFAULT_RESOLUTION,
            quality = "low",
            output_format = DEFAULT_FORMAT
        };

        string json = JsonUtility.ToJson(requestBody);
        byte[] body = Encoding.UTF8.GetBytes(json);

        using UnityWebRequest request = new(
            API_URL,
            UnityWebRequest.kHttpVerbPOST);

        request.uploadHandler = new UploadHandlerRaw(body);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader(
            "Content-Type",
            "application/json");

        request.SetRequestHeader(
            "Authorization",
            $"Bearer {ApiKey}");

        UnityWebRequestAsyncOperation operation =
            request.SendWebRequest();

        while (!operation.isDone)
        {
            await Task.Yield();
        }

        if (request.result != UnityWebRequest.Result.Success)
        {
            throw new ApiException(
                $"OpenAI image request failed " +
                $"({request.responseCode}): " +
                request.downloadHandler.text);
        }

        OpenAIImageResponse response;

        try
        {
            response = JsonUtility.FromJson<OpenAIImageResponse>(
                request.downloadHandler.text);
        }
        catch (Exception exception)
        {
            throw new ApiException(
                $"Could not parse image response: {exception.Message}");
        }

        if (response?.data == null ||
            response.data.Length == 0 ||
            string.IsNullOrWhiteSpace(response.data[0].b64_json))
        {
            throw new ApiException(
                "OpenAI did not return image data.");
        }

        try
        {
            return Convert.FromBase64String(
                response.data[0].b64_json);
        }
        catch (FormatException exception)
        {
            throw new ApiException(
                $"OpenAI returned invalid Base64 image data: " +
                exception.Message);
        }
    }
}

[Serializable]
public class OpenAIImageRequest
{
    public string model;
    public string prompt;
    public string size;
    public string quality;
    public string output_format;
}

[Serializable]
public class OpenAIImageResponse
{
    public long created;
    public OpenAIImageData[] data;
}

[Serializable]
public class OpenAIImageData
{
    public string b64_json;
}
