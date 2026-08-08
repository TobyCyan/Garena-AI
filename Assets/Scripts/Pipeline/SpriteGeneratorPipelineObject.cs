public class SpriteGeneratorPipelineObject : IPipelineObject
{
    public BasePipelineData DoPipe(BasePipelineData input)
    {
        if (input is not WeaponStatsPipelineData data)
        {
            throw new PipelineException("Input data must be of type WeaponStatsPipelineData.");
        }

        SpriteGenerationRequest request = new(data.Prompt, data.WeaponStatsData.Category);
        var sprite = SpriteService.GenerateSprite(request);
        return new SpriteGeneratorPipelineResultData(sprite);
    }
}