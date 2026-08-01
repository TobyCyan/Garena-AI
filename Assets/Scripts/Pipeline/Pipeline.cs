using System.Collections.Generic;

public class Pipeline
{
    private readonly List<IPipelineObject> pipelineObjects = new();

    public Pipeline(List<IPipelineObject> pipelineObjects)
    {
        this.pipelineObjects = pipelineObjects
                ?? throw new PipelineException("Pipeline cannot have no objects.");
    }

    public BasePipelineData Pipe(BasePipelineData input)
    {
        if (pipelineObjects.Count == 0)
        {
            throw new PipelineException("Pipeline cannot have no objects.");
        }

        BasePipelineData output = input;

        foreach (IPipelineObject pipelineObject in pipelineObjects)
        {
            output = pipelineObject.DoPipe(output);
            if (output == null)
            {
                throw new PipelineException($"Pipeline object {pipelineObject.GetType().Name} returned null output.");
            }
        }

        return output;
    }
}
