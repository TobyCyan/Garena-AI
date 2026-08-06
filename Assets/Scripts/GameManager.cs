using Player.Weapons;
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

    public void ExecutePipeline(PipelineID id, BasePipelineData input)
    {
        BasePipelineData output = pipelineManager.ExecutePipeline(id, input);
        HandlePipelineResults(output);
    }

    private void HandlePipelineResults(BasePipelineData output)
    {
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
