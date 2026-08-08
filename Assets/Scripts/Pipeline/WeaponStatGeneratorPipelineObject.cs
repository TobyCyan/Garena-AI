using Player.Weapons;

public class WeaponStatGeneratorPipelineObject : IPipelineObject
{
    public BasePipelineData DoPipe(BasePipelineData input)
    {
        if (input is not WeaponStatsPipelineInputData data)
        {
            throw new PipelineException("Input data must be of type WeaponStatGeneratorPipelineData.");
        }
        WeaponStats generatedStats = WeaponStatsService.GenerateWeaponStats(data.Prompt);
        return new WeaponStatsPipelineData(generatedStats);
    }
}