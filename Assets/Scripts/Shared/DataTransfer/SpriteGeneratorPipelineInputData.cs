public class SpriteGeneratorPipelineInputData : BasePipelineData
{
    public string Prompt { get; }

    public SpriteGeneratorPipelineInputData(string prompt)
    {
        Prompt = prompt;
    }
}