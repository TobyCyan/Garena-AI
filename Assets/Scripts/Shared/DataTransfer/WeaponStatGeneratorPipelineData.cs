public class WeaponStatGeneratorPipelineData : BasePipelineData
{
    public string Prompt { get; }

    public WeaponStatGeneratorPipelineData(string prompt)
    {
        Prompt = prompt;
    }
}