using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private PipelineManager pipelineManager;

    private void Start()
    {
        InitManagers();
    }

    private void InitManagers()
    {
        pipelineManager.Init();
    }

    public void ExecutePipeline(PipelineID id, BasePipelineData input)
    {
        BasePipelineData output = pipelineManager.ExecutePipeline(id, input);
        // Handle the output as needed
    }
}