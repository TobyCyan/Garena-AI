using System.Threading.Tasks;

public class WeaponStatGeneratorPipelineObject : IPipelineObject
{
    public async Task<BasePipelineData> DoPipeAsync(BasePipelineData input)
    {
        if (input is not WeaponStatsPipelineInputData data)
        {
            throw new PipelineException("Input data must be of type WeaponStatsPipelineInputData.");
        }
        WeaponStatsData generatedStats =
            await WeaponStatsService.GenerateWeaponStatsAsync(data.Prompt);
        return new WeaponStatsPipelineData(generatedStats, data.Prompt);
    }
}
