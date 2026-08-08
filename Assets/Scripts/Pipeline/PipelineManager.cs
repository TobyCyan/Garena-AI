using System.Collections.Generic;
using UnityEngine;

public class PipelineManager : MonoBehaviour
{
    private readonly Dictionary<PipelineID, Pipeline> pipelines = new();

    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        RegisterPipeline(
            PipelineID.WeaponStats,
            new WeaponStatGeneratorPipelineObject()
        );

        RegisterPipeline(
            PipelineID.SpriteGeneration,
            new SpriteGeneratorPipelineObject()
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

    public BasePipelineData ExecutePipeline(
        PipelineID id,
        BasePipelineData input)
    {
        if (!pipelines.TryGetValue(id, out Pipeline pipeline))
        {
            Debug.LogError($"Pipeline {id} was not found.");
            return null;
        }

        return pipeline.Pipe(input);
    }
}

public enum PipelineID
{
    WeaponBuild,
    SpriteGeneration,
    WeaponStats,
}
