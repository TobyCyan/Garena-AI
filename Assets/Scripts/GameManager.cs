using Player.Weapons;
using System;
using System.Collections;
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
    /// Executes the pipelines in parallel.
    /// </summary>
    public void ExecutePipelines(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            Debug.LogWarning("Cannot execute pipelines without input.");
            return;
        }

        WeaponStatsPipelineInputData inputData = new(input);
        StartCoroutine(ExecutePipelineCoroutine(PipelineID.WeaponBuild, inputData));
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
