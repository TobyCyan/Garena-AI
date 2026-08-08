public class WeaponStatGeneratorPipelineObject : IPipelineObject
{
    public BasePipelineData DoPipe(BasePipelineData input)
    {
        if (input is not WeaponStatGeneratorPipelineData data)
        {
            throw new PipelineException("Input data must be of type WeaponStatGeneratorPipelineData.");
        }
        var generatedStats = WeaponStatGeneratorService.GenerateWeaponStats(data.Prompt);
        // TODO: Return as WeaponStats object.
        // return new WeaponStatBalancerPipelineData(generatedStats);
        return new WeaponStatGeneratorPipelineData(generatedStats);
    }
}