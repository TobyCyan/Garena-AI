using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PipelineManager : MonoBehaviour
{
    private readonly Dictionary<PipelineID, Pipeline> pipelines = new();

    public void Init()
    {
        RegisterPipeline(
            PipelineID.WeaponBuild,
            new WeaponStatGeneratorPipelineObject(),
            new SpriteGeneratorPipelineObject(),
            new WeaponBuildPipelineObject()
        );
    }

    private void RegisterPipeline(
        PipelineID id,
        params IPipelineObject[] pipelineObjects)
    {
        if (pipelineObjects == null || pipelineObjects.Length == 0)
        {
            Debug.LogWarning(
                $"Cannot register pipeline {id} without pipeline objects.");
            return;
        }

        if (pipelines.ContainsKey(id))
        {
            Debug.LogWarning(
                $"Pipeline {id} is already registered.");
            return;
        }

        pipelines.Add(
            id,
            new Pipeline(new List<IPipelineObject>(pipelineObjects))
        );
    }

    public async Task<BasePipelineData> ExecutePipelineAsync(
        PipelineID id,
        BasePipelineData input)
    {
        if (!pipelines.TryGetValue(id, out Pipeline pipeline))
        {
            Debug.LogError($"Pipeline {id} was not found.");
            return null;
        }

        try
        {
            return await pipeline.PipeAsync(input);
        }
        catch (PipelineException ex)
        {
            Debug.LogError($"Pipeline {id} execution failed: {ex.Message}");
            return null;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Pipeline {id} execution encountered an unexpected error: {ex.Message}");
            return null;
        }
    }
}

public enum PipelineID
{
    WeaponBuild,
}
