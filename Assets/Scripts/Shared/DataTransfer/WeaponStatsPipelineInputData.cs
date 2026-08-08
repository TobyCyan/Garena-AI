public class WeaponStatsPipelineInputData : BasePipelineData
{
    public string Prompt { get; }

    public WeaponStatsPipelineInputData(string prompt)
    {
        Prompt = prompt;
    }
}