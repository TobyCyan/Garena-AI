using Player.Weapons;
using System.Threading.Tasks;
using System;

public class SpriteGeneratorPipelineObject : IPipelineObject
{
    public async Task<BasePipelineData> DoPipeAsync(BasePipelineData input)
    {
        if (input is not WeaponStatsPipelineData data)
        {
            throw new PipelineException("Input data must be of type WeaponStatsPipelineData.");
        }

        try
        {
            SpriteGenerationRequest request = new(data.Prompt, data.WeaponStatsData.Category);
            var sprite = await SpriteService.GenerateSpriteAsync(request);
            WeaponDefinition definition = new(
                "AI Weapon",
                data.WeaponStatsData.Category,
                sprite,
                data.WeaponStatsData.Stats);
            return new WeaponBuildRequestPipelineData(definition);
        }
        catch (Exception e)
        {
            throw new PipelineException(
                $"Failed to generate sprite for prompt: {data.Prompt}",
                e);
        }
    
    }
}
