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

    public void ExecutePipelines(
        string input,
        Action<BasePipelineData> onCompleted,
        Action<string> onFailed)
    {
        ExecutePipelines(input, onCompleted, onFailed, null);
    }

    public void ExecutePipelines(
        string input,
        Action<BasePipelineData> onCompleted,
        Action<string> onFailed,
        Action onBeforeShowcase)
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
            onFailed,
            onBeforeShowcase);
    }

    private async Task ExecutePipelineAsync(
        PipelineID id,
        BasePipelineData input,
        Action<BasePipelineData> onCompleted,
        Action<string> onFailed,
        Action onBeforeShowcase)
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

        HandlePipelineResults(output, onCompleted, onFailed, onBeforeShowcase);
    }

    private void HandlePipelineResults(
        BasePipelineData output,
        Action<BasePipelineData> onCompleted,
        Action<string> onFailed,
        Action onBeforeShowcase)
    {
        if (output == null)
        {
            Debug.LogWarning("Pipeline output is null.");
            onFailed?.Invoke("Pipeline output is null.");
            return;
        }

        switch (output)
        {
            case WeaponBuildPipelineResultData weaponResult:
                HandleBuiltWeapon(weaponResult, onCompleted, onFailed, onBeforeShowcase);
                break;

            default:
                Debug.LogWarning("Unhandled pipeline result type.");
                onFailed?.Invoke("Unhandled pipeline result type.");
                break;
        }
    }

    private void HandleBuiltWeapon(WeaponBuildPipelineResultData weaponResult, Action<BasePipelineData> onCompleted, Action<string> onFailed, Action onBeforeShowcase)
    {
        Weapon builtWeapon = weaponResult.Weapon;
        if (builtWeapon == null)
        {
            Debug.LogWarning("Built weapon is null.");
            onFailed?.Invoke("Built weapon is null.");
            return;
        }

        void OnWeaponConfirmed()
        {
            EquipShowcasedWeapon(weaponResult);
            onCompleted?.Invoke(weaponResult);
            weaponShowcaseManager.OnWeaponConfirmed -= OnWeaponConfirmed;
        }

        onBeforeShowcase?.Invoke();
        weaponShowcaseManager.OnWeaponConfirmed += OnWeaponConfirmed;
        weaponShowcaseManager.ShowcaseWeapon(builtWeapon.gameObject);
    }

    private void EquipShowcasedWeapon(WeaponBuildPipelineResultData weaponResult)
    {
        WeaponInventory inventory = FindWeaponInventory();
        Weapon equippedWeapon = inventory != null
            ? inventory.EquipBuiltWeapon(weaponResult.Weapon)
            : null;

        if (equippedWeapon == null)
        {
            Debug.LogWarning("Showcased weapon could not be equipped.", this);
        }
    }

    private static WeaponInventory FindWeaponInventory()
    {
        global::Player.PlayerController player =
            FindAnyObjectByType<global::Player.PlayerController>(
                FindObjectsInactive.Include);
        if (player != null && player.WeaponInventory != null)
        {
            return player.WeaponInventory;
        }

        return FindAnyObjectByType<WeaponInventory>(
            FindObjectsInactive.Include);
    }
}
