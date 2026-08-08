using Player.Weapons;

public class WeaponStatGeneratorPipelineObject : IPipelineObject
{
    public BasePipelineData DoPipe(BasePipelineData input)
    {
        if (input is not WeaponStatGeneratorPipelineData data)
        {
            throw new PipelineException("Input data must be of type WeaponStatGeneratorPipelineData.");
        }
        WeaponStats generatedStats = WeaponStatGeneratorService.GenerateWeaponStats(data.Prompt);
        return new WeaponStatBalancerPipelineData(generatedStats);
    }
}