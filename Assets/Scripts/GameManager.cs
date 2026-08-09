using Player.Weapons;
using System;
using System.Threading.Tasks;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private const string DefaultChatReply =
        "Got it. Add more details, or confirm when the weapon description is ready.";

    [SerializeField] private PipelineManager pipelineManager;
    [SerializeField] private WeaponShowcaseManager weaponShowcaseManager;
    [SerializeField, TextArea] private string placeholderChatReply =
        DefaultChatReply;

    private void Start()
    {
        InitManagers();
    }

    private void InitManagers()
    {
        pipelineManager.Init();
        weaponShowcaseManager.Init();
    }

    public void RequestChatReply(string input, Action<string> onReply)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            Debug.LogWarning("Cannot request a chat reply without input.", this);
            return;
        }

        string reply = string.IsNullOrWhiteSpace(placeholderChatReply)
            ? DefaultChatReply
            : placeholderChatReply.Trim();
        onReply?.Invoke(reply);
    }

    /// <summary>
    /// Starts the pipeline asynchronously and reports the result through callbacks.
    /// </summary>
    public void ExecutePipelines(string input)
    {
        ExecutePipelines(input, null, null);
    }

    public void ExecutePipelines(
        string input,
        Action<BasePipelineData> onCompleted,
        Action<string> onFailed)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            Debug.LogWarning("Cannot execute pipelines without input.");
            onFailed?.Invoke("Cannot execute pipelines without input.");
            return;
        }

        WeaponStatsPipelineInputData inputData = new(input);
        _ = ExecutePipelineAsync(
            PipelineID.WeaponBuild,
            inputData,
            onCompleted,
            onFailed);
    }

    private async Task ExecutePipelineAsync(
        PipelineID id,
        BasePipelineData input,
        Action<BasePipelineData> onCompleted,
        Action<string> onFailed)
    {
        BasePipelineData output;

        try
        {
            output = await pipelineManager.ExecutePipelineAsync(id, input);
        }
        catch (Exception ex)
        {
            string message = $"Weapon generation failed: {ex.Message}";
            Debug.LogError(message, this);
            onFailed?.Invoke(message);
            return;
        }

        if (output == null)
        {
            const string message = "Weapon generation failed.";
            Debug.LogWarning(message, this);
            onFailed?.Invoke(message);
            return;
        }

        HandlePipelineResults(output);
        onCompleted?.Invoke(output);
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
