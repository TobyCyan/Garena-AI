public record SpriteGenerationRequest
{
    public string Prompt { get; }

    public SpriteGenerationRequest(string prompt)
    {
        Prompt = prompt;
    }
}