using System.Collections.Generic;
using UnityEngine;
using Player.Weapons;

[DefaultExecutionOrder(100)]
[DisallowMultipleComponent]
[RequireComponent(typeof(global::Player.PlayerController))]
public sealed class WeaponSwapTester : MonoBehaviour
{
    [SerializeField] private List<WeaponDefinition> testWeapons = new()
    {
        new WeaponDefinition(),
        new WeaponDefinition()
    };
    [SerializeField] private int startingSlot;

    private global::Player.PlayerController player;

    private void Awake()
    {
        player = GetComponent<global::Player.PlayerController>();
    }

    private void Start()
    {
        BuildTestWeapons();
    }

    [ContextMenu("Build Test Weapons During Play Mode")]
    private void BuildTestWeapons()
    {
        if (!Application.isPlaying)
        {
            Debug.LogWarning("[WeaponSwapTester] Enter Play Mode before building test weapons.", this);
            return;
        }

        if (player == null || player.WeaponInventory == null)
        {
            Debug.LogError("[WeaponSwapTester] Player or WeaponInventory is missing.", this);
            return;
        }

        List<int> builtWeaponIndices = new();
        for (int i = 0; i < testWeapons.Count; i++)
        {
            WeaponDefinition definition = testWeapons[i];
            if (definition == null || definition.Image == null)
            {
                Debug.LogWarning($"[WeaponSwapTester] Slot {i} has no weapon image and was skipped.", this);
                continue;
            }

            if (player.WeaponInventory.BuildAndEquip(definition, i) != null)
            {
                builtWeaponIndices.Add(player.WeaponInventory.CurrentIndex);
            }
        }

        if (builtWeaponIndices.Count > 0)
        {
            int builtSlot = Mathf.Clamp(startingSlot, 0, builtWeaponIndices.Count - 1);
            player.WeaponInventory.Equip(builtWeaponIndices[builtSlot]);
        }

        Debug.Log($"[WeaponSwapTester] Built {builtWeaponIndices.Count} test weapons. Press Enter to cycle weapons.", this);
    }
}
