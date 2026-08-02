using System;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Api service to ChatGPT to return text only.
/// </summary>
public class ChatApiService : ITextApiService
{
    private const string API_URL = "https://api.openai.com/v1/responses";

    private static string ApiKey =>
        Environment.GetEnvironmentVariable("OPENAI_API_KEY")
        ?? throw new InvalidOperationException(
            "OPENAI_API_KEY environment variable is not set.");

    public async Task<string> GetResponse(string prompt)
    {
        if (string.IsNullOrWhiteSpace(prompt))
        {
            throw new ApiException("Input cannot be empty.");
        }

        var requestBody = new OpenAIRequest
        {
            model = "gpt-5.6",
            prompt = prompt
        };

        string json = JsonUtility.ToJson(requestBody);
        byte[] body = Encoding.UTF8.GetBytes(json);

        using UnityWebRequest request = new(
            API_URL,
            UnityWebRequest.kHttpVerbPOST);

        request.uploadHandler = new UploadHandlerRaw(body);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", $"Bearer {ApiKey}");

        var operation = request.SendWebRequest();

        while (!operation.isDone)
            await Task.Yield();

        if (request.result != UnityWebRequest.Result.Success)
        {
            throw new ApiException(
                $"OpenAI request failed ({request.responseCode}): " +
                request.downloadHandler.text);
        }

        return request.downloadHandler.text;
    }
}

[Serializable]
public class OpenAIRequest
{
    public string model;
    public string prompt;
}

[Serializable]
public class OpenAIContent
{
    public string type;
    public string text;
}
