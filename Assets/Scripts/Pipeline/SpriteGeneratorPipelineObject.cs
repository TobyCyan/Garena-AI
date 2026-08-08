public class SpriteGeneratorPipelineObject : IPipelineObject
{
    public BasePipelineData DoPipe(BasePipelineData input)
    {
        if (input is not SpriteGeneratorPipelineInputData data)
        {
            throw new PipelineException("Input data must be of type SpriteGeneratorPipelineInputData.");
        }

        SpriteGenerationRequest request = new(data.Prompt);
        var sprite = SpriteGenerationService.GenerateSprite(request);
        return new SpriteGeneratorPipelineResultData(sprite);
    }
}