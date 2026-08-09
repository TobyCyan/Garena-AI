using System.Threading.Tasks;
using System;

public class WeaponStatGeneratorPipelineObject : IPipelineObject
{
    public async Task<BasePipelineData> DoPipeAsync(BasePipelineData input)
    {
        if (input is not WeaponStatsPipelineInputData data)
        {
            throw new PipelineException("Input data must be of type WeaponStatsPipelineInputData.");
        }

        try
        {
            WeaponStatsData generatedStats =
                await WeaponStatsService.GenerateWeaponStatsAsync(data.Prompt);
            return new WeaponStatsPipelineData(generatedStats, data.Prompt);
        }
        catch (Exception e)
        {
            throw new PipelineException(
                $"Failed to generate weapon stats for prompt: {data.Prompt}",
                e);
        }
    }
}
