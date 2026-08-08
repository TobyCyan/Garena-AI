using Player.Weapons;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private PipelineManager pipelineManager;
    [SerializeField] private WeaponShowcaseManager weaponShowcaseManager;

    private void Start()
    {
        InitManagers();
    }

    private void InitManagers()
    {
        pipelineManager.Init();
        weaponShowcaseManager.Init();
    }

    /// <summary>
    /// Executes the pipelines in parallel.
    /// </summary>
    public void ExecutePipelines()
    {
        // TODO: Create and populate input data for the pipelines
        BasePipelineData spriteInputData = new();
        StartCoroutine(ExecutePipelineCoroutine(PipelineID.SpriteGeneration, spriteInputData));

        BasePipelineData weaponInputData = new();
        StartCoroutine(ExecutePipelineCoroutine(PipelineID.WeaponStats, weaponInputData));
    }

    private IEnumerator ExecutePipelineCoroutine(PipelineID id, BasePipelineData input)
    {
        yield return null;
        BasePipelineData output = pipelineManager.ExecutePipeline
            (id, input);
        HandlePipelineResults(output);
    }

    private void HandlePipelineResults(BasePipelineData output)
    {
        if (output == null)
        {
            Debug.LogWarning("Pipeline output is null.");
            return;
        }

        switch (output)
        {
            case WeaponBuildPipelineResultData weaponResult:
                Weapon builtWeapon = weaponResult.Weapon;
                weaponShowcaseManager.ShowcaseWeapon(builtWeapon.gameObject);
                break;

            default:
                Debug.LogWarning("Unhandled pipeline result type.");
                break;
        }
    }
}
