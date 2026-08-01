using UnityEngine;

public interface IPipelineObject
{
    BasePipelineData DoPipe(BasePipelineData input);
}
